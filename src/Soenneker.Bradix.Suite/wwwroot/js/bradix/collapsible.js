const collapsibleObservers = new WeakMap();

function updateCollapsibleSize(element) {
  if (!element || element.hidden || element.getClientRects().length === 0) {
    return;
  }

  const height = element.scrollHeight;
  const width = element.scrollWidth;

  // Empty content needs its own zero value instead of inheriting a parent's
  // dimensions. Only unrendered content retains its previous measurements.
  const heightValue = `${height}px`;
  if (element.style.getPropertyValue("--radix-collapsible-content-height") !== heightValue) {
    element.style.setProperty("--radix-collapsible-content-height", heightValue);
  }

  const widthValue = `${width}px`;
  if (element.style.getPropertyValue("--radix-collapsible-content-width") !== widthValue) {
    element.style.setProperty("--radix-collapsible-content-width", widthValue);
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
