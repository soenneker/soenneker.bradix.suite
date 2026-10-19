import assert from "node:assert/strict";
import { test } from "node:test";

let moduleVersion = 0;
async function createFixture() {
  const listeners = new Map();
  const added = [];
  const removed = [];
  globalThis.Node = Object;
  globalThis.HTMLElement = Object;
  globalThis.document = {
    addEventListener(type, handler, capture) {
      assert.equal(listeners.has(type), false, `Duplicate ${type} listener`);
      listeners.set(type, handler);
      added.push([type, capture]);
    },
    removeEventListener(type, handler, capture) {
      assert.equal(listeners.get(type), handler);
      listeners.delete(type);
      removed.push([type, capture]);
    }
  };
  const module = await import(`../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/delegatedInteraction.js?test=${++moduleVersion}`);
  const calls = [];
  const receiver = { invokeMethodAsync(...args) { calls.push(args); return Promise.resolve(); } };
  return { ...module, listeners, added, removed, calls, receiver };
}

test("document listeners exist only for requested events and survive shared registrations", async () => {
  const fixture = await createFixture();
  const first = { removeEventListener() {} };
  const second = { removeEventListener() {} };
  fixture.registerDelegatedInteraction(first, fixture.receiver, { click: { method: "Click" } });
  assert.deepEqual(fixture.added, [["click", true]]);
  fixture.registerDelegatedInteraction(second, fixture.receiver, {
    click: { method: "Click" }, keydown: { method: "Key" }, focusout: { method: "Blur" }
  });
  assert.deepEqual([...fixture.listeners.keys()], ["click", "keydown", "focusout"]);
  fixture.unregisterDelegatedInteraction(first);
  assert.equal(fixture.removed.length, 0);
  fixture.unregisterDelegatedInteraction(second);
  fixture.unregisterDelegatedInteraction(second);
  assert.equal(fixture.listeners.size, 0);
  assert.deepEqual(fixture.removed, [["click", true], ["keydown", true], ["focusout", false]]);
});

test("reregistering mutated options updates counts without duplicate handlers", async () => {
  const fixture = await createFixture();
  const element = { removeEventListener() {} };
  const options = { click: { method: "Click" } };
  fixture.registerDelegatedInteraction(element, fixture.receiver, options);
  fixture.registerDelegatedInteraction(element, fixture.receiver, options);
  assert.equal(fixture.added.length, 1);
  delete options.click;
  options.pointermove = { method: "Move" };
  fixture.registerDelegatedInteraction(element, fixture.receiver, options);
  assert.deepEqual([...fixture.listeners.keys()], ["pointermove"]);
  fixture.unregisterDelegatedInteraction(element);
  assert.equal(fixture.listeners.size, 0);
});

test("event filtering, preventDefault, bubbling and readiness retain their behavior", async () => {
  const fixture = await createFixture();
  const parent = { matches() { return false; }, removeEventListener() {}, parentNode: null };
  const child = { matches() { return false; }, removeEventListener() {}, parentNode: parent };
  fixture.registerDelegatedInteraction(parent, fixture.receiver, {
    readyMethod: "Ready",
    click: { method: "Parent", currentTargetOnly: false, checkForDefaultPrevented: false }
  });
  fixture.registerDelegatedInteraction(child, fixture.receiver, {
    click: { method: "Child", preventDefault: true }
  });
  const event = { target: child, defaultPrevented: false, preventDefault() { this.defaultPrevented = true; } };
  fixture.listeners.get("click")(event);
  assert.deepEqual(fixture.calls.map(call => call[0]), ["Ready", "Child", "Parent"]);
  assert.equal(fixture.calls[1][1].defaultPrevented, true);
  fixture.listeners.get("click")(event);
  assert.deepEqual(fixture.calls.map(call => call[0]), ["Ready", "Child", "Parent", "Parent"]);
  fixture.unregisterDelegatedInteraction(child);
  fixture.unregisterDelegatedInteraction(parent);
  assert.equal(fixture.listeners.size, 0);
});

test("registrations with no events do not install document handlers", async () => {
  const fixture = await createFixture();
  const element = { removeEventListener() {} };
  fixture.registerDelegatedInteraction(element, fixture.receiver, { readyMethod: "Ready" });
  assert.equal(fixture.listeners.size, 0);
  assert.deepEqual(fixture.calls, [["Ready"]]);
  fixture.unregisterDelegatedInteraction(element);
  fixture.registerDelegatedInteraction(element, fixture.receiver, { keydown: {} });
  assert.deepEqual([...fixture.listeners.keys()], ["keydown"]);
  fixture.unregisterDelegatedInteraction(element);
  assert.equal(fixture.listeners.size, 0);
});
