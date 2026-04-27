(function () {
  if (globalThis.FloatingUICore) {
    return;
  }

  const scriptUrl = document.currentScript && document.currentScript.src;
  const sourceUrl = scriptUrl
    ? scriptUrl.replace(/floating-ui\.core\.global-loader\.js(?:\?.*)?$/, "floating-ui.core.umd.min.js")
    : "./floating-ui.core.umd.min.js";

  fetch(sourceUrl)
    .then((response) => {
      if (!response.ok) {
        throw new Error(`Failed to load Floating UI Core: ${response.status}`);
      }

      return response.text();
    })
    .then((code) => {
      const previousDefine = globalThis.define;

      try {
        globalThis.define = undefined;
        (0, eval)(code);
      } finally {
        globalThis.define = previousDefine;
      }
    });
})();
