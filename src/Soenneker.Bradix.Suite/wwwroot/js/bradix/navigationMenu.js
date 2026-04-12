import { getTabbableCandidates, focusFirst, focusElement } from "./core/focus.js";

const navigationMenuIndicatorHandlers = new WeakMap();
const navigationMenuContentFocusBridgeHandlers = new WeakMap();
const navigationMenuViewportHandlers = new WeakMap();

export function registerNavigationMenuIndicator(indicator, activeTrigger, track, dotNetRef, orientation) {
  if (!indicator || !activeTrigger || !track || !dotNetRef) {
    return;
  }

  unregisterNavigationMenuIndicator(indicator);

  const notify = () => {
    const isHorizontal = orientation !== "vertical";
    const size = isHorizontal ? activeTrigger.offsetWidth : activeTrigger.offsetHeight;
    const offset = isHorizontal ? activeTrigger.offsetLeft : activeTrigger.offsetTop;
    dotNetRef.invokeMethodAsync("HandleIndicatorPositionChanged", size, offset);
  };

  const triggerResizeObserver = new ResizeObserver(notify);
  const trackResizeObserver = new ResizeObserver(notify);
  const handleWindowResize = () => notify();

  triggerResizeObserver.observe(activeTrigger);
  trackResizeObserver.observe(track);
  window.addEventListener("resize", handleWindowResize);

  notify();

  navigationMenuIndicatorHandlers.set(indicator, {
    notify,
    triggerResizeObserver,
    trackResizeObserver,
    handleWindowResize,
    dotNetRef
  });
}

export function updateNavigationMenuIndicator(indicator, activeTrigger, track, orientation) {
  const handlers = navigationMenuIndicatorHandlers.get(indicator);
  if (!handlers) {
    return;
  }

  unregisterNavigationMenuIndicator(indicator);
  registerNavigationMenuIndicator(indicator, activeTrigger, track, handlers.dotNetRef, orientation);
}

export function unregisterNavigationMenuIndicator(indicator) {
  const handlers = navigationMenuIndicatorHandlers.get(indicator);
  if (!handlers) {
    return;
  }

  handlers.triggerResizeObserver.disconnect();
  handlers.trackResizeObserver.disconnect();
  window.removeEventListener("resize", handlers.handleWindowResize);
  navigationMenuIndicatorHandlers.delete(indicator);
}

export function registerNavigationMenuContentFocusBridge(content, trigger, startProxy, endProxy) {
  if (!content || !trigger || !startProxy || !endProxy) {
    return;
  }

  unregisterNavigationMenuContentFocusBridge(content);

  const focusContent = (side) => {
    const candidates = getTabbableCandidates(content);
    if (!candidates.length) {
      return;
    }

    focusFirst(side === "start" ? candidates : [...candidates].reverse(), true);
  };

  const handleStartProxyFocus = (event) => {
    const previous = event.relatedTarget instanceof HTMLElement ? event.relatedTarget : null;
    const wasTriggerFocused = previous === trigger;
    const wasFocusFromContent = !!previous && content.contains(previous);

    if (wasTriggerFocused || !wasFocusFromContent) {
      focusContent(wasTriggerFocused ? "start" : "end");
    }
  };

  const handleEndProxyFocus = (event) => {
    const previous = event.relatedTarget instanceof HTMLElement ? event.relatedTarget : null;
    const wasFocusFromContent = !!previous && content.contains(previous);

    if (!wasFocusFromContent) {
      focusContent("end");
    }
  };

  const handleContentKeydown = (event) => {
    const isMetaKey = event.altKey || event.ctrlKey || event.metaKey;
    const isTabKey = event.key === "Tab" && !isMetaKey;
    if (!isTabKey) {
      return;
    }

    const candidates = getTabbableCandidates(content);
    const focusedElement = document.activeElement;
    const index = candidates.findIndex((candidate) => candidate === focusedElement);
    const isMovingBackwards = event.shiftKey;
    const nextCandidates = isMovingBackwards
      ? candidates.slice(0, index).reverse()
      : candidates.slice(index + 1);

    if (focusFirst(nextCandidates, true)) {
      event.preventDefault();
    } else {
      focusElement(isMovingBackwards ? startProxy : endProxy, false);
    }
  };

  startProxy.addEventListener("focus", handleStartProxyFocus);
  endProxy.addEventListener("focus", handleEndProxyFocus);
  content.addEventListener("keydown", handleContentKeydown);

  navigationMenuContentFocusBridgeHandlers.set(content, {
    trigger,
    startProxy,
    endProxy,
    handleStartProxyFocus,
    handleEndProxyFocus,
    handleContentKeydown
  });
}

export function updateNavigationMenuContentFocusBridge(content, trigger, startProxy, endProxy) {
  const handlers = navigationMenuContentFocusBridgeHandlers.get(content);
  if (!handlers) {
    return;
  }

  if (handlers.trigger === trigger && handlers.startProxy === startProxy && handlers.endProxy === endProxy) {
    return;
  }

  unregisterNavigationMenuContentFocusBridge(content);
  registerNavigationMenuContentFocusBridge(content, trigger, startProxy, endProxy);
}

export function unregisterNavigationMenuContentFocusBridge(content) {
  const handlers = navigationMenuContentFocusBridgeHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.startProxy.removeEventListener("focus", handlers.handleStartProxyFocus);
  handlers.endProxy.removeEventListener("focus", handlers.handleEndProxyFocus);
  content.removeEventListener("keydown", handlers.handleContentKeydown);
  navigationMenuContentFocusBridgeHandlers.delete(content);
}

export function registerNavigationMenuViewport(viewport, content, dotNetRef) {
  if (!viewport || !content || !dotNetRef) {
    return;
  }

  unregisterNavigationMenuViewport(viewport);

  const notify = () => {
    dotNetRef.invokeMethodAsync("HandleViewportSizeChanged", content.offsetWidth || 0, content.offsetHeight || 0);
  };

  const contentResizeObserver = new ResizeObserver(notify);
  contentResizeObserver.observe(content);
  notify();

  navigationMenuViewportHandlers.set(viewport, {
    content,
    dotNetRef,
    notify,
    contentResizeObserver
  });
}

export function updateNavigationMenuViewport(viewport, content) {
  const handlers = navigationMenuViewportHandlers.get(viewport);
  if (!handlers || !content) {
    return;
  }

  if (handlers.content === content) {
    handlers.notify();
    return;
  }

  handlers.contentResizeObserver.disconnect();
  handlers.content = content;
  handlers.contentResizeObserver.observe(content);
  handlers.notify();
}

export function unregisterNavigationMenuViewport(viewport) {
  const handlers = navigationMenuViewportHandlers.get(viewport);
  if (!handlers) {
    return;
  }

  handlers.contentResizeObserver.disconnect();
  navigationMenuViewportHandlers.delete(viewport);
}
