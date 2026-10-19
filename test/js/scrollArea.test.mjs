import { test } from 'node:test';
import assert from 'node:assert/strict';
import * as scrollArea from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/scrollArea.js';

class Element {
  listeners = new Map();
  style = { setProperty(name, value) { this[name] = value; } };
  clientWidth = 100; clientHeight = 100;
  offsetWidth = 100; offsetHeight = 100;
  scrollWidth = 500; scrollHeight = 500;
  scrollLeft = 0; scrollTop = 0;
  captures = new Set();
  addEventListener(name, callback) {
    if (!this.listeners.has(name)) this.listeners.set(name, new Set());
    this.listeners.get(name).add(callback);
  }
  removeEventListener(name, callback) { this.listeners.get(name)?.delete(callback); }
  fire(name, values = {}) {
    const event = { target: this, preventDefault() { this.prevented = true; }, ...values };
    for (const callback of [...(this.listeners.get(name) || [])]) callback(event);
    return event;
  }
  contains(target) { return target === this; }
  getBoundingClientRect() { return { left: 0, top: 0, width: 100, height: 100 }; }
  setPointerCapture(id) { this.captures.add(id); }
  hasPointerCapture(id) { return this.captures.has(id); }
  releasePointerCapture(id) { this.captures.delete(id); }
}

function environment() {
  const frames = new Map();
  const observers = [];
  const calls = [];
  let next = 1;
  globalThis.HTMLElement = globalThis.Node = Element;
  globalThis.document = new Element();
  document.body = new Element();
  globalThis.getComputedStyle = () => ({ paddingLeft: '0', paddingRight: '0', paddingTop: '0', paddingBottom: '0' });
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  globalThis.ResizeObserver = class {
    constructor(callback) { this.callback = callback; observers.push(this); }
    observe() {}
    disconnect() { this.disconnected = true; }
  };
  return {
    frames, observers, calls,
    receiver: { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } },
    flush() { const pending = [...frames.values()]; frames.clear(); pending.forEach(callback => callback()); }
  };
}

test('viewport coalesces resize/scroll bursts and suppresses unchanged interop payloads', async () => {
  const env = environment();
  const viewport = new Element();
  scrollArea.registerScrollAreaViewport(viewport, null, env.receiver);
  for (let i = 0; i < 100; i++) env.observers[0].callback();
  assert.equal(env.frames.size, 1);
  env.flush();
  await Promise.resolve();
  assert.equal(env.calls.length, 1);
  viewport.fire('scroll'); env.flush();
  assert.equal(env.calls.length, 1);
  viewport.scrollTop = 20;
  viewport.fire('scroll'); env.flush();
  assert.equal(env.calls.length, 2);
  assert.equal(env.calls[1][2], 20);
  viewport.fire('scroll');
  scrollArea.unregisterScrollAreaViewport(viewport);
  assert.equal(env.frames.size, 0);
});

test('wheel interception is scoped to the scrollbar and still scrolls its viewport', () => {
  const env = environment();
  const bar = new Element(), viewport = new Element();
  scrollArea.registerScrollAreaScrollbar(bar, null, viewport, 'vertical', 'ltr', env.receiver);
  assert.equal(document.listeners.get('wheel')?.size || 0, 0);
  const event = bar.fire('wheel', { deltaX: 0, deltaY: 30 });
  assert.equal(viewport.scrollTop, 30);
  assert.equal(event.prevented, true);
  scrollArea.unregisterScrollAreaScrollbar(bar);
  assert.equal(bar.listeners.get('wheel').size, 0);
});

test('scrollbar metrics deduplicate frames and payloads and cancel disposed work', async () => {
  const env = environment();
  const bar = new Element();
  scrollArea.registerScrollAreaScrollbar(bar, null, new Element(), 'vertical', 'ltr', env.receiver);
  for (let i = 0; i < 100; i++) env.observers[0].callback();
  assert.equal(env.frames.size, 1);
  env.flush();
  await Promise.resolve();
  assert.equal(env.calls.length, 1);
  env.observers[0].callback(); env.flush();
  assert.equal(env.calls.length, 1);
  bar.clientHeight = 200;
  env.observers[0].callback(); env.flush();
  assert.equal(env.calls.length, 2);
  env.observers[0].callback();
  const staleFrame = [...env.frames.values()][0];
  scrollArea.unregisterScrollAreaScrollbar(bar);
  assert.equal(env.frames.size, 0);
  staleFrame();
  assert.equal(env.calls.length, 2);
});

test('horizontal RTL wheel scrolling preserves its negative scroll range', () => {
  const env = environment();
  const bar = new Element(), viewport = new Element();
  viewport.scrollLeft = -100;
  scrollArea.registerScrollAreaScrollbar(bar, null, viewport, 'horizontal', 'rtl', env.receiver);
  assert.equal(bar.fire('wheel', { deltaX: -30, deltaY: 0 }).prevented, true);
  assert.equal(viewport.scrollLeft, -130);
  bar.fire('wheel', { deltaX: -500, deltaY: 0 });
  assert.equal(viewport.scrollLeft, -400);
  assert.equal(bar.fire('wheel', { deltaX: -30, deltaY: 0 }).prevented, undefined);
  bar.fire('wheel', { deltaX: 500, deltaY: 0 });
  assert.equal(viewport.scrollLeft, 0);
  scrollArea.unregisterScrollAreaScrollbar(bar);
});

test('a second pointer cannot move or end an active scrollbar drag', () => {
  const env = environment();
  const bar = new Element(), viewport = new Element();
  scrollArea.registerScrollAreaScrollbar(bar, null, viewport, 'vertical', 'ltr', env.receiver);
  bar.fire('pointerdown', { button: 0, pointerId: 1, clientX: 10, clientY: 20 });
  const position = viewport.scrollTop;
  document.fire('pointermove', { pointerId: 2, clientY: 90 });
  document.fire('pointerup', { pointerId: 2 });
  assert.equal(viewport.scrollTop, position);
  assert.equal(bar.captures.has(1), true);
  document.fire('pointermove', { pointerId: 1, clientY: 90 });
  assert.notEqual(viewport.scrollTop, position);
  document.fire('pointerup', { pointerId: 1 });
  assert.equal(bar.captures.size, 0);
  scrollArea.unregisterScrollAreaScrollbar(bar);
});

for (const ending of ['dispose', 'pointercancel', 'pointerup']) {
  test(`drag cleanup restores selection and capture on ${ending}`, () => {
    const env = environment();
    const bar = new Element(), viewport = new Element();
    document.body.style.webkitUserSelect = 'text';
    viewport.style.scrollBehavior = 'smooth';
    scrollArea.registerScrollAreaScrollbar(bar, null, viewport, 'vertical', 'ltr', env.receiver);
    bar.fire('pointerdown', { button: 0, pointerId: 1, clientX: 10, clientY: 20 });
    assert.equal(document.body.style.webkitUserSelect, 'none');
    if (ending === 'dispose') scrollArea.unregisterScrollAreaScrollbar(bar);
    else document.fire(ending);
    assert.equal(document.body.style.webkitUserSelect, 'text');
    assert.equal(viewport.style.scrollBehavior, 'smooth');
    assert.equal(bar.captures.size, 0);
    for (const event of ['pointermove', 'pointerup', 'pointercancel'])
      assert.equal(document.listeners.get(event)?.size || 0, 0);
    scrollArea.unregisterScrollAreaScrollbar(bar);
  });
}

test('slow viewport receiver retains only the latest metrics and drops queued work on teardown', async () => {
  const env = environment(), viewport = new Element();
  const calls = []; let complete;
  const receiver = { invokeMethodAsync(...args) { calls.push(args); return new Promise(resolve => { complete = resolve; }); } };
  scrollArea.registerScrollAreaViewport(viewport, null, receiver);
  env.flush();
  for (let i = 1; i <= 100; i++) { viewport.scrollTop = i; viewport.fire('scroll'); env.flush(); }
  assert.equal(calls.length, 1);
  complete(); await Promise.resolve();
  assert.equal(calls.length, 2); assert.equal(calls[1][2], 100);
  viewport.scrollTop = 101; viewport.fire('scroll'); env.flush();
  scrollArea.unregisterScrollAreaViewport(viewport);
  complete(); await Promise.resolve();
  assert.equal(calls.length, 2);
});

test('thumb motion stays local during blocked metrics and handles RTL and disposal', () => {
  const env = environment();
  const bar = new Element(), viewport = new Element();
  scrollArea.registerScrollAreaScrollbar(bar, null, viewport, 'horizontal', 'rtl', { invokeMethodAsync() { return new Promise(() => {}); } });
  env.flush();
  assert.equal(bar.style['--bradix-scroll-thumb-size'], '20px');
  assert.equal(bar.style['--bradix-scroll-thumb-offset'], '80px');
  viewport.scrollLeft = -200;
  viewport.fire('scroll'); env.flush();
  assert.equal(bar.style['--bradix-scroll-thumb-offset'], '40px');
  scrollArea.unregisterScrollAreaScrollbar(bar);
  assert.equal(viewport.listeners.get('scroll').size, 0);
});
