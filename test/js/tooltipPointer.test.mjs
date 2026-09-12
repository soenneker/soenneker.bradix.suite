import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerTooltipTrigger, unregisterTooltipTrigger } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/tooltip.js';

function target() {
  const listeners = new Map();
  return { listeners,
    addEventListener(type, callback) {
      if (!listeners.has(type)) listeners.set(type, new Set());
      listeners.get(type).add(callback);
    },
    removeEventListener(type, callback) { listeners.get(type)?.delete(callback); },
    emit(type, pointerId = 1) { listeners.get(type)?.forEach(callback => callback({ pointerId })); }
  };
}

test('unrelated pointer releases do not notify every tooltip on the page', () => {
  globalThis.document = target();
  const triggers = Array.from({ length: 100 }, target);
  const calls = [];
  const receiver = { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } };
  try {
    triggers.forEach(trigger => registerTooltipTrigger(trigger, receiver));
    document.emit('pointerup');
    assert.equal(calls.length, 0);
    triggers[0].emit('pointerdown');
    assert.equal(document.listeners.get('pointerup').size, 1);
    document.emit('pointerup', 2);
    assert.equal(calls.length, 0);
    document.emit('pointerup');
    assert.deepEqual(calls, [['HandleDocumentPointerUp']]);
    assert.equal(document.listeners.get('pointerup').size, 0);
    assert.equal(document.listeners.get('pointercancel').size, 0);
  } finally {
    triggers.forEach(unregisterTooltipTrigger);
  }
});

test('tooltip pointer cancellation and disposal clear temporary document listeners', () => {
  globalThis.document = target();
  const trigger = target(), calls = [];
  const receiver = { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } };
  registerTooltipTrigger(trigger, receiver);
  trigger.emit('pointerdown');
  document.emit('pointercancel');
  assert.equal(calls.length, 1);
  trigger.emit('pointerdown');
  unregisterTooltipTrigger(trigger);
  document.emit('pointerup');
  assert.equal(calls.length, 1);
  assert.equal(document.listeners.get('pointerup').size, 0);
  assert.equal(document.listeners.get('pointercancel').size, 0);
});
