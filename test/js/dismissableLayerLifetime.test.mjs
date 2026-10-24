import { test } from 'node:test';
import assert from 'node:assert/strict';

async function fixture() {
  class Element {
    style = {};
    dataset = {};
    id = '';
    parentElement = null;
    listeners = new Map();
    addEventListener(type, callback, capture = false) {
      if (!this.listeners.has(type)) this.listeners.set(type, []);
      this.listeners.get(type).push({ callback, capture });
    }
    removeEventListener(type, callback) {
      const remaining = (this.listeners.get(type) ?? []).filter(entry => entry.callback !== callback);
      if (remaining.length) this.listeners.set(type, remaining); else this.listeners.delete(type);
    }
    contains(target) { return target === this; }
  }
  globalThis.HTMLElement = Element;
  globalThis.document = new Element();
  document.body = new Element();
  document.activeElement = null;
  const layer = new Element();
  const outside = new Element();
  const module = await import(`../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/dismissableLayer.js?test=${crypto.randomUUID()}`);
  return { module, layer, outside,
    emit(type) { const event = { target: outside, key: 'Escape', preventDefault() { this.defaultPrevented = true; } }; for (const entry of [...(document.listeners.get(type) ?? [])].sort((a,b) => Number(b.capture) - Number(a.capture))) entry.callback(event); }
  };
}

for (const [event, method] of [
  ['pointerdown', 'HandlePointerDownOutside'],
  ['focusin', 'HandleFocusOutside'],
  ['keydown', 'HandleEscapeKeyDown']
]) {
  test(`unregister keeps the receiver alive until pending ${event} callbacks finish`, async () => {
    const f = await fixture();
    let deliver;
    let disposed = false;
    const calls = [];
    f.module.registerDismissableLayer(f.layer, {
      invokeMethodAsync(name) {
        calls.push(name);
        return new Promise(resolve => {
          deliver = () => { assert.equal(disposed, false); resolve(false); };
        });
      }
    }, true);
    f.emit(event);
  await Promise.resolve();
    assert.deepEqual(calls, [method]);
    const cleanup = Promise.resolve(f.module.unregisterDismissableLayer(f.layer)).then(() => { disposed = true; });
    await Promise.resolve();
    assert.equal(disposed, false);
    assert.equal(document.body.style.pointerEvents, '');
    f.emit(event);
  await Promise.resolve();
    assert.deepEqual(calls, [method]);
    deliver();
    await cleanup;
    assert.equal(disposed, true);
  });
}

test('repeated unregister awaits the same in-flight callback and handles rejection', async () => {
  const f = await fixture();
  let reject;
  f.module.registerDismissableLayer(f.layer, {
    invokeMethodAsync() { return new Promise((_, fail) => { reject = fail; }); }
  }, false);
  f.emit('pointerdown');
  await Promise.resolve();
  const first = f.module.unregisterDismissableLayer(f.layer);
  assert.equal(f.module.unregisterDismissableLayer(f.layer), first);
  reject(new Error('Disconnected'));
  await first;
  assert.equal(f.module.unregisterDismissableLayer(f.layer), undefined);
});

test('re-registering an element preserves callbacks from its previous registration', async () => {
  const f = await fixture();
  let finish;
  f.module.registerDismissableLayer(f.layer, {
    invokeMethodAsync() { return new Promise(resolve => { finish = resolve; }); }
  }, false);
  f.emit('pointerdown');
  await Promise.resolve();
  f.module.registerDismissableLayer(f.layer, { invokeMethodAsync() { return Promise.resolve(); } }, false);
  let completed = false;
  const cleanup = f.module.unregisterDismissableLayer(f.layer).then(() => { completed = true; });
  await Promise.resolve();
  assert.equal(completed, false);
  finish();
  await cleanup;
  assert.equal(completed, true);
});

test('cleanup still finds the registration after Blazor removes its DOM element', async () => {
  const f = await fixture();
  let finish;
  f.module.registerDismissableLayer(f.layer, {
    invokeMethodAsync() { return new Promise(resolve => { finish = resolve; }); }
  }, true, 'layer-id');
  f.emit('pointerdown');
  await Promise.resolve();
  const cleanup = f.module.unregisterDismissableLayer(null, 'layer-id');
  assert.equal(f.layer.listeners.size, 0);
  assert.equal(document.body.style.pointerEvents, '');
  assert.ok(cleanup instanceof Promise);
  finish();
  await cleanup;
});

for (const event of ['pointerdown', 'focusin']) {
  test(`portaled anchors suppress ${event} dismissal only for their owning popover`, async () => {
    const f = await fixture();
    const calls = [];
    f.layer.dataset.bradixDismissableLayerOwner = 'popover-one';
    f.outside.dataset.bradixDismissableLayerOwner = 'popover-one';
    f.module.registerDismissableLayerBranch(f.outside);
    f.module.registerDismissableLayer(f.layer, {
      invokeMethodAsync(name) { calls.push(name); return Promise.resolve(); }
    }, false);
    f.emit(event);
    await Promise.resolve();
    assert.deepEqual(calls, []);
    f.outside.dataset.bradixDismissableLayerOwner = 'popover-two';
    f.emit(event);
    await Promise.resolve();
    assert.equal(calls.length, 1);
    await f.module.unregisterDismissableLayer(f.layer);
    f.module.unregisterDismissableLayerBranch(f.outside);
  });
}
