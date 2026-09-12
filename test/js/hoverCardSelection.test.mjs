import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerHoverCardSelectionContainment as register, beginHoverCardSelectionContainment as begin,
  unregisterHoverCardSelectionContainment as unregister } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/hoverCardAvatar.js';

function fixture(t) {
  const listeners = new Map(), timers = new Map(), calls = [];
  let next = 1;
  globalThis.document = {
    body: { style: { userSelect: 'auto', webkitUserSelect: 'contain' } },
    getSelection: () => ({ toString: () => 'selected text' }),
    addEventListener(type, callback) { listeners.set(type, callback); },
    removeEventListener(type, callback) { if (listeners.get(type) === callback) listeners.delete(type); }
  };
  globalThis.setTimeout = callback => { const id = next++; timers.set(id, callback); return id; };
  globalThis.clearTimeout = id => timers.delete(id);
  const content = { style: { userSelect: 'all', webkitUserSelect: 'auto' } };
  const receiver = { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } };
  register(content, receiver);
  t.after(() => unregister(content));
  return { content, receiver, listeners, timers, calls,
    emit(type) { listeners.get(type)?.(); },
    flush() { const callbacks = [...timers.values()]; timers.clear(); callbacks.forEach(callback => callback()); },
    assertRestored() {
      assert.deepEqual(content.style, { userSelect: 'all', webkitUserSelect: 'auto' });
      assert.deepEqual(document.body.style, { userSelect: 'auto', webkitUserSelect: 'contain' });
      assert.equal(listeners.size, 0);
    }
  };
}

test('idle hover cards have no document listeners; only an active selection listens for release', t => {
  const f = fixture(t);
  assert.equal(f.listeners.size, 0);
  f.emit('pointerup');
  assert.equal(f.calls.length, 0);
  begin(f.content);
  begin(f.content);
  assert.equal(f.listeners.size, 2);
  assert.equal(document.body.style.userSelect, 'none');
  assert.equal(f.content.style.userSelect, 'text');
  f.emit('pointerup');
  f.assertRestored();
  assert.equal(f.calls.length, 0);
  f.flush();
  assert.deepEqual(f.calls, [['HandleDocumentPointerUp', true]]);
});

test('pointer cancellation restores both independent body selection styles', t => {
  const f = fixture(t);
  begin(f.content);
  f.emit('pointercancel');
  f.assertRestored();
  f.flush();
  assert.equal(f.calls.length, 1);
});

test('unregister during selection restores styles without notifying a disposed receiver', t => {
  const f = fixture(t);
  begin(f.content);
  unregister(f.content);
  f.assertRestored();
  f.emit('pointerup');
  f.flush();
  assert.equal(f.calls.length, 0);
});

test('unregister cancels queued release and guards callbacks already dequeued', t => {
  const f = fixture(t);
  begin(f.content);
  f.emit('pointerup');
  const staleCallback = [...f.timers.values()][0];
  unregister(f.content);
  assert.equal(f.timers.size, 0);
  staleCallback();
  assert.equal(f.calls.length, 0);
});

test('starting a new selection cancels the previous deferred release', t => {
  const f = fixture(t);
  begin(f.content);
  f.emit('pointerup');
  begin(f.content);
  assert.equal(f.timers.size, 0);
  f.flush();
  assert.equal(f.calls.length, 0);
  f.emit('pointerup');
  f.flush();
  f.assertRestored();
  assert.equal(f.calls.length, 1);
});

test('reregistering invalidates the old release callback', t => {
  const f = fixture(t);
  begin(f.content);
  f.emit('pointerup');
  const staleCallback = [...f.timers.values()][0];
  register(f.content, f.receiver);
  staleCallback();
  assert.equal(f.calls.length, 0);
  begin(f.content);
  f.emit('pointerup');
  f.flush();
  assert.equal(f.calls.length, 1);
});
