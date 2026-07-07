import { cssEscape } from "./core/dom.js";

const selectViewportHandlers = new WeakMap();
const selectContentPointerTrackers = new WeakMap();
const selectContentKeyboardHandlers = new WeakMap();
const selectWindowDismissHandlers = new WeakMap();
const selectItemAlignedHandlers = new WeakMap();

const selectContentKeyboardKeys = new Set([
  "ArrowUp",
  "ArrowDown",
  "Home",
  "End",
  "PageUp",
  "PageDown"
]);

const TYPEAHEAD_RESET_MS = 700;
const POINTER_UP_OPTION_RETRY_COUNT = 8;

function isSelectViewportScrollEvent(event, viewport) {
  const target = event?.target;
  return viewport instanceof HTMLElement && target instanceof Node && (target === viewport || viewport.contains(target));
}

function invokeDotNetSafely(dotNetRef, methodName, ...args) {
  try {
    const invocation = dotNetRef?.invokeMethodAsync?.(methodName, ...args);
    if (invocation && typeof invocation.catch === "function") {
      invocation.catch(() => {});
    }
  } catch {
  }
}

export function registerSelectViewport(viewport, content, wrapper, dotNetRef) {
  if (!viewport) {
    return;
  }

  const itemAlignedWrapper = wrapper instanceof HTMLElement && wrapper.getAttribute("data-radix-select-position") === "item-aligned"
    ? wrapper
    : null;

  unregisterSelectViewport(viewport);

  const registration = {
    animationFrameIds: new Set()
  };

  const queueNotify = () => {
    const frameId = requestAnimationFrame(() => {
      registration.animationFrameIds.delete(frameId);

      if (selectViewportHandlers.get(viewport) !== registration) {
        return;
      }

      notify();
    });

    registration.animationFrameIds.add(frameId);
  };

  const expandOnScroll = () => {
    if (!itemAlignedWrapper) {
      return;
    }

    const itemAligned = selectItemAlignedHandlers.get(itemAlignedWrapper);
    if (!itemAligned || !itemAligned.state.shouldExpandOnScroll) {
      return;
    }

    const scrollDelta = viewport.scrollTop - itemAligned.state.previousScrollTop;
    const scrolledBy = Math.abs(scrollDelta);
    if (scrolledBy <= 0) {
      itemAligned.state.previousScrollTop = viewport.scrollTop;
      return;
    }

    const isBottomAnchored = itemAlignedWrapper.style.bottom === "0px";
    const isTopAnchored = itemAlignedWrapper.style.top === "0px";
    const isRevealingMoreContent = isBottomAnchored
      ? scrollDelta > 0
      : isTopAnchored
        ? scrollDelta < 0
        : true;

    if (!isRevealingMoreContent) {
      itemAligned.state.previousScrollTop = viewport.scrollTop;
      return;
    }

    const CONTENT_MARGIN = 10;
    const availableHeight = window.innerHeight - CONTENT_MARGIN * 2;
    const cssMinHeight = parseFloat(itemAlignedWrapper.style.minHeight || "0");
    const cssHeight = parseFloat(itemAlignedWrapper.style.height || "0");
    const previousHeight = Math.max(cssMinHeight, cssHeight);

    if (previousHeight < availableHeight) {
      const nextHeight = previousHeight + scrolledBy;
      const clampedNextHeight = Math.min(availableHeight, nextHeight);
      const heightDiff = nextHeight - clampedNextHeight;

      itemAlignedWrapper.style.height = `${clampedNextHeight}px`;
      if (isBottomAnchored) {
        viewport.scrollTop = heightDiff > 0 ? heightDiff : 0;
        itemAlignedWrapper.style.justifyContent = "flex-end";
      }
    }

    itemAligned.state.previousScrollTop = viewport.scrollTop;
  };

  const notify = () => {
    if (selectViewportHandlers.get(viewport) !== registration) {
      return;
    }

    const contentElement = content || viewport.firstElementChild;
    invokeDotNetSafely(
      dotNetRef,
      "HandleViewportMetricsChanged",
      viewport.scrollTop,
      contentElement ? contentElement.scrollHeight : viewport.scrollHeight,
      viewport.offsetHeight
    );
  };

  const scroll = () => {
    expandOnScroll();
    notify();
  };
  viewport.addEventListener("scroll", scroll);

  const viewportResizeObserver = new ResizeObserver(() => {
    queueNotify();
  });
  viewportResizeObserver.observe(viewport);

  let contentResizeObserver = null;
  if (content) {
    contentResizeObserver = new ResizeObserver(() => {
      queueNotify();
    });
    contentResizeObserver.observe(content);
  }

  registration.scroll = scroll;
  registration.viewportResizeObserver = viewportResizeObserver;
  registration.contentResizeObserver = contentResizeObserver;

  selectViewportHandlers.set(viewport, registration);
  queueNotify();
  queueItemAlignedPositionFromViewport(viewport, content, itemAlignedWrapper);
}

export function unregisterSelectViewport(viewport) {
  const handlers = selectViewportHandlers.get(viewport);
  if (!handlers) {
    return;
  }

  viewport.removeEventListener("scroll", handlers.scroll);
  handlers.viewportResizeObserver.disconnect();
  if (handlers.contentResizeObserver) {
    handlers.contentResizeObserver.disconnect();
  }
  if (handlers.animationFrameIds) {
    for (const frameId of handlers.animationFrameIds) {
      cancelAnimationFrame(frameId);
    }
    handlers.animationFrameIds.clear();
  }

  selectViewportHandlers.delete(viewport);
}

export function registerSelectContentKeyboard(content, dotNetRef) {
  if (!content || !dotNetRef) {
    return;
  }

  unregisterSelectContentKeyboard(content);

  const registration = {
    search: "",
    searchResetTimeout: 0,
    focusedValue: "",
    dotNetRef
  };

  registration.mutationObserver = new MutationObserver(() => {
    applyFocusedSelectValue(content, registration);
  });
  registration.mutationObserver.observe(content, {
    attributes: true,
    attributeFilter: ["data-highlighted", "data-state", "aria-selected"],
    childList: true,
    subtree: true
  });

  const keydown = (event) => {
    if (event.ctrlKey || event.altKey || event.metaKey || event.key === "Tab") {
      return;
    }

    if (!selectContentKeyboardKeys.has(event.key) && event.key.length !== 1) {
      return;
    }

    event.preventDefault();
    event.stopPropagation();
    event.stopImmediatePropagation?.();

    if (focusSelectContentKeyboardTarget(content, event.key, registration)) {
      return;
    }

    invokeDotNetSafely(dotNetRef, "HandleDelegatedContentKeyDown", createSelectKeyboardEventSnapshot(event));
  };

  content.addEventListener("keydown", keydown, true);
  registration.keydown = keydown;
  selectContentKeyboardHandlers.set(content, registration);
}

export function unregisterSelectContentKeyboard(content) {
  if (!content) {
    return;
  }

  const handlers = selectContentKeyboardHandlers.get(content);

  if (!handlers) {
    return;
  }

  content.removeEventListener("keydown", handlers.keydown, true);
  clearTimeout(handlers.searchResetTimeout);
  handlers.mutationObserver?.disconnect();
  selectContentKeyboardHandlers.delete(content);
}

function createSelectKeyboardEventSnapshot(event) {
  return {
    key: event.key || "",
    code: event.code || "",
    ctrlKey: !!event.ctrlKey,
    shiftKey: !!event.shiftKey,
    altKey: !!event.altKey,
    metaKey: !!event.metaKey,
    repeat: !!event.repeat,
    defaultPrevented: !!event.defaultPrevented,
    targetId: event.target instanceof HTMLElement && event.target.id ? event.target.id : "",
    ancestorIds: []
  };
}

function focusSelectContentKeyboardTarget(content, key, registration) {
  const options = getEnabledSelectOptions(content);

  if (options.length === 0) {
    return false;
  }

  const current = getCurrentSelectOption(content, options);
  let target = null;

  switch (key) {
    case "Home":
    case "PageUp":
      target = options[0];
      break;
    case "End":
    case "PageDown":
      target = options[options.length - 1];
      break;
    case "ArrowUp":
      target = getAdjacentSelectOption(options, current, -1);
      break;
    case "ArrowDown":
      target = getAdjacentSelectOption(options, current, 1);
      break;
    default:
      if (key.length === 1) {
        target = getTypeaheadSelectOption(options, current, key, registration);
      }
      break;
  }

  if (!target) {
    return false;
  }

  registration.focusedValue = target.getAttribute("data-value") || "";
  setHighlightedSelectOption(content, target);
  invokeDotNetSafely(registration.dotNetRef, "HandleDelegatedContentFocusedValueChanged", registration.focusedValue);
  target.focus({ preventScroll: true });
  target.scrollIntoView({ block: "nearest" });
  return true;
}

function applyFocusedSelectValue(content, registration) {
  if (!registration.focusedValue) {
    return;
  }

  const target = content.querySelector(`[role='option'][data-value="${cssEscape(registration.focusedValue)}"]`);

  if (!target || target.hasAttribute("data-highlighted")) {
    return;
  }

  setHighlightedSelectOption(content, target);
}

function getEnabledSelectOptions(content) {
  return Array.from(content.querySelectorAll("[role='option']"))
    .filter(option => !option.hasAttribute("data-disabled") && option.getAttribute("aria-disabled") !== "true");
}

function getCurrentSelectOption(content, options) {
  const active = document.activeElement instanceof HTMLElement && content.contains(document.activeElement)
    ? document.activeElement.closest("[role='option']")
    : null;

  if (active && options.includes(active)) {
    return active;
  }

  return options.find(option => option.hasAttribute("data-highlighted"))
    || options.find(option => option.getAttribute("data-state") === "checked")
    || options[0];
}

function getAdjacentSelectOption(options, current, delta) {
  const currentIndex = Math.max(options.indexOf(current), 0);
  const nextIndex = (currentIndex + delta + options.length) % options.length;
  return options[nextIndex];
}

function getTypeaheadSelectOption(options, current, key, registration) {
  clearTimeout(registration.searchResetTimeout);
  const normalizedKey = key.toLowerCase();
  registration.search += normalizedKey;
  registration.searchResetTimeout = setTimeout(() => {
    registration.search = "";
  }, TYPEAHEAD_RESET_MS);

  const currentIndex = Math.max(options.indexOf(current), -1);
  const ordered = registration.search.length === 1
    ? options
    : options.slice(currentIndex + 1).concat(options.slice(0, currentIndex + 1));
  let target = findTypeaheadOption(ordered, registration.search);

  if (!target && registration.search !== normalizedKey) {
    registration.search = normalizedKey;
    target = findTypeaheadOption(options, registration.search);
  }

  return target;
}

function findTypeaheadOption(options, search) {
  return options.find(option => (option.textContent || "").trim().toLowerCase().startsWith(search));
}

function setHighlightedSelectOption(content, target) {
  for (const option of content.querySelectorAll("[role='option'][data-highlighted]")) {
    if (option !== target) {
      option.removeAttribute("data-highlighted");
      if (option.getAttribute("aria-selected") === "true") {
        option.setAttribute("aria-selected", "false");
      }
    }
  }

  target.setAttribute("data-highlighted", "");
  if (target.getAttribute("data-state") === "checked") {
    target.setAttribute("aria-selected", "true");
  }
}

export function scrollSelectViewportByItem(viewport, item, upward) {
  if (!viewport || !item) {
    return;
  }

  const delta = item.offsetHeight || 0;
  viewport.scrollTop = upward ? viewport.scrollTop - delta : viewport.scrollTop + delta;
}

export function registerSelectContentPointerTracker(content, dotNetRef, pageX, pageY) {
  if (!content) {
    return;
  }

  unregisterSelectContentPointerTracker(content);

  const registration = {};

  let pointerMoveDelta = { x: 0, y: 0 };

  const handlePointerMove = (event) => {
    if (selectContentPointerTrackers.get(content) !== registration) {
      return;
    }

    pointerMoveDelta = {
      x: Math.abs(Math.round(event.pageX) - Math.round(pageX || 0)),
      y: Math.abs(Math.round(event.pageY) - Math.round(pageY || 0))
    };
  };

  const handlePointerUp = (event) => {
    if (selectContentPointerTrackers.get(content) !== registration) {
      return;
    }

    document.removeEventListener("pointermove", handlePointerMove);
    selectContentPointerTrackers.delete(content);

    const withinPointerTolerance = pointerMoveDelta.x <= 10 && pointerMoveDelta.y <= 10;
    const target = event.target;
    const targetInsideContent = !!target && content.contains(target);
    const shouldClose = !withinPointerTolerance && !!target && !targetInsideContent;

    if (withinPointerTolerance) {
      event.preventDefault();
      event.stopPropagation();
      event.stopImmediatePropagation?.();
      invokeDotNetSafely(dotNetRef, "HandleTriggerPointerGuardResult", true, false);
      return;
    }

    invokeDotNetSafely(dotNetRef, "HandleTriggerPointerGuardResult", false, shouldClose);
  };

  document.addEventListener("pointermove", handlePointerMove);
  document.addEventListener("pointerup", handlePointerUp, { capture: true, once: true });
  registration.handlePointerMove = handlePointerMove;
  registration.handlePointerUp = handlePointerUp;
  selectContentPointerTrackers.set(content, registration);
}

export function unregisterSelectContentPointerTracker(content) {
  const handlers = selectContentPointerTrackers.get(content);
  if (!handlers) {
    return;
  }

  document.removeEventListener("pointermove", handlers.handlePointerMove);
  document.removeEventListener("pointerup", handlers.handlePointerUp, true);
  selectContentPointerTrackers.delete(content);
}

export function getSelectOptionValueAtPoint(clientX, clientY) {
  return new Promise((resolve) => {
    let attempts = 0;

    const read = () => {
      const target = document.elementFromPoint(clientX, clientY);
      const option = target instanceof HTMLElement
        ? target.closest("[role='option']")
        : null;

      if (!option) {
        if (attempts < POINTER_UP_OPTION_RETRY_COUNT) {
          attempts += 1;
          requestAnimationFrame(read);
          return;
        }

        resolve(null);
        return;
      }

      if (option.hasAttribute("data-disabled") || option.getAttribute("aria-disabled") === "true") {
        resolve(null);
        return;
      }

      resolve(option.getAttribute("data-value"));
    };

    requestAnimationFrame(read);
  });
}

export function registerSelectWindowDismiss(content, dotNetRef) {
  if (!content || !dotNetRef) {
    return;
  }

  unregisterSelectWindowDismiss(content);

  const registration = {};

  const dismiss = () => {
    if (selectWindowDismissHandlers.get(content) !== registration) {
      return;
    }

    invokeDotNetSafely(dotNetRef, "HandleWindowDismiss");
  };

  window.addEventListener("blur", dismiss);
  window.addEventListener("resize", dismiss);

  registration.dismiss = dismiss;
  selectWindowDismissHandlers.set(content, registration);
}

export function unregisterSelectWindowDismiss(content) {
  const handlers = selectWindowDismissHandlers.get(content);
  if (!handlers) {
    return;
  }

  window.removeEventListener("blur", handlers.dismiss);
  window.removeEventListener("resize", handlers.dismiss);
  selectWindowDismissHandlers.delete(content);
}

export function registerSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir) {
  if (!(wrapper instanceof HTMLElement)
    || !(content instanceof HTMLElement)
    || !(viewport instanceof HTMLElement)
    || !(trigger instanceof HTMLElement)
    || !(valueNode instanceof HTMLElement)
    || !(selectedItem instanceof HTMLElement)
    || !(selectedItemText instanceof HTMLElement)) {
    return;
  }

  unregisterSelectItemAlignedPosition(wrapper);

  const state = {
    content,
    viewport,
    trigger,
    valueNode,
    selectedItem,
    selectedItemText,
    dir,
    animationFrameId: 0,
    hasPositioned: false,
    shouldExpandOnScroll: false,
    previousScrollTop: viewport ? viewport.scrollTop : 0
  };
  const updateNow = (preserveViewportScroll = state.hasPositioned) => {
    if (state.animationFrameId) {
      cancelAnimationFrame(state.animationFrameId);
      state.animationFrameId = 0;
    }

    positionSelectItemAligned(
      wrapper,
      state.content,
      state.viewport,
      state.trigger,
      state.valueNode,
      state.selectedItem,
      state.selectedItemText,
      state.dir,
      preserveViewportScroll
    );
    state.hasPositioned = true;
  };
  const update = (event) => {
    if (event instanceof Event && isSelectViewportScrollEvent(event, state.viewport)) {
      return;
    }

    if (state.animationFrameId) {
      return;
    }

    const preserveViewportScroll = state.hasPositioned;
    state.animationFrameId = requestAnimationFrame(() => {
      state.animationFrameId = 0;
      positionSelectItemAligned(
        wrapper,
        state.content,
        state.viewport,
        state.trigger,
        state.valueNode,
        state.selectedItem,
        state.selectedItemText,
        state.dir,
        preserveViewportScroll
      );
      state.hasPositioned = true;
    });
  };
  const resizeObserver = new ResizeObserver(update);

  resizeObserver.observe(content);
  if (viewport) {
    resizeObserver.observe(viewport);
  }
  if (trigger) {
    resizeObserver.observe(trigger);
  }
  if (selectedItem) {
    resizeObserver.observe(selectedItem);
  }
  if (selectedItemText) {
    resizeObserver.observe(selectedItemText);
  }

  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, true);

  selectItemAlignedHandlers.set(wrapper, {
    update,
    updateNow,
    resizeObserver,
    state
  });

  requestAnimationFrame(() => {
    updateNow();
    state.previousScrollTop = viewport ? viewport.scrollTop : 0;
    requestAnimationFrame(() => {
      state.shouldExpandOnScroll = true;
    });
  });
}

export function updateSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir) {
  const handlers = selectItemAlignedHandlers.get(wrapper);

  if (!handlers) {
    registerSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir);
    return;
  }

  const state = handlers.state;
  const canReuseRegistration =
    state.content === content &&
    state.viewport === viewport &&
    state.trigger === trigger &&
    state.valueNode === valueNode &&
    state.selectedItem === selectedItem &&
    state.selectedItemText === selectedItemText;

  if (!canReuseRegistration) {
    registerSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir);
    return;
  }

  state.dir = dir;
  handlers.updateNow();
}

export function unregisterSelectItemAlignedPosition(wrapper) {
  const handlers = selectItemAlignedHandlers.get(wrapper);
  if (!handlers) {
    return;
  }

  if (handlers.state.animationFrameId) {
    cancelAnimationFrame(handlers.state.animationFrameId);
  }

  handlers.resizeObserver.disconnect();
  window.removeEventListener("resize", handlers.update);
  window.removeEventListener("scroll", handlers.update, true);
  selectItemAlignedHandlers.delete(wrapper);
}

function clampValue(value, min, max) {
  return Math.min(Math.max(value, min), max);
}

function queueItemAlignedPositionFromViewport(viewport, content, wrapper) {
  if (!viewport || !content || !(wrapper instanceof HTMLElement) || wrapper.getAttribute("data-radix-select-position") !== "item-aligned") {
    return;
  }

  requestAnimationFrame(() => {
    const trigger = content.id
      ? document.querySelector(`[role='combobox'][aria-controls='${CSS.escape(content.id)}']`)
      : null;
    const valueNode = trigger ? trigger.querySelector("span") : null;
    const selectedItem = content.querySelector("[role='option'][data-highlighted]")
      || content.querySelector("[role='option'][data-state='checked']")
      || content.querySelector("[role='option']:not([data-disabled])");
    const selectedItemText = selectedItem
      ? document.getElementById(selectedItem.getAttribute("aria-labelledby") || "")
      : null;

    if (!trigger || !valueNode || !selectedItem || !selectedItemText) {
      return;
    }

    updateSelectItemAlignedPosition(
      wrapper,
      content,
      viewport,
      trigger,
      valueNode,
      selectedItem,
      selectedItemText,
      content.getAttribute("dir") || "ltr"
    );
  });
}

function positionSelectItemAligned(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir, preserveViewportScroll = false) {
  if (!wrapper || !content || !viewport || !trigger || !valueNode || !selectedItem || !selectedItemText) {
    return;
  }

  const CONTENT_MARGIN = 10;
  const triggerRect = trigger.getBoundingClientRect();
  const contentRect = content.getBoundingClientRect();
  const valueNodeRect = valueNode.getBoundingClientRect();
  const itemTextRect = selectedItemText.getBoundingClientRect();
  const isRtl = dir === "rtl";

  wrapper.style.position = "fixed";
  wrapper.style.display = "flex";
  wrapper.style.flexDirection = "column";
  wrapper.style.margin = `${CONTENT_MARGIN}px 0`;
  wrapper.style.top = "";
  wrapper.style.right = "";
  wrapper.style.bottom = "";
  wrapper.style.left = "";
  wrapper.style.zIndex = window.getComputedStyle(content).zIndex;

  if (!isRtl) {
    const itemTextOffset = itemTextRect.left - contentRect.left;
    const left = valueNodeRect.left - itemTextOffset;
    const leftDelta = triggerRect.left - left;
    const minContentWidth = Math.max(triggerRect.width, triggerRect.width + leftDelta);
    const contentWidth = Math.max(minContentWidth, contentRect.width);
    const rightEdge = window.innerWidth - CONTENT_MARGIN;
    const clampedLeft = clampValue(left, CONTENT_MARGIN, Math.max(CONTENT_MARGIN, rightEdge - contentWidth));
    wrapper.style.minWidth = `${minContentWidth}px`;
    content.style.minWidth = `${minContentWidth}px`;
    wrapper.style.left = `${clampedLeft}px`;
  } else {
    const itemTextOffset = contentRect.right - itemTextRect.right;
    const right = window.innerWidth - valueNodeRect.right - itemTextOffset;
    const rightDelta = window.innerWidth - triggerRect.right - right;
    const minContentWidth = Math.max(triggerRect.width, triggerRect.width + rightDelta);
    const contentWidth = Math.max(minContentWidth, contentRect.width);
    const leftEdge = window.innerWidth - CONTENT_MARGIN;
    const clampedRight = clampValue(right, CONTENT_MARGIN, Math.max(CONTENT_MARGIN, leftEdge - contentWidth));
    wrapper.style.minWidth = `${minContentWidth}px`;
    content.style.minWidth = `${minContentWidth}px`;
    wrapper.style.right = `${clampedRight}px`;
  }

  const availableHeight = window.innerHeight - CONTENT_MARGIN * 2;
  const itemsHeight = viewport.scrollHeight;
  const contentStyles = getComputedStyle(content);
  const contentBorderTopWidth = parseInt(contentStyles.borderTopWidth || "0", 10);
  const contentPaddingTop = parseInt(contentStyles.paddingTop || "0", 10);
  const contentBorderBottomWidth = parseInt(contentStyles.borderBottomWidth || "0", 10);
  const contentPaddingBottom = parseInt(contentStyles.paddingBottom || "0", 10);
  const fullContentHeight = contentBorderTopWidth + contentPaddingTop + itemsHeight + contentPaddingBottom + contentBorderBottomWidth;
  const minContentHeight = Math.min(selectedItem.offsetHeight * 5, fullContentHeight);

  const viewportStyles = getComputedStyle(viewport);
  const viewportPaddingTop = parseInt(viewportStyles.paddingTop || "0", 10);
  const viewportPaddingBottom = parseInt(viewportStyles.paddingBottom || "0", 10);

  const topEdgeToTriggerMiddle = triggerRect.top + triggerRect.height / 2 - CONTENT_MARGIN;
  const triggerMiddleToBottomEdge = availableHeight - topEdgeToTriggerMiddle;
  const selectedItemHalfHeight = selectedItem.offsetHeight / 2;
  const itemOffsetMiddle = selectedItem.offsetTop + selectedItemHalfHeight;
  const contentTopToItemMiddle = contentBorderTopWidth + contentPaddingTop + itemOffsetMiddle;
  const itemMiddleToContentBottom = fullContentHeight - contentTopToItemMiddle;
  const items = Array.from(viewport.querySelectorAll("[role='option']"));
  const willAlignWithoutTopOverflow = contentTopToItemMiddle <= topEdgeToTriggerMiddle;

  if (willAlignWithoutTopOverflow) {
    const isLastItem = items.length > 0 && selectedItem === items[items.length - 1];
    wrapper.style.bottom = "0px";
    const viewportOffsetBottom = content.clientHeight - viewport.offsetTop - viewport.offsetHeight;
    const clampedTriggerMiddleToBottomEdge = Math.max(
      triggerMiddleToBottomEdge,
      selectedItemHalfHeight + (isLastItem ? viewportPaddingBottom : 0) + viewportOffsetBottom + contentBorderBottomWidth
    );
    wrapper.style.height = `${contentTopToItemMiddle + clampedTriggerMiddleToBottomEdge}px`;
    wrapper.style.justifyContent = "";
  } else {
    const isFirstItem = items.length > 0 && selectedItem === items[0];
    wrapper.style.top = "0px";
    const clampedTopEdgeToTriggerMiddle = Math.max(
      topEdgeToTriggerMiddle,
      contentBorderTopWidth + viewport.offsetTop + (isFirstItem ? viewportPaddingTop : 0) + selectedItemHalfHeight
    );
    wrapper.style.height = `${clampedTopEdgeToTriggerMiddle + itemMiddleToContentBottom}px`;
    if (!preserveViewportScroll) {
      viewport.scrollTop = contentTopToItemMiddle - topEdgeToTriggerMiddle + viewport.offsetTop;
    }
  }

  wrapper.style.minHeight = `${minContentHeight}px`;
  wrapper.style.maxHeight = `${availableHeight}px`;
  content.style.boxSizing = "border-box";
  content.style.maxHeight = "100%";
}
