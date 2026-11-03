import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerSelectContentKeyboard, unregisterSelectContentKeyboard } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/select.js';

for (const backward of [false, true]) {
  test(`Tab commits the browser's current option (backward=${backward})`, () => {
    globalThis.HTMLElement = class {};
    globalThis.MutationObserver = class { observe() {} disconnect() {} };
    const option = new HTMLElement();
    option.closest = () => option;
    option.getAttribute = name => name === 'data-value' ? 'banana' : null;
    globalThis.document = { activeElement: option };
    const listeners = new Map(), calls = [];
    const content = {
      contains: element => element === option,
      querySelectorAll: () => [option],
      addEventListener: (name, handler) => listeners.set(name, handler),
      removeEventListener: name => listeners.delete(name)
    };
    registerSelectContentKeyboard(content, { invokeMethodAsync: (...args) => { calls.push(args); return Promise.resolve(); } });
    const event = { key: 'Tab', shiftKey: backward, preventDefault() { this.prevented = true; }, stopPropagation() {}, stopImmediatePropagation() {} };
    listeners.get('keydown')(event);
    assert.equal(event.prevented, true);
    assert.deepEqual(calls, [['HandleContentTab', 'banana', backward]]);
    unregisterSelectContentKeyboard(content);
    assert.equal(listeners.size, 0);
  });
}
