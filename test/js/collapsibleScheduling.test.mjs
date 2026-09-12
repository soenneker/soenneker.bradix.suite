import { test } from 'node:test';
import assert from 'node:assert/strict';
import { observeCollapsibleContent, unobserveCollapsibleContent } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/collapsible.js';

function fixture(t) {
  const observers = [], frames = new Map(), timers = new Map(), styles = new Map();
  let next = 1, reads = 0, writes = 0;
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  globalThis.setTimeout = callback => { const id = next++; timers.set(id, callback); return id; };
  globalThis.clearTimeout = id => timers.delete(id);
  globalThis.ResizeObserver = globalThis.MutationObserver = class {
    constructor(callback) { this.callback = callback; observers.push(this); }
    observe() {}
    disconnect() { this.disconnected = true; }
  };
  const element = { hidden: false, height: 200, width: 300,
    get scrollHeight() { reads++; return this.height; },
    get scrollWidth() { reads++; return this.width; },
    style: { getPropertyValue: key => styles.get(key) ?? '', setProperty(key, value) { writes++; styles.set(key, value); } }
  };
  t.after(() => unobserveCollapsibleContent(element));
  return { element, frames, timers, observers, styles, reads: () => reads, writes: () => writes,
    flush() { const callbacks = [...frames.values()]; frames.clear(); callbacks.forEach(callback => callback()); }
  };
}

test('initial size remains synchronous and repeated observers share one frame without redundant writes', t => {
  const env = fixture(t);
  observeCollapsibleContent(env.element);
  assert.equal(env.writes(), 2);
  assert.equal(env.frames.size, 1);
  for (let i = 0; i < 100; i++) env.observers.forEach(observer => observer.callback());
  assert.equal(env.reads(), 2);
  env.flush();
  assert.equal(env.reads(), 4);
  assert.equal(env.writes(), 2);
  env.element.height = 450;
  env.observers[0].callback();
  env.flush();
  assert.equal(env.writes(), 3);
  assert.equal(env.styles.get('--radix-collapsible-content-height'), '450px');
});

test('hidden and zero-size measurements preserve the last usable animation dimensions', t => {
  const env = fixture(t);
  observeCollapsibleContent(env.element);
  env.element.hidden = true;
  env.flush();
  assert.equal(env.reads(), 2);
  env.element.hidden = false;
  env.element.width = env.element.height = 0;
  env.observers[0].callback();
  env.flush();
  assert.equal(env.writes(), 2);
});

test('teardown cancels queued work and ignores already-delivered observer callbacks', t => {
  const env = fixture(t);
  observeCollapsibleContent(env.element);
  const fallback = [...env.timers.values()][0];
  unobserveCollapsibleContent(env.element);
  env.observers.forEach(observer => { assert.equal(observer.disconnected, true); observer.callback(); });
  fallback();
  assert.equal(env.frames.size, 0);
  assert.equal(env.timers.size, 0);
  assert.equal(env.reads(), 2);
});

test('reobservation replaces the old schedule', t => {
  const env = fixture(t);
  observeCollapsibleContent(env.element);
  const previous = env.observers[0];
  observeCollapsibleContent(env.element);
  assert.equal(previous.disconnected, true);
  previous.callback();
  assert.equal(env.frames.size, 1);
  assert.equal(env.timers.size, 1);
});
