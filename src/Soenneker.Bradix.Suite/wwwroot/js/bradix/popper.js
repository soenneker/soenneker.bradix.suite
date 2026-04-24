const popperContentHandlers = new WeakMap();

function getFloatingUi() {
  const floating = globalThis.FloatingUIDOM;

  if (!floating) {
    throw new Error("Floating UI has not been loaded. BradixSuiteInterop must load FloatingUIDOM before Popper registration.");
  }

  return floating;
}

function getSideAndAlign(placement) {
  const [side, align = "center"] = placement.split("-");
  return [side, align];
}

function getPlacement(options) {
  const side = options.side || "bottom";
  const align = options.align || "center";
  return align === "center" ? side : `${side}-${align}`;
}

function getCollisionPadding(options) {
  const padding = options.collisionPadding ?? 0;

  if (typeof padding === "number") {
    return padding;
  }

  return {
    top: 0,
    right: 0,
    bottom: 0,
    left: 0,
    ...padding
  };
}

function alignToOrigin(align) {
  return {
    start: "0%",
    center: "50%",
    end: "100%"
  }[align] || "50%";
}

function transformOriginMiddleware(arrowWidth, arrowHeight) {
  return {
    name: "transformOrigin",
    options: { arrowWidth, arrowHeight },
    fn(data) {
      const { placement, rects, middlewareData } = data;
      const cannotCenterArrow = middlewareData.arrow?.centerOffset !== 0;
      const isArrowHidden = cannotCenterArrow;
      const resolvedArrowWidth = isArrowHidden ? 0 : arrowWidth;
      const resolvedArrowHeight = isArrowHidden ? 0 : arrowHeight;
      const [placedSide, placedAlign] = getSideAndAlign(placement);
      const noArrowAlign = alignToOrigin(placedAlign);
      const arrowXCenter = (middlewareData.arrow?.x ?? 0) + resolvedArrowWidth / 2;
      const arrowYCenter = (middlewareData.arrow?.y ?? 0) + resolvedArrowHeight / 2;
      let x = "";
      let y = "";

      if (placedSide === "bottom") {
        x = isArrowHidden ? noArrowAlign : `${arrowXCenter}px`;
        y = `${-resolvedArrowHeight}px`;
      } else if (placedSide === "top") {
        x = isArrowHidden ? noArrowAlign : `${arrowXCenter}px`;
        y = `${rects.floating.height + resolvedArrowHeight}px`;
      } else if (placedSide === "right") {
        x = `${-resolvedArrowHeight}px`;
        y = isArrowHidden ? noArrowAlign : `${arrowYCenter}px`;
      } else if (placedSide === "left") {
        x = `${rects.floating.width + resolvedArrowHeight}px`;
        y = isArrowHidden ? noArrowAlign : `${arrowYCenter}px`;
      }

      return { data: { x, y } };
    }
  };
}

async function updateRegisteredPopperContent(content) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers?.reference || !handlers.content) {
    return;
  }

  const floating = getFloatingUi();
  const options = handlers.options || {};
  const arrowRect = handlers.arrow?.getBoundingClientRect?.() || null;
  const arrowWidth = arrowRect?.width || 0;
  const arrowHeight = arrowRect?.height || 0;
  const collisionPadding = getCollisionPadding(options);
  let availableWidth = 0;
  let availableHeight = 0;
  let anchorWidth = 0;
  let anchorHeight = 0;

  const detectOverflowOptions = {
    padding: collisionPadding,
    boundary: [],
    altBoundary: false
  };

  const middleware = [
    floating.offset({
      mainAxis: Number(options.sideOffset || 0) + arrowHeight,
      alignmentAxis: Number(options.alignOffset || 0)
    })
  ];

  if (options.avoidCollisions !== false) {
    middleware.push(
      floating.shift({
        mainAxis: true,
        crossAxis: false,
        limiter: options.sticky === "always" ? undefined : floating.limitShift(),
        ...detectOverflowOptions
      }),
      floating.flip(detectOverflowOptions)
    );
  }

  middleware.push(
    floating.size({
      ...detectOverflowOptions,
      apply({ rects, availableWidth: width, availableHeight: height }) {
        availableWidth = width;
        availableHeight = height;
        anchorWidth = rects.reference.width;
        anchorHeight = rects.reference.height;
      }
    })
  );

  if (handlers.arrow) {
    middleware.push(floating.arrow({ element: handlers.arrow, padding: Number(options.arrowPadding || 0) }));
  }

  middleware.push(transformOriginMiddleware(arrowWidth, arrowHeight));

  if (options.hideWhenDetached === true) {
    middleware.push(floating.hide({ strategy: "referenceHidden", ...detectOverflowOptions }));
  }

  const position = await floating.computePosition(handlers.reference, handlers.content, {
    strategy: "fixed",
    placement: getPlacement(options),
    middleware
  });

  const [placedSide, placedAlign] = getSideAndAlign(position.placement);
  const arrowData = position.middlewareData.arrow;
  const transformOriginData = position.middlewareData.transformOrigin || {};
  const nextPosition = {
    placedSide,
    placedAlign,
    left: position.x,
    top: position.y,
    availableWidth,
    availableHeight,
    anchorWidth,
    anchorHeight,
    arrowX: arrowData?.x ?? null,
    arrowY: arrowData?.y ?? null,
    shouldHideArrow: arrowData?.centerOffset !== 0,
    hidden: options.hideWhenDetached === true && position.middlewareData.hide?.referenceHidden === true,
    transformOriginX: transformOriginData.x || "50%",
    transformOriginY: transformOriginData.y || "50%"
  };

  if (isSamePosition(handlers.lastPosition, nextPosition)) {
    return;
  }

  handlers.lastPosition = nextPosition;
  await handlers.dotNetRef.invokeMethodAsync(
    "HandlePositionChanged",
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

function isSamePosition(previous, next) {
  return previous &&
    previous.placedSide === next.placedSide &&
    previous.placedAlign === next.placedAlign &&
    previous.left === next.left &&
    previous.top === next.top &&
    previous.availableWidth === next.availableWidth &&
    previous.availableHeight === next.availableHeight &&
    previous.anchorWidth === next.anchorWidth &&
    previous.anchorHeight === next.anchorHeight &&
    previous.arrowX === next.arrowX &&
    previous.arrowY === next.arrowY &&
    previous.shouldHideArrow === next.shouldHideArrow &&
    previous.hidden === next.hidden &&
    previous.transformOriginX === next.transformOriginX &&
    previous.transformOriginY === next.transformOriginY;
}

function getAnchorRect(anchor) {
  const rect = anchor.getBoundingClientRect();

  if ((rect.width > 0 || rect.height > 0) || !anchor.firstElementChild) {
    return rect;
  }

  return anchor.firstElementChild.getBoundingClientRect();
}

function createAnchorReference(anchor) {
  return {
    contextElement: anchor,
    getBoundingClientRect() {
      return getAnchorRect(anchor);
    },
    getClientRects() {
      return anchor.getClientRects?.() ?? [];
    }
  };
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
    },
    getClientRects() {
      return [];
    }
  };
}

function observeAutoUpdate(handlers) {
  const floating = getFloatingUi();
  handlers.cleanup = floating.autoUpdate(handlers.reference, handlers.content, handlers.update, {
    animationFrame: handlers.options?.updatePositionStrategy === "always"
  });
}

function reconnect(content) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.cleanup?.();
  handlers.lastPosition = null;
  observeAutoUpdate(handlers);
}

export function registerPopperContent(anchor, content, arrow, dotNetRef, options) {
  if (!anchor || !content) {
    return;
  }

  unregisterPopperContent(content);

  const update = () => updateRegisteredPopperContent(content);
  const resolvedArrow = arrow instanceof Element ? arrow : null;

  popperContentHandlers.set(content, {
    anchor,
    reference: createAnchorReference(anchor),
    content,
    arrow: resolvedArrow,
    dotNetRef,
    options: options || {},
    cleanup: null,
    update,
    lastPosition: null
  });

  reconnect(content);
}

export function registerVirtualPopperContent(content, arrow, dotNetRef, x, y, options) {
  if (!content) {
    return;
  }

  unregisterPopperContent(content);

  const update = () => updateRegisteredPopperContent(content);
  const resolvedArrow = arrow instanceof Element ? arrow : null;

  popperContentHandlers.set(content, {
    anchor: null,
    reference: createVirtualAnchor(x, y),
    content,
    arrow: resolvedArrow,
    dotNetRef,
    options: options || {},
    cleanup: null,
    update,
    lastPosition: null
  });

  reconnect(content);
}

export function updatePopperContent(content, arrow, options) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.arrow = arrow instanceof Element ? arrow : null;
  handlers.options = options || {};
  reconnect(content);
}

export function updateVirtualPopperContent(content, arrow, x, y, options) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.reference = createVirtualAnchor(x, y);
  handlers.arrow = arrow instanceof Element ? arrow : null;
  handlers.options = options || {};
  reconnect(content);
}

export function unregisterPopperContent(content) {
  const handlers = popperContentHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.cleanup?.();
  popperContentHandlers.delete(content);
}
