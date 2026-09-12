import { test } from 'node:test';
import assert from 'node:assert/strict';
import * as popper from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/popper.js';

function deferred() {
  let resolve, reject;
  const promise = new Promise((yes, no) => { resolve = yes; reject = no; });
  return { promise, resolve, reject };
}

function fixture(t) {
  const frames = new Map(), computations = [], observers = [], notifications = [];
  let next = 1;
  globalThis.Element = class {};
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  globalThis.getComputedStyle = () => ({ zIndex: 'auto' });
  globalThis.document = { querySelectorAll: () => [] };
  globalThis.DOMRect = { fromRect: rect => rect };
  globalThis.FloatingUIDOM = {
    offset: options => options, flip: options => options, shift: options => options,
    limitShift: () => ({}), size: options => options, arrow: options => options, hide: options => options,
    autoUpdate(reference, content, callback) {
      const observer = { callback, disconnected: false };
      observers.push(observer);
      callback();
      return () => { observer.disconnected = true; };
    },
    computePosition(reference, content, options) {
      const computation = { ...deferred(), options };
      computations.push(computation);
      return computation.promise;
    }
  };
  const content = { style: { removeProperty() {} } };
  const receiver = { invokeMethodAsync(...args) { notifications.push(args); return Promise.resolve(); } };
  const register = () => popper.registerVirtualPopperContent(content, null, receiver, 0, 0, {});
  register();
  t.after(() => popper.unregisterPopperContent(content));
  return { frames, computations, observers, notifications, content, register,
    runFrame() {
      const callbacks = [...frames.values()];
      frames.clear();
      return Promise.all(callbacks.map(callback => callback()));
    },
    result(index, x = 10) {
      computations[index].resolve({ x, y: 20, placement: computations[index].options.placement, middlewareData: {} });
    }
  };
}

test('popper coalesces observer bursts and never overlaps positioning computations', async t => {
  const env = fixture(t);
  for (let i = 0; i < 100; i++) env.observers[0].callback();
  assert.equal(env.frames.size, 1);
  const first = env.runFrame();
  assert.equal(env.computations.length, 1);
  for (let i = 0; i < 100; i++) env.observers[0].callback();
  assert.equal(env.frames.size, 0);
  assert.equal(env.computations.length, 1);
  env.result(0);
  await first;
  assert.equal(env.frames.size, 1);
  const second = env.runFrame();
  env.result(1, 30);
  await second;
  assert.equal(env.notifications.length, 2);
  assert.equal(env.notifications[1][3], 30);
});

test('option updates invalidate an in-flight position and render the new placement', async t => {
  const env = fixture(t);
  const first = env.runFrame();
  popper.updatePopperContent(env.content, null, { side: 'top' });
  env.result(0);
  await first;
  assert.equal(env.notifications.length, 0);
  const second = env.runFrame();
  assert.equal(env.computations[1].options.placement, 'top');
  env.result(1);
  await second;
  assert.equal(env.notifications[0][1], 'top');
});

test('unregister cancels queued work and ignores stale observer callbacks', async t => {
  const env = fixture(t);
  popper.unregisterPopperContent(env.content);
  env.observers[0].callback();
  assert.equal(env.frames.size, 0);
  await env.runFrame();
  assert.equal(env.computations.length, 0);
  assert.ok(env.observers[0].disconnected);
});

test('unregister suppresses results from a computation already in flight', async t => {
  const env = fixture(t);
  const running = env.runFrame();
  env.observers[0].callback();
  popper.unregisterPopperContent(env.content);
  env.result(0);
  await running;
  assert.equal(env.notifications.length, 0);
  assert.equal(env.frames.size, 0);
});

test('replacing a registration ignores old results and observers', async t => {
  const env = fixture(t);
  const first = env.runFrame();
  env.register();
  env.observers[0].callback();
  const second = env.runFrame();
  env.result(1, 30);
  await second;
  env.result(0, 10);
  await first;
  assert.equal(env.notifications.length, 1);
  assert.equal(env.notifications[0][3], 30);
  assert.equal(env.frames.size, 0);
});

test('unchanged positions do not cross the JS interop boundary again', async t => {
  const env = fixture(t);
  const first = env.runFrame();
  env.result(0);
  await first;
  env.observers[0].callback();
  const second = env.runFrame();
  env.result(1);
  await second;
  assert.equal(env.notifications.length, 1);
});
