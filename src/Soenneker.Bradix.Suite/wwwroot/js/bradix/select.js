const selectViewportHandlers = new WeakMap();
const selectContentPointerTrackers = new WeakMap();
const selectWindowDismissHandlers = new WeakMap();
const selectItemAlignedHandlers = new WeakMap();

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
    if (!wrapper) {
      return;
    }

    const itemAligned = selectItemAlignedHandlers.get(wrapper);
    if (!itemAligned || !itemAligned.state.shouldExpandOnScroll) {
      return;
    }

    const scrolledBy = Math.abs(itemAligned.state.previousScrollTop - viewport.scrollTop);
    if (scrolledBy <= 0) {
      itemAligned.state.previousScrollTop = viewport.scrollTop;
      return;
    }

    const CONTENT_MARGIN = 10;
    const availableHeight = window.innerHeight - CONTENT_MARGIN * 2;
    const cssMinHeight = parseFloat(wrapper.style.minHeight || "0");
    const cssHeight = parseFloat(wrapper.style.height || "0");
    const previousHeight = Math.max(cssMinHeight, cssHeight);

    if (previousHeight < availableHeight) {
      const nextHeight = previousHeight + scrolledBy;
      const clampedNextHeight = Math.min(availableHeight, nextHeight);
      const heightDiff = nextHeight - clampedNextHeight;

      wrapper.style.height = `${clampedNextHeight}px`;
      if (wrapper.style.bottom === "0px") {
        viewport.scrollTop = heightDiff > 0 ? heightDiff : 0;
        wrapper.style.justifyContent = "flex-end";
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
    const suppressSelection = withinPointerTolerance && targetInsideContent;
    const shouldClose = !withinPointerTolerance && !!target && !targetInsideContent;

    invokeDotNetSafely(dotNetRef, "HandleTriggerPointerGuardResult", suppressSelection, shouldClose);
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
  if (!wrapper || !content) {
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
    shouldExpandOnScroll: false,
    previousScrollTop: viewport ? viewport.scrollTop : 0
  };
  const update = () => positionSelectItemAligned(
    wrapper,
    state.content,
    state.viewport,
    state.trigger,
    state.valueNode,
    state.selectedItem,
    state.selectedItemText,
    state.dir
  );
  const resizeObserver = new ResizeObserver(update);

  resizeObserver.observe(content);
  resizeObserver.observe(viewport);
  resizeObserver.observe(trigger);
  resizeObserver.observe(selectedItem);
  resizeObserver.observe(selectedItemText);

  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, true);

  selectItemAlignedHandlers.set(wrapper, {
    update,
    resizeObserver,
    state
  });

  requestAnimationFrame(() => {
    update();
    state.previousScrollTop = viewport ? viewport.scrollTop : 0;
    requestAnimationFrame(() => {
      state.shouldExpandOnScroll = true;
    });
  });
}

export function updateSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir) {
  unregisterSelectItemAlignedPosition(wrapper);
  registerSelectItemAlignedPosition(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir);
}

export function unregisterSelectItemAlignedPosition(wrapper) {
  const handlers = selectItemAlignedHandlers.get(wrapper);
  if (!handlers) {
    return;
  }

  handlers.resizeObserver.disconnect();
  window.removeEventListener("resize", handlers.update);
  window.removeEventListener("scroll", handlers.update, true);
  selectItemAlignedHandlers.delete(wrapper);
}

function clampValue(value, min, max) {
  return Math.min(Math.max(value, min), max);
}

function positionSelectItemAligned(wrapper, content, viewport, trigger, valueNode, selectedItem, selectedItemText, dir) {
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
    const minContentWidth = triggerRect.width + leftDelta;
    const contentWidth = Math.max(minContentWidth, contentRect.width);
    const rightEdge = window.innerWidth - CONTENT_MARGIN;
    const clampedLeft = clampValue(left, CONTENT_MARGIN, Math.max(CONTENT_MARGIN, rightEdge - contentWidth));
    wrapper.style.minWidth = `${minContentWidth}px`;
    wrapper.style.left = `${clampedLeft}px`;
  } else {
    const itemTextOffset = contentRect.right - itemTextRect.right;
    const right = window.innerWidth - valueNodeRect.right - itemTextOffset;
    const rightDelta = window.innerWidth - triggerRect.right - right;
    const minContentWidth = triggerRect.width + rightDelta;
    const contentWidth = Math.max(minContentWidth, contentRect.width);
    const leftEdge = window.innerWidth - CONTENT_MARGIN;
    const clampedRight = clampValue(right, CONTENT_MARGIN, Math.max(CONTENT_MARGIN, leftEdge - contentWidth));
    wrapper.style.minWidth = `${minContentWidth}px`;
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
    viewport.scrollTop = contentTopToItemMiddle - topEdgeToTriggerMiddle + viewport.offsetTop;
  }

  wrapper.style.minHeight = `${minContentHeight}px`;
  wrapper.style.maxHeight = `${availableHeight}px`;
  content.style.boxSizing = "border-box";
  content.style.maxHeight = "100%";
}
