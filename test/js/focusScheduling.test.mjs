import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerFocusScope, unregisterFocusScope } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/focusScope.js';

class ElementStub {
  tagName = 'BUTTON';
  children = [];
  parentElement = null;
  tabIndex = 0;
  hasAttribute() { return false; }
  addEventListener() {}
  removeEventListener() {}
  focus() { document.activeElement = this; }
  contains(element) { return element === this || this.children.includes(element); }
}

function fixture(t) {
  const timers = new Map(), frames = new Map(), observers = [];
  let next = 1;
  const originalSetTimeout = globalThis.setTimeout, originalClearTimeout = globalThis.clearTimeout;
  globalThis.setTimeout = callback => { const id = next++; timers.set(id, callback); return id; };
  globalThis.clearTimeout = id => timers.delete(id);
  globalThis.requestAnimationFrame = callback => { const id = next++; frames.set(id, callback); return id; };
  globalThis.cancelAnimationFrame = id => frames.delete(id);
  globalThis.HTMLElement = ElementStub;
  globalThis.HTMLInputElement = class extends ElementStub {};
  globalThis.NodeFilter = { SHOW_ELEMENT: 1, FILTER_ACCEPT: 1, FILTER_SKIP: 3 };
  globalThis.MutationObserver = class {
    constructor(callback) { this.callback = callback; observers.push(this); }
    observe() {}
    disconnect() {}
  };
  const root = new ElementStub(), child = new ElementStub(), previous = new ElementStub();
  child.parentElement = root;
  globalThis.document = {
    activeElement: previous, body: previous, addEventListener() {}, removeEventListener() {},
    createTreeWalker(container) {
      let index = 0;
      return { currentNode: null, nextNode() { return this.currentNode = container.children[index++]; } };
    }
  };
  const receiver = { invokeMethodAsync: () => Promise.resolve(false) };
  t.after(async () => {
    await unregisterFocusScope(root, true);
    for (const callback of timers.values()) callback();
    globalThis.setTimeout = originalSetTimeout;
    globalThis.clearTimeout = originalClearTimeout;
  });
  return { timers, frames, observers, root, child, receiver };
}

test('successful initial autofocus does not allocate five retry timers and a frame', async t => {
  const env = fixture(t);
  env.root.children.push(env.child);
  await registerFocusScope(env.root, env.receiver, true, true, false, false);
  assert.equal(document.activeElement, env.child);
  assert.equal(env.timers.size, 0);
  assert.equal(env.frames.size, 0);
});

test('late focusable content cancels the remaining retries after it receives focus', async t => {
  const env = fixture(t);
  await registerFocusScope(env.root, env.receiver, true, true, false, false);
  assert.equal(env.timers.size, 5);
  assert.equal(env.frames.size, 1);
  env.root.children.push(env.child);
  env.observers[0].callback([{ addedNodes: [env.child], removedNodes: [], type: 'childList' }]);
  assert.equal(document.activeElement, env.child);
  assert.equal(env.timers.size, 0);
  assert.equal(env.frames.size, 0);
});

test('unmount clears pending mount retries while retaining its one focus-restoration task', async t => {
  const env = fixture(t);
  await registerFocusScope(env.root, env.receiver, true, true, false, false);
  await unregisterFocusScope(env.root, true);
  assert.equal(env.timers.size, 1);
  assert.equal(env.frames.size, 0);
});
