import { test } from 'node:test';
import assert from 'node:assert/strict';
import { syncCheckboxBubbleInputState } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/controls.js';

class Control {
  constructor(checked, role = 'radio') { this.checked = checked; this.role = role; }
  getAttribute(name) { return name === 'role' ? this.role : name === 'aria-checked' ? String(this.checked) : null; }
}
class Input {
  type = 'radio';
  events = [];
  set checked(value) { this.current = value; }
  get checked() { return this.current; }
  dispatchEvent(event) { this.events.push(event); }
}

test('delayed control synchronization uses the current rendered selection and suppresses stale events', () => {
  globalThis.HTMLElement = Control;
  globalThis.window = { HTMLInputElement: Input };
  for (const role of ['radio', 'switch', 'checkbox']) for (const selected of [false, true]) {
    const input = new Input();
    input.previousElementSibling = new Control(selected, role);
    syncCheckboxBubbleInputState(input, !selected, false, true);
    assert.equal(input.checked, selected);
    assert.equal(input.events.length, 0);
  }
});

test('current radio synchronization preserves change event dispatch', () => {
  globalThis.HTMLElement = Control;
  globalThis.window = { HTMLInputElement: Input };
  const input = new Input();
  input.previousElementSibling = new Control(true);
  syncCheckboxBubbleInputState(input, true, false, true);
  assert.equal(input.checked, true);
  assert.equal(input.events.length, 1);
});
