const collapsibleObservers = new WeakMap();

function updateCollapsibleSize(element) {
  if (!element || element.hidden) {
    return;
  }

  const height = element.scrollHeight;
  const width = element.scrollWidth;

  if (height > 0) {
    element.style.setProperty("--radix-collapsible-content-height", `${height}px`);
  }

  if (width > 0) {
    element.style.setProperty("--radix-collapsible-content-width", `${width}px`);
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
