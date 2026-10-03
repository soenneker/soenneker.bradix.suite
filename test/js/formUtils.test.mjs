import { test } from 'node:test';
import assert from 'node:assert/strict';
import { serializeFormDataSnapshot } from '../../src/Soenneker.Bradix.Suite/wwwroot/js/bradix/core/formUtils.js';

test('form snapshots preserve repeated fields and names inherited by ordinary objects', t => {
  const original = globalThis.FormData;
  t.after(() => { globalThis.FormData = original; });
  globalThis.FormData = class {
    *entries() {
      yield ['__proto__', 'first'];
      yield ['constructor', 'second'];
      yield ['toString', 'third'];
      yield ['__proto__', 'fourth'];
      yield ['attachment', {}];
    }
  };
  const snapshot = serializeFormDataSnapshot({});
  assert.deepEqual(JSON.parse(JSON.stringify(snapshot)), {
    values: { ['__proto__']: ['first', 'fourth'], constructor: ['second'], toString: ['third'], attachment: [''] }
  });
});
