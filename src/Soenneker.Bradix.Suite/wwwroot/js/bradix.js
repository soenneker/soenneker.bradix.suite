const collapsibleObservers = new WeakMap();
const rovingFocusHandlers = new WeakMap();
const radioGroupItemHandlers = new WeakMap();
const checkboxRootHandlers = new WeakMap();
const sliderPointerHandlers = new WeakMap();
const scrollAreaRootHandlers = new WeakMap();
const scrollAreaViewportHandlers = new WeakMap();
const scrollAreaScrollbarHandlers = new WeakMap();
const portalMounts = new WeakMap();
const labelTextSelectionHandlers = new WeakMap();
const dismissableBranches = new Set();
const dismissableLayers = [];
let dismissableLayerListenersRegistered = false;
let originalDismissableBodyPointerEvents = "";
const focusScopeHandlers = new WeakMap();
const focusScopeStack = [];
const popperContentHandlers = new WeakMap();
const presenceHandlers = new WeakMap();
let focusGuardsCount = 0;
const hideOthersState = new WeakMap();
let removeScrollCount = 0;
let originalBodyOverflow = "";
let originalBodyPaddingRight = "";
const rovingFocusKeys = new Set([
  "ArrowLeft",
  "ArrowRight",
  "ArrowUp",
  "ArrowDown",
  "Home",
  "End",
  "PageUp",
  "PageDown"
]);

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

export function registerRovingFocusNavigationKeys(element) {
  if (!element) {
    return;
  }

  unregisterRovingFocusNavigationKeys(element);

  const handler = (event) => {
    if (rovingFocusKeys.has(event.key)) {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", handler);
  rovingFocusHandlers.set(element, handler);
}

export function unregisterRovingFocusNavigationKeys(element) {
  const handler = rovingFocusHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("keydown", handler);
  rovingFocusHandlers.delete(element);
}

export function registerRadioGroupItemKeys(element) {
  if (!element) {
    return;
  }

  unregisterRadioGroupItemKeys(element);

  const handler = (event) => {
    if (rovingFocusKeys.has(event.key) || event.key === "Enter" || event.key === " " || event.key === "Spacebar") {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", handler);
  radioGroupItemHandlers.set(element, handler);
}

export function unregisterRadioGroupItemKeys(element) {
  const handler = radioGroupItemHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("keydown", handler);
  radioGroupItemHandlers.delete(element);
}

export function registerCheckboxRoot(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterCheckboxRoot(element);

  const keydown = (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", keydown);

  const form = element.form || (typeof element.closest === "function" ? element.closest("form") : null);
  let reset = null;

  if (form && dotNetRef) {
    reset = () => {
      dotNetRef.invokeMethodAsync("HandleFormResetAsync");
    };

    form.addEventListener("reset", reset);
  }

  checkboxRootHandlers.set(element, { keydown, form, reset, dotNetRef });
}

export function unregisterCheckboxRoot(element) {
  const handlers = checkboxRootHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("keydown", handlers.keydown);

  if (handlers.form && handlers.reset) {
    handlers.form.removeEventListener("reset", handlers.reset);
  }

  checkboxRootHandlers.delete(element);
}

export function syncCheckboxBubbleInputState(element, isChecked, isIndeterminate, dispatchEvent) {
  if (!element) {
    return;
  }

  element.indeterminate = !!isIndeterminate;
  element.checked = !!isChecked;

  if (dispatchEvent) {
    element.dispatchEvent(new Event("change", { bubbles: true }));
  }
}

export function clickElement(element) {
  if (!element) {
    return;
  }

  element.click();
}

export function registerSliderPointerBridge(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterSliderPointerBridge(element);

  const getFractions = (event) => {
    const rect = element.getBoundingClientRect();
    const x = rect.width <= 0 ? 0 : (event.clientX - rect.left) / rect.width;
    const y = rect.height <= 0 ? 0 : (event.clientY - rect.top) / rect.height;

    return {
      x: Math.min(1, Math.max(0, x)),
      y: Math.min(1, Math.max(0, y))
    };
  };

  const pointerdown = (event) => {
    if (event.button !== 0) {
      return;
    }

    event.preventDefault();

    const thumb = event.target && typeof event.target.closest === "function"
      ? event.target.closest("[data-bradix-slider-thumb-index]")
      : null;
    const thumbIndex = thumb ? Number.parseInt(thumb.getAttribute("data-bradix-slider-thumb-index") || "-1", 10) : -1;
    const fractions = getFractions(event);

    dotNetRef.invokeMethodAsync("HandlePointerStartAsync", fractions.x, fractions.y, thumbIndex);

    const pointermove = (moveEvent) => {
      const moveFractions = getFractions(moveEvent);
      dotNetRef.invokeMethodAsync("HandlePointerMoveAsync", moveFractions.x, moveFractions.y);
    };

    const pointerup = () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
      dotNetRef.invokeMethodAsync("HandlePointerEndAsync");
    };

    document.addEventListener("pointermove", pointermove);
    document.addEventListener("pointerup", pointerup);

    const handlers = sliderPointerHandlers.get(element);
    if (handlers) {
      handlers.pointermove = pointermove;
      handlers.pointerup = pointerup;
    }
  };

  sliderPointerHandlers.set(element, { pointerdown, pointermove: null, pointerup: null });
  element.addEventListener("pointerdown", pointerdown);
}

export function unregisterSliderPointerBridge(element) {
  const handlers = sliderPointerHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("pointerdown", handlers.pointerdown);

  if (handlers.pointermove) {
    document.removeEventListener("pointermove", handlers.pointermove);
  }

  if (handlers.pointerup) {
    document.removeEventListener("pointerup", handlers.pointerup);
  }

  sliderPointerHandlers.delete(element);
}

export function registerScrollAreaRoot(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterScrollAreaRoot(element);

  const pointerenter = () => {
    dotNetRef.invokeMethodAsync("HandleHoverChangedAsync", true);
  };

  const pointerleave = () => {
    dotNetRef.invokeMethodAsync("HandleHoverChangedAsync", false);
  };

  element.addEventListener("pointerenter", pointerenter);
  element.addEventListener("pointerleave", pointerleave);
  scrollAreaRootHandlers.set(element, { pointerenter, pointerleave });
}

export function unregisterScrollAreaRoot(element) {
  const handlers = scrollAreaRootHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("pointerenter", handlers.pointerenter);
  element.removeEventListener("pointerleave", handlers.pointerleave);
  scrollAreaRootHandlers.delete(element);
}

export function registerScrollAreaViewport(viewport, content, dotNetRef) {
  if (!viewport) {
    return;
  }

  unregisterScrollAreaViewport(viewport);

  const notify = () => {
    const contentElement = content || viewport.firstElementChild;
    dotNetRef.invokeMethodAsync(
      "HandleViewportMetricsChangedAsync",
      viewport.scrollLeft,
      viewport.scrollTop,
      contentElement ? contentElement.scrollWidth : viewport.scrollWidth,
      contentElement ? contentElement.scrollHeight : viewport.scrollHeight,
      viewport.offsetWidth,
      viewport.offsetHeight
    );
  };

  const scroll = () => notify();
  viewport.addEventListener("scroll", scroll);

  const viewportResizeObserver = new ResizeObserver(() => {
    requestAnimationFrame(notify);
  });
  viewportResizeObserver.observe(viewport);

  let contentResizeObserver = null;
  if (content) {
    contentResizeObserver = new ResizeObserver(() => {
      requestAnimationFrame(notify);
    });
    contentResizeObserver.observe(content);
  }

  requestAnimationFrame(notify);
  scrollAreaViewportHandlers.set(viewport, { scroll, viewportResizeObserver, contentResizeObserver, content });
}

export function unregisterScrollAreaViewport(viewport) {
  const handlers = scrollAreaViewportHandlers.get(viewport);

  if (!handlers) {
    return;
  }

  viewport.removeEventListener("scroll", handlers.scroll);
  handlers.viewportResizeObserver.disconnect();

  if (handlers.contentResizeObserver) {
    handlers.contentResizeObserver.disconnect();
  }

  scrollAreaViewportHandlers.delete(viewport);
}

export function registerScrollAreaScrollbar(scrollbar, thumb, viewport, orientation, dir, dotNetRef) {
  if (!scrollbar || !viewport) {
    return;
  }

  unregisterScrollAreaScrollbar(scrollbar);

  const getPaddingValue = (style, property) => {
    const raw = style ? style[property] : "0";
    const value = Number.parseInt(raw || "0", 10);
    return Number.isFinite(value) ? value : 0;
  };

  const notify = () => {
    const style = getComputedStyle(scrollbar);
    const paddingStart = orientation === "horizontal"
      ? getPaddingValue(style, "paddingLeft")
      : getPaddingValue(style, "paddingTop");
    const paddingEnd = orientation === "horizontal"
      ? getPaddingValue(style, "paddingRight")
      : getPaddingValue(style, "paddingBottom");

    dotNetRef.invokeMethodAsync(
      "HandleScrollbarMetricsChangedAsync",
      orientation,
      scrollbar.clientWidth,
      scrollbar.clientHeight,
      paddingStart,
      paddingEnd
    );
  };

  const getThumbSize = () => {
    if (thumb) {
      return orientation === "horizontal" ? thumb.offsetWidth : thumb.offsetHeight;
    }

    return 0;
  };

  const getScrollPosition = (pointerPosition, pointerOffset) => {
    const style = getComputedStyle(scrollbar);
    const paddingStart = orientation === "horizontal"
      ? getPaddingValue(style, "paddingLeft")
      : getPaddingValue(style, "paddingTop");
    const paddingEnd = orientation === "horizontal"
      ? getPaddingValue(style, "paddingRight")
      : getPaddingValue(style, "paddingBottom");
    const thumbSize = getThumbSize();
    const scrollbarSize = orientation === "horizontal" ? scrollbar.clientWidth : scrollbar.clientHeight;
    const viewportSize = orientation === "horizontal" ? viewport.offsetWidth : viewport.offsetHeight;
    const contentSize = orientation === "horizontal" ? viewport.scrollWidth : viewport.scrollHeight;
    const maxScrollPos = contentSize - viewportSize;
    const thumbCenter = thumbSize / 2;
    const offset = pointerOffset || thumbCenter;
    const thumbOffsetFromEnd = thumbSize - offset;
    const minPointerPos = paddingStart + offset;
    const maxPointerPos = scrollbarSize - paddingEnd - thumbOffsetFromEnd;

    if (maxPointerPos <= minPointerPos || maxScrollPos <= 0) {
      return 0;
    }

    const ratio = (pointerPosition - minPointerPos) / (maxPointerPos - minPointerPos);
    const clamped = Math.min(1, Math.max(0, ratio));

    if (orientation === "horizontal" && dir === "rtl") {
      return -maxScrollPos + clamped * maxScrollPos;
    }

    return clamped * maxScrollPos;
  };

  let activePointerOffset = 0;
  const pointerdown = (event) => {
    if (event.button !== 0) {
      return;
    }

    event.preventDefault();

    const thumbElement = event.target && typeof event.target.closest === "function"
      ? event.target.closest("[data-bradix-scroll-area-thumb]")
      : null;

    if (thumbElement) {
      const thumbRect = thumbElement.getBoundingClientRect();
      activePointerOffset = orientation === "horizontal"
        ? event.clientX - thumbRect.left
        : event.clientY - thumbRect.top;
    } else {
      activePointerOffset = 0;
    }

    const scrollbarRect = scrollbar.getBoundingClientRect();
    const pointerPosition = orientation === "horizontal"
      ? event.clientX - scrollbarRect.left
      : event.clientY - scrollbarRect.top;

    const nextScrollPosition = getScrollPosition(pointerPosition, activePointerOffset);

    if (orientation === "horizontal") {
      viewport.scrollLeft = nextScrollPosition;
    } else {
      viewport.scrollTop = nextScrollPosition;
    }

    const pointermove = (moveEvent) => {
      const movePosition = orientation === "horizontal"
        ? moveEvent.clientX - scrollbarRect.left
        : moveEvent.clientY - scrollbarRect.top;
      const moveScrollPosition = getScrollPosition(movePosition, activePointerOffset);

      if (orientation === "horizontal") {
        viewport.scrollLeft = moveScrollPosition;
      } else {
        viewport.scrollTop = moveScrollPosition;
      }
    };

    const pointerup = () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
    };

    document.addEventListener("pointermove", pointermove);
    document.addEventListener("pointerup", pointerup);

    const handlers = scrollAreaScrollbarHandlers.get(scrollbar);
    if (handlers) {
      handlers.pointermove = pointermove;
      handlers.pointerup = pointerup;
    }
  };

  const resizeObserver = new ResizeObserver(() => {
    requestAnimationFrame(notify);
  });
  resizeObserver.observe(scrollbar);
  if (thumb) {
    resizeObserver.observe(thumb);
  }

  requestAnimationFrame(notify);
  scrollbar.addEventListener("pointerdown", pointerdown);
  scrollAreaScrollbarHandlers.set(scrollbar, { pointerdown, pointermove: null, pointerup: null, resizeObserver });
}

export function unregisterScrollAreaScrollbar(scrollbar) {
  const handlers = scrollAreaScrollbarHandlers.get(scrollbar);

  if (!handlers) {
    return;
  }

  scrollbar.removeEventListener("pointerdown", handlers.pointerdown);
  handlers.resizeObserver.disconnect();

  if (handlers.pointermove) {
    document.removeEventListener("pointermove", handlers.pointermove);
  }

  if (handlers.pointerup) {
    document.removeEventListener("pointerup", handlers.pointerup);
  }

  scrollAreaScrollbarHandlers.delete(scrollbar);
}

export function mountPortal(element, containerSelector) {
  if (!element) {
    return;
  }

  unmountPortal(element);

  const parent = element.parentNode;
  const nextSibling = element.nextSibling;
  const container = containerSelector
    ? document.querySelector(containerSelector)
    : document.body;

  if (!container) {
    return;
  }

  container.appendChild(element);
  portalMounts.set(element, { parent, nextSibling });
}

export function unmountPortal(element) {
  const mount = portalMounts.get(element);

  if (!mount) {
    return;
  }

  if (mount.parent) {
    if (mount.nextSibling && mount.nextSibling.parentNode === mount.parent) {
      mount.parent.insertBefore(element, mount.nextSibling);
    } else {
      mount.parent.appendChild(element);
    }
  }

  portalMounts.delete(element);
}

function updateDismissableLayerPointerEvents() {
  const highestDisabledIndex = [...dismissableLayers].map((layer) => layer.disableOutsidePointerEvents).lastIndexOf(true);

  if (highestDisabledIndex >= 0) {
    if (!originalDismissableBodyPointerEvents) {
      originalDismissableBodyPointerEvents = document.body.style.pointerEvents || "";
    }

    document.body.style.pointerEvents = "none";
  } else if (originalDismissableBodyPointerEvents !== "") {
    document.body.style.pointerEvents = originalDismissableBodyPointerEvents;
    originalDismissableBodyPointerEvents = "";
  } else {
    document.body.style.pointerEvents = "";
  }

  dismissableLayers.forEach((layer, index) => {
    if (highestDisabledIndex >= 0) {
      layer.element.style.pointerEvents = index >= highestDisabledIndex ? "auto" : "none";
    } else {
      layer.element.style.pointerEvents = "";
    }
  });
}

function ensureDismissableLayerListeners() {
  if (dismissableLayerListenersRegistered) {
    return;
  }

  document.addEventListener("pointerdown", (event) => {
    const topLayer = dismissableLayers[dismissableLayers.length - 1];

    if (!topLayer || !event.target) {
      return;
    }

    if (topLayer.element.contains(event.target)) {
      return;
    }

    for (const branch of dismissableBranches) {
      if (branch.contains(event.target)) {
        return;
      }
    }

    topLayer.dotNetRef.invokeMethodAsync("HandlePointerDownOutsideAsync");
  });

  document.addEventListener("focusin", (event) => {
    const topLayer = dismissableLayers[dismissableLayers.length - 1];

    if (!topLayer || !event.target) {
      return;
    }

    if (topLayer.element.contains(event.target)) {
      return;
    }

    for (const branch of dismissableBranches) {
      if (branch.contains(event.target)) {
        return;
      }
    }

    topLayer.dotNetRef.invokeMethodAsync("HandleFocusOutsideAsync");
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
      return;
    }

    const topLayer = dismissableLayers[dismissableLayers.length - 1];
    if (!topLayer) {
      return;
    }

    topLayer.dotNetRef.invokeMethodAsync("HandleEscapeKeyDownAsync");
  });

  dismissableLayerListenersRegistered = true;
}

export function registerDismissableLayer(element, dotNetRef, disableOutsidePointerEvents) {
  if (!element) {
    return;
  }

  unregisterDismissableLayer(element);
  ensureDismissableLayerListeners();
  dismissableLayers.push({ element, dotNetRef, disableOutsidePointerEvents: !!disableOutsidePointerEvents });
  updateDismissableLayerPointerEvents();
}

export function updateDismissableLayer(element, disableOutsidePointerEvents) {
  const layer = dismissableLayers.find((item) => item.element === element);

  if (!layer) {
    return;
  }

  layer.disableOutsidePointerEvents = !!disableOutsidePointerEvents;
  updateDismissableLayerPointerEvents();
}

export function unregisterDismissableLayer(element) {
  const index = dismissableLayers.findIndex((item) => item.element === element);

  if (index < 0) {
    return;
  }

  dismissableLayers.splice(index, 1);

  if (element) {
    element.style.pointerEvents = "";
  }

  updateDismissableLayerPointerEvents();
}

export function registerDismissableLayerBranch(element) {
  if (!element) {
    return;
  }

  dismissableBranches.add(element);
}

export function unregisterDismissableLayerBranch(element) {
  if (!element) {
    return;
  }

  dismissableBranches.delete(element);
}

function arrayRemove(array, item) {
  const next = [...array];
  const index = next.indexOf(item);
  if (index >= 0) {
    next.splice(index, 1);
  }
  return next;
}

function getTabbableCandidates(container) {
  const nodes = [];
  const walker = document.createTreeWalker(container, NodeFilter.SHOW_ELEMENT, {
    acceptNode: (node) => {
      const isHiddenInput = node.tagName === "INPUT" && node.type === "hidden";
      if (node.disabled || node.hidden || isHiddenInput) {
        return NodeFilter.FILTER_SKIP;
      }

      return node.tabIndex >= 0 ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP;
    }
  });

  while (walker.nextNode()) {
    nodes.push(walker.currentNode);
  }

  return nodes;
}

function isHidden(node, upTo) {
  if (getComputedStyle(node).visibility === "hidden") {
    return true;
  }

  while (node) {
    if (upTo && node === upTo) {
      return false;
    }

    if (getComputedStyle(node).display === "none") {
      return true;
    }

    node = node.parentElement;
  }

  return false;
}

function findVisible(elements, container) {
  for (const element of elements) {
    if (!isHidden(element, container)) {
      return element;
    }
  }

  return null;
}

function getTabbableEdges(container) {
  const candidates = getTabbableCandidates(container);
  const first = findVisible(candidates, container);
  const last = findVisible([...candidates].reverse(), container);
  return [first, last];
}

function focusElement(element, select) {
  if (!element || typeof element.focus !== "function") {
    return;
  }

  const previous = document.activeElement;
  element.focus({ preventScroll: true });

  if (select && element !== previous && element instanceof HTMLInputElement && typeof element.select === "function") {
    element.select();
  }
}

function focusFirst(candidates, select) {
  const previous = document.activeElement;
  for (const candidate of candidates) {
    focusElement(candidate, select);
    if (document.activeElement !== previous) {
      return;
    }
  }
}

function removeLinks(items) {
  return items.filter((item) => item.tagName !== "A");
}

function addFocusScopeToStack(scope) {
  const active = focusScopeStack[0];
  if (active && active !== scope) {
    active.paused = true;
  }

  const next = arrayRemove(focusScopeStack, scope);
  next.unshift(scope);
  focusScopeStack.length = 0;
  focusScopeStack.push(...next);
}

function removeFocusScopeFromStack(scope) {
  const next = arrayRemove(focusScopeStack, scope);
  focusScopeStack.length = 0;
  focusScopeStack.push(...next);
  if (focusScopeStack[0]) {
    focusScopeStack[0].paused = false;
  }
}

export function registerFocusScope(element, dotNetRef, loop, trapped) {
  if (!element) {
    return;
  }

  unregisterFocusScope(element);

  const scope = {
    element,
    dotNetRef,
    loop: !!loop,
    trapped: !!trapped,
    paused: false,
    lastFocusedElement: null,
    previouslyFocusedElement: document.activeElement instanceof HTMLElement ? document.activeElement : null
  };

  const focusin = (event) => {
    if (scope.paused || !scope.trapped) {
      return;
    }

    const target = event.target;
    if (scope.element.contains(target)) {
      scope.lastFocusedElement = target;
    } else {
      focusElement(scope.lastFocusedElement || scope.element, true);
    }
  };

  const focusout = (event) => {
    if (scope.paused || !scope.trapped) {
      return;
    }

    const relatedTarget = event.relatedTarget;
    if (relatedTarget === null) {
      return;
    }

    if (!scope.element.contains(relatedTarget)) {
      focusElement(scope.lastFocusedElement || scope.element, true);
    }
  };

  const mutationObserver = new MutationObserver((mutations) => {
    if (!scope.trapped) {
      return;
    }

    const focusedElement = document.activeElement;
    if (focusedElement !== document.body) {
      return;
    }

    for (const mutation of mutations) {
      if (mutation.removedNodes.length > 0) {
        focusElement(scope.element, false);
        break;
      }
    }
  });

  const keydown = (event) => {
    if ((!scope.loop && !scope.trapped) || scope.paused) {
      return;
    }

    const isTabKey = event.key === "Tab" && !event.altKey && !event.ctrlKey && !event.metaKey;
    const focusedElement = document.activeElement;

    if (!isTabKey || !focusedElement) {
      return;
    }

    const [first, last] = getTabbableEdges(scope.element);
    const hasTabbableElementsInside = first && last;

    if (!hasTabbableElementsInside) {
      if (focusedElement === scope.element) {
        event.preventDefault();
      }
      return;
    }

    if (!event.shiftKey && focusedElement === last) {
      event.preventDefault();
      if (scope.loop) {
        focusElement(first, true);
      }
    } else if (event.shiftKey && focusedElement === first) {
      event.preventDefault();
      if (scope.loop) {
        focusElement(last, true);
      }
    }
  };

  document.addEventListener("focusin", focusin);
  document.addEventListener("focusout", focusout);
  scope.element.addEventListener("keydown", keydown);
  mutationObserver.observe(scope.element, { childList: true, subtree: true });

  addFocusScopeToStack(scope);

  const previous = scope.previouslyFocusedElement;
  const hasFocusedCandidate = previous && scope.element.contains(previous);
  if (!hasFocusedCandidate) {
    dotNetRef.invokeMethodAsync("HandleMountAutoFocusAsync");
    focusFirst(removeLinks(getTabbableCandidates(scope.element)), true);
    if (document.activeElement === previous) {
      focusElement(scope.element, false);
    }
  }

  focusScopeHandlers.set(element, { scope, focusin, focusout, keydown, mutationObserver });
}

export function updateFocusScope(element, loop, trapped) {
  const handlers = focusScopeHandlers.get(element);

  if (!handlers) {
    return;
  }

  handlers.scope.loop = !!loop;
  handlers.scope.trapped = !!trapped;
}

export function unregisterFocusScope(element) {
  const handlers = focusScopeHandlers.get(element);

  if (!handlers) {
    return;
  }

  document.removeEventListener("focusin", handlers.focusin);
  document.removeEventListener("focusout", handlers.focusout);
  element.removeEventListener("keydown", handlers.keydown);
  handlers.mutationObserver.disconnect();

  setTimeout(() => {
    handlers.scope.dotNetRef.invokeMethodAsync("HandleUnmountAutoFocusAsync");
    focusElement(handlers.scope.previouslyFocusedElement || document.body, true);
    removeFocusScopeFromStack(handlers.scope);
  }, 0);

  focusScopeHandlers.delete(element);
}

function getOppositeSide(side) {
  return {
    top: "bottom",
    right: "left",
    bottom: "top",
    left: "right"
  }[side] || "bottom";
}

function clamp(value, min, max) {
  return Math.min(Math.max(value, min), max);
}

function resolvePopperPosition(anchorRect, contentRect, options) {
  const side = options.side || "bottom";
  const align = options.align || "center";
  const sideOffset = Number(options.sideOffset || 0);
  const alignOffset = Number(options.alignOffset || 0);
  const collisionPadding = Number(options.collisionPadding || 0);
  const arrowPadding = Number(options.arrowPadding || 0);
  const avoidCollisions = options.avoidCollisions !== false;
  const viewportWidth = window.innerWidth;
  const viewportHeight = window.innerHeight;
  const availableWidth = Math.max(viewportWidth - collisionPadding * 2, 0);
  const availableHeight = Math.max(viewportHeight - collisionPadding * 2, 0);
  const spaces = {
    top: anchorRect.top - collisionPadding,
    right: viewportWidth - anchorRect.right - collisionPadding,
    bottom: viewportHeight - anchorRect.bottom - collisionPadding,
    left: anchorRect.left - collisionPadding
  };

  let placedSide = side;
  if (avoidCollisions) {
    const requiredMainAxis = side === "top" || side === "bottom" ? contentRect.height + sideOffset : contentRect.width + sideOffset;
    if (spaces[side] < requiredMainAxis && spaces[getOppositeSide(side)] > spaces[side]) {
      placedSide = getOppositeSide(side);
    }
  }

  let left = 0;
  let top = 0;

  if (placedSide === "bottom" || placedSide === "top") {
    top = placedSide === "bottom"
      ? anchorRect.bottom + sideOffset
      : anchorRect.top - contentRect.height - sideOffset;

    if (align === "start") {
      left = anchorRect.left + alignOffset;
    } else if (align === "end") {
      left = anchorRect.right - contentRect.width + alignOffset;
    } else {
      left = anchorRect.left + ((anchorRect.width - contentRect.width) / 2) + alignOffset;
    }
  } else {
    left = placedSide === "right"
      ? anchorRect.right + sideOffset
      : anchorRect.left - contentRect.width - sideOffset;

    if (align === "start") {
      top = anchorRect.top + alignOffset;
    } else if (align === "end") {
      top = anchorRect.bottom - contentRect.height + alignOffset;
    } else {
      top = anchorRect.top + ((anchorRect.height - contentRect.height) / 2) + alignOffset;
    }
  }

  if (avoidCollisions) {
    left = clamp(left, collisionPadding, Math.max(collisionPadding, viewportWidth - collisionPadding - contentRect.width));
    top = clamp(top, collisionPadding, Math.max(collisionPadding, viewportHeight - collisionPadding - contentRect.height));
  }

  let arrowX = null;
  let arrowY = null;
  let transformOriginX = "50%";
  let transformOriginY = "50%";

  if (placedSide === "bottom" || placedSide === "top") {
    const anchorCenterX = anchorRect.left + (anchorRect.width / 2);
    arrowX = clamp(anchorCenterX - left, arrowPadding, Math.max(arrowPadding, contentRect.width - arrowPadding));
    transformOriginX = `${arrowX}px`;
    transformOriginY = placedSide === "bottom" ? "0px" : `${contentRect.height}px`;
  } else {
    const anchorCenterY = anchorRect.top + (anchorRect.height / 2);
    arrowY = clamp(anchorCenterY - top, arrowPadding, Math.max(arrowPadding, contentRect.height - arrowPadding));
    transformOriginX = placedSide === "right" ? "0px" : `${contentRect.width}px`;
    transformOriginY = `${arrowY}px`;
  }

  const referenceHidden =
    anchorRect.bottom < 0 ||
    anchorRect.top > viewportHeight ||
    anchorRect.right < 0 ||
    anchorRect.left > viewportWidth;

  return {
    placedSide,
    placedAlign: align,
    left,
    top,
    availableWidth,
    availableHeight,
    anchorWidth: anchorRect.width,
    anchorHeight: anchorRect.height,
    arrowX,
    arrowY,
    shouldHideArrow: false,
    referenceHidden,
    transformOriginX,
    transformOriginY
  };
}

function updateRegisteredPopperContent(content) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers || !handlers.anchor || !handlers.content) {
    return;
  }

  const anchorRect = handlers.anchor.getBoundingClientRect();
  const contentRect = handlers.content.getBoundingClientRect();

  const position = resolvePopperPosition(anchorRect, contentRect, handlers.options);
  handlers.dotNetRef.invokeMethodAsync(
    "HandlePositionChangedAsync",
    position.placedSide,
    position.placedAlign,
    position.left,
    position.top,
    position.availableWidth,
    position.availableHeight,
    position.anchorWidth,
    position.anchorHeight,
    position.arrowX,
    position.arrowY,
    position.shouldHideArrow,
    handlers.options.hideWhenDetached === true && position.referenceHidden,
    position.transformOriginX,
    position.transformOriginY);
}

export function registerPopperContent(anchor, content, arrow, dotNetRef, options) {
  if (!anchor || !content) {
    return;
  }

  unregisterPopperContent(content);

  const update = () => updateRegisteredPopperContent(content);
  const resizeObserver = new ResizeObserver(update);
  resizeObserver.observe(anchor);
  resizeObserver.observe(content);
  if (arrow) {
    resizeObserver.observe(arrow);
  }

  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, true);

  popperContentHandlers.set(content, {
    anchor,
    content,
    arrow,
    dotNetRef,
    options: options || {},
    resizeObserver,
    update
  });

  update();
}

export function updatePopperContent(content, arrow, options) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.arrow = arrow;
  handlers.options = options || {};
  handlers.resizeObserver.disconnect();
  handlers.resizeObserver.observe(handlers.anchor);
  handlers.resizeObserver.observe(handlers.content);
  if (arrow) {
    handlers.resizeObserver.observe(arrow);
  }

  handlers.update();
}

export function unregisterPopperContent(content) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.resizeObserver.disconnect();
  window.removeEventListener("resize", handlers.update);
  window.removeEventListener("scroll", handlers.update, true);
  popperContentHandlers.delete(content);
}

export function registerPresence(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterPresence(element);

  const handleAnimationStart = (event) => {
    if (event.target === element) {
      dotNetRef.invokeMethodAsync("HandleAnimationStartAsync", event.animationName || "none");
    }
  };

  const handleAnimationEnd = (event) => {
    if (event.target === element) {
      dotNetRef.invokeMethodAsync("HandleAnimationEndAsync", event.animationName || "none");
    }
  };

  element.addEventListener("animationstart", handleAnimationStart);
  element.addEventListener("animationcancel", handleAnimationEnd);
  element.addEventListener("animationend", handleAnimationEnd);

  presenceHandlers.set(element, {
    handleAnimationStart,
    handleAnimationEnd
  });
}

export function getPresenceState(element) {
  if (!element) {
    return { animationName: "none", display: "none" };
  }

  const styles = getComputedStyle(element);
  return {
    animationName: styles.animationName || "none",
    display: styles.display || ""
  };
}

export function unregisterPresence(element) {
  const handlers = presenceHandlers.get(element);
  if (!handlers) {
    return;
  }

  element.removeEventListener("animationstart", handlers.handleAnimationStart);
  element.removeEventListener("animationcancel", handlers.handleAnimationEnd);
  element.removeEventListener("animationend", handlers.handleAnimationEnd);
  presenceHandlers.delete(element);
}

function createFocusGuard() {
  const element = document.createElement("span");
  element.setAttribute("data-radix-focus-guard", "");
  element.tabIndex = 0;
  element.style.outline = "none";
  element.style.opacity = "0";
  element.style.position = "fixed";
  element.style.pointerEvents = "none";
  return element;
}

export function registerFocusGuards() {
  const edgeGuards = document.querySelectorAll("[data-radix-focus-guard]");
  document.body.insertAdjacentElement("afterbegin", edgeGuards[0] || createFocusGuard());
  document.body.insertAdjacentElement("beforeend", edgeGuards[1] || createFocusGuard());
  focusGuardsCount += 1;
}

export function unregisterFocusGuards() {
  if (focusGuardsCount <= 1) {
    document.querySelectorAll("[data-radix-focus-guard]").forEach((node) => node.remove());
  }

  focusGuardsCount = Math.max(focusGuardsCount - 1, 0);
}

export function registerHideOthers(element) {
  if (!element || !document.body) {
    return;
  }

  unregisterHideOthers(element);

  const changed = [];
  const bodyChildren = Array.from(document.body.children);

  for (const child of bodyChildren) {
    if (child === element || child.contains(element) || element.contains(child)) {
      continue;
    }

    changed.push({
      element: child,
      previous: child.getAttribute("aria-hidden")
    });

    child.setAttribute("aria-hidden", "true");
  }

  hideOthersState.set(element, changed);
}

export function unregisterHideOthers(element) {
  const changed = hideOthersState.get(element);
  if (!changed) {
    return;
  }

  for (const entry of changed) {
    if (entry.previous === null) {
      entry.element.removeAttribute("aria-hidden");
    } else {
      entry.element.setAttribute("aria-hidden", entry.previous);
    }
  }

  hideOthersState.delete(element);
}

export function registerRemoveScroll() {
  if (!document.body || !document.documentElement) {
    return;
  }

  if (removeScrollCount === 0) {
    originalBodyOverflow = document.body.style.overflow || "";
    originalBodyPaddingRight = document.body.style.paddingRight || "";

    const scrollbarWidth = Math.max(window.innerWidth - document.documentElement.clientWidth, 0);
    document.body.style.overflow = "hidden";

    if (scrollbarWidth > 0) {
      document.body.style.paddingRight = `${scrollbarWidth}px`;
    }
  }

  removeScrollCount += 1;
}

export function unregisterRemoveScroll() {
  if (!document.body) {
    return;
  }

  removeScrollCount = Math.max(removeScrollCount - 1, 0);

  if (removeScrollCount === 0) {
    document.body.style.overflow = originalBodyOverflow;
    document.body.style.paddingRight = originalBodyPaddingRight;
  }
}

export function registerLabelTextSelectionGuard(element) {
  if (!element) {
    return;
  }

  unregisterLabelTextSelectionGuard(element);

  const handler = (event) => {
    const target = event.target;

    if (target && typeof target.closest === "function" && target.closest("button, input, select, textarea")) {
      return;
    }

    if (!event.defaultPrevented && event.detail > 1) {
      event.preventDefault();
    }
  };

  element.addEventListener("mousedown", handler);
  labelTextSelectionHandlers.set(element, handler);
}

export function unregisterLabelTextSelectionGuard(element) {
  const handler = labelTextSelectionHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("mousedown", handler);
  labelTextSelectionHandlers.delete(element);
}
