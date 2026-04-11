import { isPointInPolygon } from "./core/geometry.js";

const menuSubmenuGraceHandlers = new WeakMap();

export function beginMenuSubmenuPointerGrace(trigger, content, clientX, clientY, dotNetRef) {
  if (!trigger || !content || !dotNetRef) {
    return false;
  }

  cancelMenuSubmenuPointerGrace(trigger);

  const contentSideElement = content.closest("[data-side]");
  const side = contentSideElement instanceof HTMLElement
    ? contentSideElement.dataset.side
    : null;

  if (!side) {
    return false;
  }

  const contentRect = content.getBoundingClientRect();
  if (!contentRect || (contentRect.width <= 0 && contentRect.height <= 0)) {
    return false;
  }

  const rightSide = side === "right";
  const intendedDirection = rightSide ? "right" : "left";
  const bleed = rightSide ? -5 : 5;
  const contentNearEdge = contentRect[rightSide ? "left" : "right"];
  const contentFarEdge = contentRect[rightSide ? "right" : "left"];
  let lastClientX = clientX;
  const pointerGraceArea = [
    { x: clientX + bleed, y: clientY },
    { x: contentNearEdge, y: contentRect.top },
    { x: contentFarEdge, y: contentRect.top },
    { x: contentFarEdge, y: contentRect.bottom },
    { x: contentNearEdge, y: contentRect.bottom }
  ];

  const cleanup = () => {
    const handlers = menuSubmenuGraceHandlers.get(trigger);

    if (!handlers) {
      return;
    }

    if (handlers.pointerMove) {
      document.removeEventListener("pointermove", handlers.pointerMove);
    }

    if (handlers.timeoutId) {
      window.clearTimeout(handlers.timeoutId);
    }

    menuSubmenuGraceHandlers.delete(trigger);
    dotNetRef.invokeMethodAsync("HandlePointerGraceChangedAsync", false).catch(() => {});
  };

  const pointerMove = (event) => {
    const target = event.target;
    const pointerPosition = { x: event.clientX, y: event.clientY };
    const pointerXHasChanged = event.clientX !== lastClientX;
    const pointerDirection = pointerXHasChanged
      ? (event.clientX > lastClientX ? "right" : "left")
      : null;
    const isMovingTowardsSubmenu = pointerDirection === null || pointerDirection === intendedDirection;
    const hasEnteredTarget =
      (trigger instanceof HTMLElement && trigger.contains(target)) ||
      (content instanceof HTMLElement && content.contains(target));
    const isPointerOutsideGraceArea = !isPointInPolygon(pointerPosition, pointerGraceArea);

    lastClientX = event.clientX;

    if (!isMovingTowardsSubmenu || hasEnteredTarget || isPointerOutsideGraceArea) {
      cleanup();
    }
  };

  const timeoutId = window.setTimeout(() => {
    cleanup();
  }, 300);

  menuSubmenuGraceHandlers.set(trigger, { pointerMove, timeoutId });
  document.addEventListener("pointermove", pointerMove);
  dotNetRef.invokeMethodAsync("HandlePointerGraceChangedAsync", true).catch(() => {});
  return true;
}

export function cancelMenuSubmenuPointerGrace(trigger) {
  const handlers = menuSubmenuGraceHandlers.get(trigger);

  if (!handlers) {
    return;
  }

  if (handlers.pointerMove) {
    document.removeEventListener("pointermove", handlers.pointerMove);
  }

  if (handlers.timeoutId) {
    window.clearTimeout(handlers.timeoutId);
  }

  menuSubmenuGraceHandlers.delete(trigger);
}
