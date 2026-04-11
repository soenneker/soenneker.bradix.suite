const popperContentHandlers = new WeakMap();

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
  const dir = options.dir === "rtl" ? "rtl" : "ltr";
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
      left = dir === "rtl"
        ? anchorRect.right - contentRect.width - alignOffset
        : anchorRect.left + alignOffset;
    } else if (align === "end") {
      left = dir === "rtl"
        ? anchorRect.left + alignOffset
        : anchorRect.right - contentRect.width + alignOffset;
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
  const nextPosition = {
    placedSide: position.placedSide,
    placedAlign: position.placedAlign,
    left: position.left,
    top: position.top,
    availableWidth: position.availableWidth,
    availableHeight: position.availableHeight,
    anchorWidth: position.anchorWidth,
    anchorHeight: position.anchorHeight,
    arrowX: position.arrowX,
    arrowY: position.arrowY,
    shouldHideArrow: position.shouldHideArrow,
    hidden: handlers.options.hideWhenDetached === true && position.referenceHidden,
    transformOriginX: position.transformOriginX,
    transformOriginY: position.transformOriginY
  };

  if (
    handlers.lastPosition &&
    handlers.lastPosition.placedSide === nextPosition.placedSide &&
    handlers.lastPosition.placedAlign === nextPosition.placedAlign &&
    handlers.lastPosition.left === nextPosition.left &&
    handlers.lastPosition.top === nextPosition.top &&
    handlers.lastPosition.availableWidth === nextPosition.availableWidth &&
    handlers.lastPosition.availableHeight === nextPosition.availableHeight &&
    handlers.lastPosition.anchorWidth === nextPosition.anchorWidth &&
    handlers.lastPosition.anchorHeight === nextPosition.anchorHeight &&
    handlers.lastPosition.arrowX === nextPosition.arrowX &&
    handlers.lastPosition.arrowY === nextPosition.arrowY &&
    handlers.lastPosition.shouldHideArrow === nextPosition.shouldHideArrow &&
    handlers.lastPosition.hidden === nextPosition.hidden &&
    handlers.lastPosition.transformOriginX === nextPosition.transformOriginX &&
    handlers.lastPosition.transformOriginY === nextPosition.transformOriginY
  ) {
    return;
  }

  handlers.lastPosition = nextPosition;
  handlers.dotNetRef.invokeMethodAsync(
    "HandlePositionChangedAsync",
    nextPosition.placedSide,
    nextPosition.placedAlign,
    nextPosition.left,
    nextPosition.top,
    nextPosition.availableWidth,
    nextPosition.availableHeight,
    nextPosition.anchorWidth,
    nextPosition.anchorHeight,
    nextPosition.arrowX,
    nextPosition.arrowY,
    nextPosition.shouldHideArrow,
    nextPosition.hidden,
    nextPosition.transformOriginX,
    nextPosition.transformOriginY
  );
}

function createVirtualAnchor(x, y) {
  return {
    getBoundingClientRect() {
      return DOMRect.fromRect({
        x,
        y,
        width: 0,
        height: 0
      });
    }
  };
}

export function registerPopperContent(anchor, content, arrow, dotNetRef, options) {
  if (!anchor || !content) {
    return;
  }

  unregisterPopperContent(content);

  const update = () => updateRegisteredPopperContent(content);
  const resizeObserver = new ResizeObserver(update);
  const resolvedArrow = arrow instanceof Element ? arrow : null;
  resizeObserver.observe(anchor);
  resizeObserver.observe(content);
  if (resolvedArrow) {
    resizeObserver.observe(resolvedArrow);
  }

  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, true);

  popperContentHandlers.set(content, {
    anchor,
    content,
    arrow: resolvedArrow,
    dotNetRef,
    options: options || {},
    resizeObserver,
    update,
    lastPosition: null
  });

  update();
}

export function registerVirtualPopperContent(content, arrow, dotNetRef, x, y, options) {
  if (!content) {
    return;
  }

  unregisterPopperContent(content);

  const update = () => updateRegisteredPopperContent(content);
  const resizeObserver = new ResizeObserver(update);
  const resolvedArrow = arrow instanceof Element ? arrow : null;
  resizeObserver.observe(content);
  if (resolvedArrow) {
    resizeObserver.observe(resolvedArrow);
  }

  window.addEventListener("resize", update);
  window.addEventListener("scroll", update, true);

  popperContentHandlers.set(content, {
    anchor: createVirtualAnchor(x, y),
    content,
    arrow: resolvedArrow,
    dotNetRef,
    options: options || {},
    resizeObserver,
    update,
    lastPosition: null
  });

  update();
}

export function updatePopperContent(content, arrow, options) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.arrow = arrow instanceof Element ? arrow : null;
  handlers.options = options || {};
  handlers.lastPosition = null;
  handlers.resizeObserver.disconnect();
  handlers.resizeObserver.observe(handlers.anchor);
  handlers.resizeObserver.observe(handlers.content);
  if (handlers.arrow) {
    handlers.resizeObserver.observe(handlers.arrow);
  }

  handlers.update();
}

export function updateVirtualPopperContent(content, arrow, x, y, options) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.anchor = createVirtualAnchor(x, y);
  handlers.arrow = arrow instanceof Element ? arrow : null;
  handlers.options = options || {};
  handlers.lastPosition = null;
  handlers.resizeObserver.disconnect();
  handlers.resizeObserver.observe(handlers.content);
  if (handlers.arrow) {
    handlers.resizeObserver.observe(handlers.arrow);
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
