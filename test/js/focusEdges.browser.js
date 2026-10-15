// Run in a browser after loading core/focus.js into the same evaluation scope.
(() => {
  const scope = document.createElement("div");
  scope.tabIndex = 0;
  document.body.append(scope);
  const verify = (html, expected) => {
    scope.innerHTML = html;
    const edges = getTabbableEdges(scope).map(element => element?.id ?? null);
    if (JSON.stringify(edges) !== JSON.stringify(expected))
      throw new Error(`Unexpected edges ${JSON.stringify(edges)} for ${html}`);
  };
  verify("", [null, null]);
  verify('<div tabindex="0" id="parent"><span><button id="child">Child</button></span></div>', ["parent", "child"]);
  verify('<button id="only">Only</button><div hidden><button>Hidden</button></div>', ["only", "only"]);
  verify('<div style="display:none"><button>Hidden</button></div><button id="visible">Visible</button><button style="visibility:hidden">Hidden</button>', ["visible", "visible"]);
  verify('<div inert><button>Inert</button></div><fieldset disabled><legend><button id="legend">Legend</button></legend><button>Disabled</button></fieldset><input type="hidden"><button id="last">Last</button>', ["legend", "last"]);
  verify('<div style="display:none"><button>Hidden</button></div>', [null, null]);

  // Compare the direct edge traversal with the original complete candidate scan
  // over nested, disabled, hidden and focusable parent combinations.
  let randomState = 481;
  const random = () => ((randomState = (Math.imul(randomState, 1664525) + 1013904223) >>> 0) / 4294967296);
  const originalEdges = () => {
    const candidates = getTabbableCandidates(scope);
    const visible = candidates.filter(element => !isHidden(element, scope));
    return [visible[0] ?? null, visible.at(-1) ?? null];
  };
  for (let iteration = 0; iteration < 300; iteration++) {
    scope.replaceChildren();
    const parents = [scope];
    for (let index = 0; index < 80; index++) {
      const node = document.createElement(random() < 0.45 ? "button" : "div");
      node.id = `node-${index}`;
      if (random() < 0.2) node.tabIndex = 0;
      if (random() < 0.1) node.hidden = true;
      if (random() < 0.1) node.inert = true;
      if (random() < 0.1) node.style.display = "none";
      if (random() < 0.1) node.style.visibility = "hidden";
      if (random() < 0.15) node.disabled = true;
      parents[Math.floor(random() * parents.length)].append(node);
      if (node.tagName === "DIV") parents.push(node);
    }
    const actual = getTabbableEdges(scope);
    const expected = originalEdges();
    if (actual[0] !== expected[0] || actual[1] !== expected[1])
      throw new Error(`Edge traversal differs on generated tree ${iteration}`);
  }
  scope.remove();
  return { cases: 306, passed: true };
})();
