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

  const resizeObserver = new ResizeObserver(() => updateCollapsibleSize(element));
  resizeObserver.observe(element);

  const mutationObserver = new MutationObserver(() => updateCollapsibleSize(element));
  mutationObserver.observe(element, {
    childList: true,
    subtree: true,
    characterData: true,
  });

  const rafIds = [];
  const scheduleFrame = () => {
    const id = requestAnimationFrame(() => updateCollapsibleSize(element));
    rafIds.push(id);
  };

  scheduleFrame();
  scheduleFrame();

  const timeoutId = setTimeout(() => updateCollapsibleSize(element), 50);

  collapsibleObservers.set(element, {
    mutationObserver,
    rafIds,
    resizeObserver,
    timeoutId,
  });
}

export function unobserveCollapsibleContent(element) {
  const observer = collapsibleObservers.get(element);

  if (!observer) {
    return;
  }

  observer.resizeObserver?.disconnect();
  observer.mutationObserver?.disconnect();
  observer.rafIds?.forEach((id) => cancelAnimationFrame(id));

  if (observer.timeoutId) {
    clearTimeout(observer.timeoutId);
  }

  collapsibleObservers.delete(element);
}
