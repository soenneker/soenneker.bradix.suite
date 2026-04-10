const collapsibleObservers = new WeakMap();

function updateCollapsibleSize(element) {
  if (!element || element.hidden) {
    return;
  }

  const rect = element.getBoundingClientRect();

  if (rect.height > 0) {
    element.style.setProperty("--radix-collapsible-content-height", `${rect.height}px`);
  }

  if (rect.width > 0) {
    element.style.setProperty("--radix-collapsible-content-width", `${rect.width}px`);
  }
}

export function observeCollapsibleContent(element) {
  if (!element) {
    return;
  }

  unobserveCollapsibleContent(element);

  const observer = new ResizeObserver(() => updateCollapsibleSize(element));
  observer.observe(element);

  collapsibleObservers.set(element, observer);

  requestAnimationFrame(() => updateCollapsibleSize(element));
}

export function unobserveCollapsibleContent(element) {
  const observer = collapsibleObservers.get(element);

  if (!observer) {
    return;
  }

  observer.disconnect();
  collapsibleObservers.delete(element);
}
