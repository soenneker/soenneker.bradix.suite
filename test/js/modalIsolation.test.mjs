import { test } from 'node:test';
import assert from 'node:assert/strict';
import { registerHideOthers, unregisterHideOthers } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/presenceOverlay.js';

class Element {
  get ownerDocument() { return globalThis.document; }
  children = [];
  parentElement = null;
  attributes = new Map();
  inert = false;
  get isConnected() { return document.body.contains(this); }
  append(child) { this.children.push(child); child.parentElement = this; return child; }
  contains(child) { return child === this || this.children.some(element => element.contains(child)); }
  getAttribute(name) { return this.attributes.get(name) ?? null; }
  setAttribute(name, value) { this.attributes.set(name, value); }
  removeAttribute(name) { this.attributes.delete(name); }
  querySelectorAll() {
    const result = [];
    for (const child of this.children) {
      if (child.attributes.has('aria-live')) result.push(child);
      result.push(...child.querySelectorAll());
    }
    return result;
  }
}

test('inline modal hides sibling subtrees, including triggers inside dismissable branches', () => {
  const body = new Element();
  globalThis.document = { body };
  const app = body.append(new Element());
  const trigger = app.append(new Element());
  trigger.setAttribute('data-bradix-dismissable-layer-branch', '');
  const modal = app.append(new Element());
  registerHideOthers(modal);
  assert.equal(app.inert, false);
  assert.equal(modal.inert, false);
  assert.equal(trigger.inert, true);
  assert.equal(trigger.getAttribute('aria-hidden'), 'true');
  unregisterHideOthers(modal);
  assert.equal(trigger.inert, false);
  assert.equal(trigger.getAttribute('aria-hidden'), null);
});

test('overlapping modal registrations restore backgrounds only after the last owner leaves', () => {
  const body = new Element();
  globalThis.document = { body };
  const background = body.append(new Element());
  background.setAttribute('aria-hidden', 'false');
  const first = body.append(new Element());
  registerHideOthers(first);
  const second = body.append(new Element());
  registerHideOthers(second);
  unregisterHideOthers(first);
  assert.equal(background.inert, true);
  assert.equal(background.getAttribute('aria-hidden'), 'true');
  unregisterHideOthers(second);
  assert.equal(background.inert, false);
  assert.equal(background.getAttribute('aria-hidden'), 'false');
});

test('live announcements remain exposed while siblings and preexisting inert state are preserved', () => {
  const body = new Element();
  globalThis.document = { body };
  const region = body.append(new Element());
  const live = region.append(new Element());
  live.setAttribute('aria-live', 'polite');
  const sibling = region.append(new Element());
  sibling.inert = true;
  const modal = body.append(new Element());
  registerHideOthers(modal);
  assert.equal(region.inert, false);
  assert.equal(live.inert, false);
  assert.equal(sibling.getAttribute('aria-hidden'), 'true');
  unregisterHideOthers(modal);
  assert.equal(sibling.inert, true);
});
