(function () {
  const guard = globalThis.__bradixFloatingUiAmdGuard;

  if (!guard) {
    return;
  }

  try {
    delete globalThis.define;
    delete globalThis.exports;
    delete globalThis.module;

    if (guard.hadDefine) {
      globalThis.define = guard.define;
    }

    if (guard.hadExports) {
      globalThis.exports = guard.exports;
    }

    if (guard.hadModule) {
      globalThis.module = guard.module;
    }
  } finally {
    delete globalThis.__bradixFloatingUiAmdGuard;
  }
})();
