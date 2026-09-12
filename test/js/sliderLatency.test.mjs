import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerSliderPointerBridge, unregisterSliderPointerBridge } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/controls.js';

function deferred() {
  let resolve;
  const promise = new Promise(complete => { resolve = complete; });
  return { promise, resolve };
}
const settle = () => new Promise(resolve => setImmediate(resolve));

function target() {
  const listeners = new Map();
  return { listeners,
    addEventListener(type, callback) {
      if (!listeners.has(type)) listeners.set(type, new Set());
      listeners.get(type).add(callback);
    },
    removeEventListener(type, callback) { listeners.get(type)?.delete(callback); },
    emit(type, values = {}) {
      const event = { button: 0, pointerId: 1, clientX: 10, clientY: 10, target: this, preventDefault() {}, ...values };
      return Promise.all([...(listeners.get(type) || [])].map(callback => callback(event)));
    }
  };
}

function fixture(t) {
  globalThis.document = target();
  globalThis.window = { performance: { now: () => 1000 } };
  globalThis.ResizeObserver = globalThis.MutationObserver = class { observe() {} disconnect() {} };
  const frames = new Map(), calls = [], start = deferred();
  let next = 1;
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  const element = Object.assign(target(), {
    captured: null,
    querySelectorAll: () => [],
    getBoundingClientRect: () => ({ left: 0, top: 0, width: 100, height: 100 }),
    setPointerCapture(id) { this.captured = id; },
    hasPointerCapture(id) { return this.captured === id; },
    releasePointerCapture(id) { assert.equal(this.captured, id); this.captured = null; }
  });
  const receiver = {
    moveGate: null,
    invokeMethodAsync(...args) {
      calls.push(args);
      return args[0] === 'HandlePointerStart' ? start.promise : args[0] === 'HandlePointerMove' && this.moveGate ? this.moveGate.promise : Promise.resolve();
    }
  };
  registerSliderPointerBridge(element, receiver);
  t.after(() => unregisterSliderPointerBridge(element));
  return { element, frames, calls, start, receiver,
    flush() { const callbacks = [...frames.values()]; frames.clear(); callbacks.forEach(callback => callback()); }
  };
}

test('a slider release before PointerStart completes commits the final value and detaches immediately', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  const end = document.emit('pointerup', { clientX: 75, clientY: 50 });
  assert.equal(document.listeners.get('pointermove').size, 0);
  assert.equal(env.element.captured, null);
  env.start.resolve();
  await end;
  assert.deepEqual(env.calls, [
    ['HandlePointerStart', .1, .1, -1], ['HandlePointerMove', .75, .5], ['HandlePointerEnd']
  ]);
});

test('disposal during pending PointerStart cannot reattach document listeners or invoke movement', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  await document.emit('pointermove', { clientX: 80 });
  env.flush();
  unregisterSliderPointerBridge(env.element);
  env.start.resolve();
  await settle();
  assert.deepEqual(env.calls.map(call => call[0]), ['HandlePointerStart']);
  assert.equal(env.element.captured, null);
  assert.equal(document.listeners.get('pointermove').size, 0);
  assert.equal(document.listeners.get('pointerup').size, 0);
  assert.equal(env.frames.size, 0);
});

test('pending slider moves coalesce across a slow server response without overlapping calls', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  env.start.resolve();
  env.receiver.moveGate = deferred();
  await document.emit('pointermove', { clientX: 20 });
  env.flush();
  await settle();
  for (let x = 21; x <= 80; x++) {
    await document.emit('pointermove', { clientX: x });
    env.flush();
  }
  assert.equal(env.calls.filter(call => call[0] === 'HandlePointerMove').length, 1);
  env.receiver.moveGate.resolve();
  await settle();
  assert.equal(env.frames.size, 1);
  env.flush();
  await settle();
  assert.deepEqual(env.calls.at(-1), ['HandlePointerMove', .8, .1]);
});

test('foreign pointers do not overwrite movement or end the active slider gesture', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  await document.emit('pointermove', { pointerId: 2, clientX: 80 });
  await document.emit('pointerup', { pointerId: 2 });
  assert.equal(env.frames.size, 0);
  assert.equal(env.element.captured, 1);
  env.start.resolve();
  await document.emit('pointerup', { clientX: 40 });
  assert.deepEqual(env.calls.at(-2), ['HandlePointerMove', .4, .1]);
});

test('cancellation while PointerStart is pending cancels instead of committing', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  const cancel = document.emit('pointercancel');
  env.start.resolve();
  await cancel;
  assert.deepEqual(env.calls.map(call => call[0]), ['HandlePointerStart', 'HandlePointerCancel']);
  assert.equal(env.element.captured, null);
});

test('release waits for an in-flight move then commits the release coordinate', async t => {
  const env = fixture(t);
  await env.element.emit('pointerdown');
  env.start.resolve();
  env.receiver.moveGate = deferred();
  await document.emit('pointermove', { clientX: 20 });
  env.flush();
  await settle();
  const end = document.emit('pointerup', { clientX: 90 });
  env.receiver.moveGate.resolve();
  await end;
  assert.deepEqual(env.calls.map(call => call[0]), ['HandlePointerStart', 'HandlePointerMove', 'HandlePointerMove', 'HandlePointerEnd']);
  assert.deepEqual(env.calls.at(-2), ['HandlePointerMove', .9, .1]);
  assert.equal(env.frames.size, 0);
});
