(function () {
  if (globalThis.__bradixFloatingUiAmdGuard) {
    return;
  }

  globalThis.__bradixFloatingUiAmdGuard = {
    hadDefine: Object.prototype.hasOwnProperty.call(globalThis, "define"),
    define: globalThis.define,
    defineStub: function bradixSuppressedDefine() {
    },
    defineAmd: undefined,
    hadExports: Object.prototype.hasOwnProperty.call(globalThis, "exports"),
    exports: globalThis.exports,
    hadModule: Object.prototype.hasOwnProperty.call(globalThis, "module"),
    module: globalThis.module
  };

  try {
    Object.defineProperty(globalThis.__bradixFloatingUiAmdGuard.defineStub, "amd", {
      configurable: true,
      get() {
        return undefined;
      },
      set(value) {
        globalThis.__bradixFloatingUiAmdGuard.defineAmd = value;
      }
    });

    Object.defineProperty(globalThis, "define", {
      configurable: true,
      get() {
        return globalThis.__bradixFloatingUiAmdGuard.defineStub;
      },
      set(value) {
        globalThis.__bradixFloatingUiAmdGuard.define = value;
        globalThis.__bradixFloatingUiAmdGuard.hadDefine = true;
      }
    });
    Object.defineProperty(globalThis, "exports", {
      configurable: true,
      get() {
        return undefined;
      },
      set(value) {
        globalThis.__bradixFloatingUiAmdGuard.exports = value;
        globalThis.__bradixFloatingUiAmdGuard.hadExports = true;
      }
    });
    Object.defineProperty(globalThis, "module", {
      configurable: true,
      get() {
        return undefined;
      },
      set(value) {
        globalThis.__bradixFloatingUiAmdGuard.module = value;
        globalThis.__bradixFloatingUiAmdGuard.hadModule = true;
      }
    });
  } catch {
  }
})();
