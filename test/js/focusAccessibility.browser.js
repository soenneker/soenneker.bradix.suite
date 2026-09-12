// Run in a browser after loading core/focus.js into the same evaluation scope.
document.body.innerHTML = `
  <div id="scope" tabindex="-1">
    <div inert><button id="inert-button">Inert</button></div>
    <div hidden><button id="hidden-button">Hidden</button></div>
    <fieldset disabled>
      <legend><button id="legend-button">Legend exception</button></legend>
      <button id="disabled-button">Disabled by fieldset</button>
    </fieldset>
    <button id="last-button">Last</button>
    <div inert><button id="inert-last">Inert last</button></div>
  </div>`;
const scope = document.getElementById('scope');
const candidates = getTabbableCandidates(scope).map(element => element.id);
if (JSON.stringify(candidates) !== JSON.stringify(['legend-button', 'last-button'])) {
  throw new Error(`Unexpected focus candidates: ${candidates}`);
}
const edges = getTabbableEdges(scope).map(element => element.id);
if (JSON.stringify(edges) !== JSON.stringify(['legend-button', 'last-button'])) {
  throw new Error(`Unexpected focus edges: ${edges}`);
}
focusFirst(getTabbableCandidates(scope));
if (document.activeElement.id !== 'legend-button') {
  throw new Error('Initial focus missed the first enabled control');
}
JSON.stringify({ candidates, edges, focused: document.activeElement.id });
