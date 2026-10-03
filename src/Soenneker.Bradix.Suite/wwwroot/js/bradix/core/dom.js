const booleanAttributeNames = {
  __proto__: null,
  bradixPreventEnter: "data-bradix-prevent-enter",
  bradixSpaceClick: "data-bradix-space-click",
  bradixRovingClickOnFocus: "data-bradix-roving-click-on-focus",
  bradixPreventNonprimaryMousedown: "data-bradix-prevent-nonprimary-mousedown",
  bradixPreventMousedownWhenDisabled: "data-bradix-prevent-mousedown-when-disabled",
  bradixRovingLoop: "data-bradix-roving-loop"
};

export function getAncestorIds(element) {
  const ids = [];
  let current = element;

  while (current instanceof HTMLElement) {
    if (current.id) {
      ids.push(current.id);
    }

    current = current.parentElement;
  }

  return ids;
}

export function readBooleanDataAttribute(element, name) {
  const value = element.getAttribute(booleanAttributeNames[name] ?? `data-${toKebabCase(name)}`);
  return value !== null && value !== "false";
}

export function toKebabCase(value) {
  return value.replace(/[A-Z]/g, (match) => `-${match.toLowerCase()}`);
}

export function cssEscape(value) {
  if (typeof CSS !== "undefined" && typeof CSS.escape === "function") {
    return CSS.escape(value);
  }

  return String(value).replace(/["\\]/g, "\\$&");
}

export function getTextContent(element) {
  if (!element) {
    return "";
  }

  return (element.textContent || "").trim();
}

const textObservers = new WeakMap();

export function observeTextContent(element, receiver) {
  unobserveTextContent(element);
  let text = getTextContent(element);
  if (!element) return text;

  const observer = new MutationObserver(() => {
    const nextText = getTextContent(element);
    if (nextText === text) return;
    text = nextText;
    // The element or circuit may be disposed while a notification is in flight.
    receiver.invokeMethodAsync("OnTextContentChanged", text).catch(() => {});
  });
  observer.observe(element, { childList: true, subtree: true, characterData: true });
  textObservers.set(element, observer);
  return text;
}

export function unobserveTextContent(element) {
  textObservers.get(element)?.disconnect();
  textObservers.delete(element);
}

export function getTextContentExcluding(element, excludeSelector) {
  if (!element) {
    return "";
  }

  if (!excludeSelector) return getTextContent(element);

  // Complex selectors can depend on ancestors or DOM state. Preserve the detached
  // clone semantics for those; tooltip exclusion uses a simple attribute selector.
  if (!/^(?:\.[\w-]+|#[\w-]+|\[[\w-]+(?:=(?:"[^"\\]*"|'[^'\\]*'|[\w-]+))?\])$/.test(excludeSelector)) {
    const clone = element.cloneNode(true);
    for (const node of clone.querySelectorAll(excludeSelector)) node.remove();
    return (clone.textContent || "").trim();
  }

  const excluded = new Set(element.querySelectorAll(excludeSelector));
  if (excluded.size === 0) return getTextContent(element);

  // Reject entire excluded subtrees without cloning controls, IDs, or custom elements.
  const walker = element.ownerDocument.createTreeWalker(
    element,
    NodeFilter.SHOW_ELEMENT | NodeFilter.SHOW_TEXT | NodeFilter.SHOW_CDATA_SECTION,
    {
      acceptNode(node) {
        if (node.nodeType === Node.ELEMENT_NODE)
          return excluded.has(node) ? NodeFilter.FILTER_REJECT : NodeFilter.FILTER_SKIP;
        return NodeFilter.FILTER_ACCEPT;
      }
    });
  const parts = [];
  while (walker.nextNode()) parts.push(walker.currentNode.nodeValue);
  return parts.join("").trim();
}
