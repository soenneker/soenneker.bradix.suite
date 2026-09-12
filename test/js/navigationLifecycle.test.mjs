import { test } from 'node:test';
import assert from 'node:assert/strict';
import * as navigation from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/navigationMenu.js';
import * as delegated from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/delegatedInteraction.js';

function environment() {
  const observers = [], calls = [], listeners = new Map();
  globalThis.ResizeObserver = class {
    elements = new Set();
    constructor(callback) { this.callback = callback; observers.push(this); }
    observe(element) { this.elements.add(element); }
    disconnect() { this.elements.clear(); }
  };
  globalThis.document = globalThis.window = {
    addEventListener(type, callback, capture = false) {
      const key = `${type}/${capture}`;
      if (!listeners.has(key)) listeners.set(key, new Set());
      listeners.get(key).add(callback);
    },
    removeEventListener(type, callback, capture = false) {
      listeners.get(`${type}/${capture}`)?.delete(callback);
    }
  };
  return { observers, calls, listeners,
    receiver: { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } },
    listenerCount() { return [...listeners.values()].reduce((count, entries) => count + entries.size, 0); }
  };
}

test('navigation viewport measures replacement content and suppresses unchanged sizes', () => {
  const env = environment();
  const viewport = {}, first = { offsetWidth: 100, offsetHeight: 50 }, second = { offsetWidth: 200, offsetHeight: 80 };
  try {
    navigation.registerNavigationMenuViewport(viewport, first, env.receiver);
    env.observers[0].callback();
    navigation.updateNavigationMenuViewport(viewport, first);
    assert.deepEqual(env.calls, [['HandleViewportSizeChanged', 100, 50]]);
    navigation.updateNavigationMenuViewport(viewport, second);
    assert.deepEqual(env.calls[1], ['HandleViewportSizeChanged', 200, 80]);
    assert.deepEqual([...env.observers[0].elements], [second]);
    second.offsetWidth = 250;
    env.observers[0].callback();
    assert.deepEqual(env.calls[2], ['HandleViewportSizeChanged', 250, 80]);
  } finally {
    navigation.unregisterNavigationMenuViewport(viewport);
  }
  assert.equal(env.observers[0].elements.size, 0);
});

test('one indicator observer tracks both elements without duplicate notifications', () => {
  const env = environment();
  const indicator = {}, trigger = { offsetWidth: 100, offsetLeft: 10 }, track = {};
  try {
    navigation.registerNavigationMenuIndicator(indicator, trigger, track, env.receiver, 'horizontal');
    assert.equal(env.observers.length, 1);
    assert.equal(env.observers[0].elements.size, 2);
    for (let i = 0; i < 100; i++) env.observers[0].callback();
    assert.equal(env.calls.length, 1);
    trigger.offsetLeft = 20;
    env.observers[0].callback();
    assert.deepEqual(env.calls[1], ['HandleIndicatorPositionChanged', 100, 20]);
  } finally {
    navigation.unregisterNavigationMenuIndicator(indicator);
  }
  assert.equal(env.observers[0].elements.size, 0);
  assert.equal(env.listenerCount(), 0);
});

test('delegated document listeners exist only while registrations remain', () => {
  const env = environment();
  const first = {}, second = {};
  try {
    delegated.registerDelegatedInteraction(first, env.receiver, {});
    assert.equal(env.listenerCount(), 10);
    delegated.registerDelegatedInteraction(first, env.receiver, {});
    delegated.registerDelegatedInteraction(second, env.receiver, {});
    assert.equal(env.listenerCount(), 10);
    delegated.unregisterDelegatedInteraction(first);
    delegated.unregisterDelegatedInteraction(first);
    assert.equal(env.listenerCount(), 10);
    delegated.unregisterDelegatedInteraction(second);
    assert.equal(env.listenerCount(), 0);
    delegated.registerDelegatedInteraction(first, env.receiver, {});
    assert.equal(env.listenerCount(), 10);
  } finally {
    delegated.unregisterDelegatedInteraction(first);
    delegated.unregisterDelegatedInteraction(second);
  }
  assert.equal(env.listenerCount(), 0);
});
