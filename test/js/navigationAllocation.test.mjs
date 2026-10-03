import { test } from 'node:test';
import assert from 'node:assert/strict';
import * as roving from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/rovingFocus.js';
import * as select from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/select.js';
import * as toast from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/toast.js';

class Element {
  listeners = new Map(); attributes = new Map(); style = {}; children = [];
  addEventListener(type, callback) {
    if (!this.listeners.has(type)) this.listeners.set(type, new Set());
    this.listeners.get(type).add(callback);
  }
  removeEventListener(type, callback) { this.listeners.get(type)?.delete(callback); }
  fire(type, values = {}) {
    const event = { target: this, key: '', preventDefault() {}, stopPropagation() {}, ...values };
    for (const callback of this.listeners.get(type) || []) callback(event);
  }
  getAttribute(name) { return this.attributes.get(name) ?? null; }
  setAttribute(name, value) { this.attributes.set(name, value); }
  hasAttribute(name) { return this.attributes.has(name); }
  removeAttribute(name) { this.attributes.delete(name); }
  contains(node) { return node === this || this.children.includes(node); }
  closest() { return this; }
  focus() { document.activeElement = this; }
  scrollIntoView() {}
}

function fixture(t) {
  globalThis.Node = globalThis.HTMLElement = globalThis.Element = Element;
  globalThis.HTMLInputElement = class extends Element {};
  globalThis.window = new Element(); globalThis.document = new Element();
  const timers = new Map(), frames = new Map(), observers = [], calls = [];
  let next = 1;
  const originalSetTimeout = globalThis.setTimeout, originalClearTimeout = globalThis.clearTimeout;
  globalThis.setTimeout = callback => { const id = next++; timers.set(id, callback); return id; };
  globalThis.clearTimeout = id => timers.delete(id);
  t.after(() => { globalThis.setTimeout = originalSetTimeout; globalThis.clearTimeout = originalClearTimeout; });
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  globalThis.ResizeObserver = globalThis.MutationObserver = class {
    constructor(callback) { this.callback = callback; observers.push(this); }
    observe() {} disconnect() {}
  };
  return { timers, frames, observers, calls,
    receiver: { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } },
    flushTimers() { const current = [...timers.values()]; timers.clear(); current.forEach(callback => callback()); }
  };
}

test('roving navigation skips disabled entries, wraps, and observes live mutations', t => {
  const env = fixture(t);
  const entries = Array.from({ length: 4 }, () => new Element());
  for (const entry of entries) {
    entry.setAttribute('data-bradix-roving-group', 'group');
    entry.setAttribute('data-bradix-roving-loop', 'true');
    roving.registerRovingFocusNavigationKeys(entry);
  }
  t.after(() => entries.forEach(entry => roving.unregisterRovingFocusNavigationKeys(entry)));
  document.querySelectorAll = () => entries;
  entries[1].setAttribute('disabled', '');
  const move = (index, key) => { document.fire('keydown', { target: entries[index], key }); env.flushTimers(); };
  move(0, 'ArrowRight'); assert.equal(document.activeElement, entries[2]);
  move(0, 'ArrowLeft'); assert.equal(document.activeElement, entries[3]);
  entries[0].setAttribute('data-bradix-roving-loop', 'false');
  move(0, 'ArrowLeft'); assert.equal(document.activeElement, entries[3]);
  entries[1].removeAttribute('disabled');
  move(0, 'ArrowRight'); assert.equal(document.activeElement, entries[1]);
  move(1, 'End'); assert.equal(document.activeElement, entries[3]);
  move(3, 'Home'); assert.equal(document.activeElement, entries[0]);
});

test('select keyboard navigation works with a NodeList and retains selection priority and typeahead', t => {
  const env = fixture(t), content = new Element();
  const entries = ['Alpha', 'Beta', 'Bravo'].map(text => {
    const option = new Element(); option.textContent = text; option.setAttribute('data-value', text); return option;
  });
  content.children = entries;
  const nodeList = { ...entries, length: entries.length, [Symbol.iterator]: function* () { yield* entries; } };
  content.querySelectorAll = selector => selector.includes('not(') ? nodeList : entries.filter(e => e.hasAttribute('data-highlighted'));
  entries[1].setAttribute('data-state', 'checked');
  select.registerSelectContentKeyboard(content, env.receiver);
  t.after(() => select.unregisterSelectContentKeyboard(content));
  content.fire('keydown', { key: 'ArrowDown' }); assert.equal(document.activeElement, entries[2]);
  content.fire('keydown', { key: 'Home' }); assert.equal(document.activeElement, entries[0]);
  content.fire('keydown', { key: 'b' }); assert.equal(document.activeElement, entries[1]);
  content.fire('keydown', { key: 'b' }); assert.equal(document.activeElement, entries[2]);
  assert.equal(env.timers.size, 1);
});

test('select viewport ignores stale observer work after unregister', t => {
  const env = fixture(t), viewport = new Element();
  select.registerSelectViewport(viewport, null, null, env.receiver);
  assert.equal(env.frames.size, 1);
  select.unregisterSelectViewport(viewport);
  env.observers[0].callback();
  assert.equal(env.frames.size, 0);
});

test('select viewport notifies only when measured values change', t => {
  const env = fixture(t), viewport = new Element();
  viewport.scrollTop = 0; viewport.scrollHeight = 500; viewport.offsetHeight = 100;
  const flush = () => { const pending = [...env.frames.values()]; env.frames.clear(); pending.forEach(fn => fn()); };
  select.registerSelectViewport(viewport, null, null, env.receiver);
  flush();
  for (let i = 0; i < 10; i++) { env.observers[0].callback(); flush(); }
  assert.deepEqual(env.calls, [['HandleViewportMetricsChanged', 0, 500, 100]]);
  viewport.scrollTop = 30; viewport.fire('scroll'); flush();
  assert.deepEqual(env.calls[1], ['HandleViewportMetricsChanged', 30, 500, 100]);
  select.unregisterSelectViewport(viewport);
});

test('unregistering a toast during a swipe releases its pointer capture', t => {
  const env = fixture(t), element = new Element(), released = [];
  element.dataset = {}; element.style = { setProperty() {}, removeProperty() {} };
  element.setPointerCapture = () => {}; element.releasePointerCapture = id => released.push(id);
  toast.registerToastSwipe(element, 'right', 50, false, false, env.receiver);
  element.fire('pointerdown', { button: 0, pointerId: 3, clientX: 0, clientY: 0 });
  element.fire('pointermove', { pointerId: 3, clientX: 20, clientY: 0 });
  toast.unregisterToastSwipe(element);
  assert.deepEqual(released, [3]);
  element.fire('pointerup', { pointerId: 3 });
  assert.deepEqual(env.calls, []);
});

test('toast hotkey frames coalesce and cannot steal focus after unregister', t => {
  const env = fixture(t), viewport = new Element(), outside = new Element();
  toast.registerToastViewport(null, viewport, null, null, ['F8'], env.receiver);
  for (let i = 0; i < 10; i++) document.fire('keydown', { key: 'F8' });
  assert.equal(document.activeElement, viewport);
  assert.equal(env.frames.size, 1);
  const stale = [...env.frames.values()][0];
  toast.unregisterToastViewport(viewport);
  outside.focus(); stale();
  assert.equal(env.frames.size, 0);
  assert.equal(document.activeElement, outside);
});
