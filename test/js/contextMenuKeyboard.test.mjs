import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerContextMenuKeyboard, unregisterContextMenuKeyboard } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/controls.js';

function fixture() {
  const events = [], listeners = new Map();
  globalThis.MouseEvent = class { constructor(type, options) { this.type = type; Object.assign(this, options); } };
  const element = {
    disabled: false,
    hasAttribute() { return this.disabled; },
    addEventListener(type, handler) { listeners.set(type, handler); },
    removeEventListener(type) { listeners.delete(type); },
    getBoundingClientRect() { return { left: 40, bottom: 120 }; },
    dispatchEvent(event) { events.push(event); }
  };
  registerContextMenuKeyboard(element);
  return { element, events, listeners, key(key, shiftKey = false) {
    const event = { key, shiftKey, defaultPrevented: false, preventDefault() { this.defaultPrevented = true; } };
    listeners.get('keydown')?.(event);
    return event;
  } };
}

test('keyboard context menu opens at the target with native contextmenu semantics', () => {
  const f = fixture();
  assert.equal(f.key('F10', true).defaultPrevented, true);
  assert.equal(f.key('ContextMenu').defaultPrevented, true);
  assert.equal(f.events.length, 2);
  assert.deepEqual([f.events[0].type, f.events[0].clientX, f.events[0].clientY, f.events[0].button], ['contextmenu', 40, 120, 2]);
});

test('ordinary keys and disabled triggers retain native behavior', () => {
  const f = fixture();
  assert.equal(f.key('Tab').defaultPrevented, false);
  assert.equal(f.key('F10').defaultPrevented, false);
  f.element.disabled = true;
  assert.equal(f.key('ContextMenu').defaultPrevented, false);
  assert.equal(f.events.length, 0);
});

test('unregister removes keyboard handling', () => {
  const f = fixture();
  unregisterContextMenuKeyboard(f.element);
  f.key('ContextMenu');
  assert.equal(f.events.length, 0);
  assert.equal(f.listeners.size, 0);
});
