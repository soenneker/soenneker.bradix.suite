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
  const value = element.getAttribute(`data-${toKebabCase(name)}`);
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

  const clone = element.cloneNode(true);

  if (excludeSelector) {
    for (const excluded of clone.querySelectorAll(excludeSelector)) {
      excluded.remove();
    }
  }

  return (clone.textContent || "").trim();
}
