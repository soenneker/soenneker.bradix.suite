const collapsibleObservers = new WeakMap();

function updateCollapsibleSize(element) {
  if (!element || element.hidden) {
    return;
  }

  const height = element.scrollHeight;
  const width = element.scrollWidth;

  if (height > 0) {
    const value = `${height}px`;
    if (element.style.getPropertyValue("--radix-collapsible-content-height") !== value) {
      element.style.setProperty("--radix-collapsible-content-height", value);
    }
  }

  if (width > 0) {
    const value = `${width}px`;
    if (element.style.getPropertyValue("--radix-collapsible-content-width") !== value) {
      element.style.setProperty("--radix-collapsible-content-width", value);
    }
  }
}

export function observeCollapsibleContent(element) {
  if (!element) {
    return;
  }

  unobserveCollapsibleContent(element);
  updateCollapsibleSize(element);

  let frame = 0;
  let disposed = false;
  const update = () => {
    frame = 0;
    if (!disposed) updateCollapsibleSize(element);
  };
  const scheduleFrame = () => {
    if (!disposed && !frame) frame = requestAnimationFrame(update);
  };

  const resizeObserver = new ResizeObserver(scheduleFrame);
  resizeObserver.observe(element);

  const mutationObserver = new MutationObserver(scheduleFrame);
  mutationObserver.observe(element, {
    childList: true,
    subtree: true,
    characterData: true,
  });

  scheduleFrame();

  const timeoutId = setTimeout(scheduleFrame, 50);

  collapsibleObservers.set(element, {
    dispose() {
      disposed = true;
      resizeObserver.disconnect();
      mutationObserver.disconnect();
      if (frame) cancelAnimationFrame(frame);
      clearTimeout(timeoutId);
    },
  });
}

export function unobserveCollapsibleContent(element) {
  const observer = collapsibleObservers.get(element);

  if (!observer) {
    return;
  }

  observer.dispose();
  collapsibleObservers.delete(element);
}
