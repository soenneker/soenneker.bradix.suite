(function () {
  if (globalThis.FloatingUIDOM && globalThis.FloatingUICore) {
    return;
  }

  const state = globalThis.__bradixFloatingUiGlobalLoader || {
    promise: null
  };

  globalThis.__bradixFloatingUiGlobalLoader = state;

  state.promise ||= loadFloatingUi();

  async function loadFloatingUi() {
    await loadUmdGlobal("/_content/Soenneker.Bradix.Suite/js/vendor/floating-ui.core.umd.min.js");
    await loadUmdGlobal("/_content/Soenneker.Bradix.Suite/js/vendor/floating-ui.dom.umd.min.js");
  }

  async function loadUmdGlobal(url) {
    const response = await fetch(url, { cache: "force-cache" });

    if (!response.ok) {
      throw new Error(`Failed to load ${url}: ${response.status}`);
    }

    const source = await response.text();
    new Function("define", "exports", "module", source)(undefined, undefined, undefined);
  }
})();
