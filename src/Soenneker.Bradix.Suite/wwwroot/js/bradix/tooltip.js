import {
  getExitSideFromRect,
  getPaddedExitPoints,
  getPointsFromRect,
  getHull,
  isPointInPolygon
} from "./core/geometry.js";

const tooltipTriggerHandlers = new WeakMap();
const tooltipContentHandlers = new WeakMap();
const TOOLTIP_OPEN_EVENT = "tooltip.open";

export function registerTooltipTrigger(element, dotNetRef) {
  if (!element || !dotNetRef) {
    return;
  }

  unregisterTooltipTrigger(element);

  const pointerUp = () => {
    dotNetRef.invokeMethodAsync("HandleDocumentPointerUp").catch(() => {});
  };

  document.addEventListener("pointerup", pointerUp);
  tooltipTriggerHandlers.set(element, { pointerUp });
}

export function unregisterTooltipTrigger(element) {
  const handlers = tooltipTriggerHandlers.get(element);

  if (!handlers) {
    return;
  }

  document.removeEventListener("pointerup", handlers.pointerUp);
  tooltipTriggerHandlers.delete(element);
}

export function dispatchTooltipOpen(contentId) {
  document.dispatchEvent(new CustomEvent(TOOLTIP_OPEN_EVENT, {
    detail: { contentId: contentId || "" }
  }));
}

export function registerTooltipContent(content, trigger, dotNetRef, contentId, hoverableContent) {
  if (!content || !trigger || !dotNetRef) {
    return;
  }

  unregisterTooltipContent(content);

  const tooltipOpen = (event) => {
    if (event.detail && event.detail.contentId === contentId) {
      return;
    }

    dotNetRef.invokeMethodAsync("HandleTooltipOpenFromOutside").catch(() => {});
  };

  const scroll = (event) => {
    const target = event.target;

    if (target instanceof HTMLElement && target.contains(trigger)) {
      dotNetRef.invokeMethodAsync("HandleTooltipTriggerScroll").catch(() => {});
    }
  };

  document.addEventListener(TOOLTIP_OPEN_EVENT, tooltipOpen);
  window.addEventListener("scroll", scroll, { capture: true });

  const state = {
    trigger,
    tooltipOpen,
    scroll,
    triggerLeave: null,
    contentLeave: null,
    pointerMove: null,
    pointerGraceArea: null
  };

  if (hoverableContent) {
    const removeGraceArea = () => {
      state.pointerGraceArea = null;

      if (state.pointerMove) {
        document.removeEventListener("pointermove", state.pointerMove);
        state.pointerMove = null;
      }

      dotNetRef.invokeMethodAsync("HandlePointerGraceAreaChanged", false).catch(() => {});
    };

    const createGraceArea = (event, hoverTarget) => {
      const currentTarget = event.currentTarget;

      if (!(currentTarget instanceof HTMLElement) || !(hoverTarget instanceof HTMLElement)) {
        return;
      }

      const exitPoint = { x: event.clientX, y: event.clientY };
      const exitSide = getExitSideFromRect(exitPoint, currentTarget.getBoundingClientRect());
      const paddedExitPoints = getPaddedExitPoints(exitPoint, exitSide);
      const hoverTargetPoints = getPointsFromRect(hoverTarget.getBoundingClientRect());
      state.pointerGraceArea = getHull([...paddedExitPoints, ...hoverTargetPoints]);

      dotNetRef.invokeMethodAsync("HandlePointerGraceAreaChanged", true).catch(() => {});

      if (state.pointerMove) {
        document.removeEventListener("pointermove", state.pointerMove);
      }

      state.pointerMove = (moveEvent) => {
        const target = moveEvent.target;
        const pointerPosition = { x: moveEvent.clientX, y: moveEvent.clientY };
        const hasEnteredTarget =
          (trigger instanceof HTMLElement && trigger.contains(target)) ||
          (content instanceof HTMLElement && content.contains(target));
        const isPointerOutsideGraceArea =
          state.pointerGraceArea && !isPointInPolygon(pointerPosition, state.pointerGraceArea);

        if (hasEnteredTarget) {
          removeGraceArea();
        } else if (isPointerOutsideGraceArea) {
          removeGraceArea();
          dotNetRef.invokeMethodAsync("HandleTooltipGraceAreaExit").catch(() => {});
        }
      };

      document.addEventListener("pointermove", state.pointerMove);
    };

    state.triggerLeave = (event) => createGraceArea(event, content);
    state.contentLeave = (event) => createGraceArea(event, trigger);

    trigger.addEventListener("pointerleave", state.triggerLeave);
    content.addEventListener("pointerleave", state.contentLeave);
  }

  tooltipContentHandlers.set(content, state);
}

export function unregisterTooltipContent(content) {
  const handlers = tooltipContentHandlers.get(content);

  if (!handlers) {
    return;
  }

  document.removeEventListener(TOOLTIP_OPEN_EVENT, handlers.tooltipOpen);
  window.removeEventListener("scroll", handlers.scroll, { capture: true });

  if (handlers.triggerLeave && handlers.trigger) {
    handlers.trigger.removeEventListener("pointerleave", handlers.triggerLeave);
  }

  if (handlers.contentLeave) {
    content.removeEventListener("pointerleave", handlers.contentLeave);
  }

  if (handlers.pointerMove) {
    document.removeEventListener("pointermove", handlers.pointerMove);
  }

  tooltipContentHandlers.delete(content);
}
