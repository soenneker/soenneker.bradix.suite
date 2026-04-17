import { getAncestorIds } from "./dom.js";

export function createDelegatedEventSnapshot(type, event) {
  if (type === "click" || type === "mousedown" || type === "pointerdown" || type === "mouseover" || type === "mouseenter" || type === "pointermove" || type === "pointerover") {
    return {
      button: typeof event.button === "number" ? event.button : 0,
      pointerId: typeof event.pointerId === "number" ? event.pointerId : 0,
      ctrlKey: !!event.ctrlKey,
      shiftKey: !!event.shiftKey,
      altKey: !!event.altKey,
      metaKey: !!event.metaKey,
      detail: typeof event.detail === "number" ? event.detail : 0,
      pageX: typeof event.pageX === "number" ? event.pageX : 0,
      pageY: typeof event.pageY === "number" ? event.pageY : 0,
      pointerType: typeof event.pointerType === "string" ? event.pointerType : "",
      defaultPrevented: !!event.defaultPrevented
    };
  }

  if (type === "focusin" || type === "focusout") {
    const target = event.target instanceof HTMLElement ? event.target : null;
    const relatedTarget = event.relatedTarget instanceof HTMLElement ? event.relatedTarget : null;
    return {
      defaultPrevented: !!event.defaultPrevented,
      targetId: target && target.id ? target.id : "",
      ancestorIds: getAncestorIds(target),
      relatedTargetId: relatedTarget && relatedTarget.id ? relatedTarget.id : "",
      relatedTargetAncestorIds: getAncestorIds(relatedTarget)
    };
  }

  if (type === "keydown") {
    const target = event.target instanceof HTMLElement ? event.target : null;
    const closestMenubarContent = target && typeof target.closest === "function"
      ? target.closest("[data-radix-menubar-content]")
      : null;

    return {
      key: event.key || "",
      code: event.code || "",
      ctrlKey: !!event.ctrlKey,
      shiftKey: !!event.shiftKey,
      altKey: !!event.altKey,
      metaKey: !!event.metaKey,
      repeat: !!event.repeat,
      defaultPrevented: !!event.defaultPrevented,
      targetId: target && target.id ? target.id : "",
      ancestorIds: getAncestorIds(target),
      closestMenubarContentId: closestMenubarContent instanceof HTMLElement && closestMenubarContent.id ? closestMenubarContent.id : "",
      isMenubarSubTrigger: !!(target && target.hasAttribute("data-radix-menubar-subtrigger"))
    };
  }

  return {
    defaultPrevented: !!event.defaultPrevented
  };
}

export function createDismissablePointerSnapshot(event) {
  const target = event.target instanceof HTMLElement ? event.target : null;
  return {
    button: typeof event.button === "number" ? event.button : 0,
    ctrlKey: !!event.ctrlKey,
    shiftKey: !!event.shiftKey,
    altKey: !!event.altKey,
    metaKey: !!event.metaKey,
    detail: typeof event.detail === "number" ? event.detail : 0,
    defaultPrevented: !!event.defaultPrevented,
    targetId: target && target.id ? target.id : "",
    ancestorIds: getAncestorIds(target),
    activeElementInsideLayer: false
  };
}

export function createDismissableKeyboardSnapshot(event) {
  return {
    key: event.key || "",
    code: event.code || "",
    ctrlKey: !!event.ctrlKey,
    shiftKey: !!event.shiftKey,
    altKey: !!event.altKey,
    metaKey: !!event.metaKey,
    repeat: !!event.repeat,
    defaultPrevented: !!event.defaultPrevented
  };
}

export function createDismissableFocusSnapshot(event) {
  const target = event.target instanceof HTMLElement ? event.target : null;
  return {
    defaultPrevented: !!event.defaultPrevented,
    targetId: target && target.id ? target.id : "",
    ancestorIds: getAncestorIds(target)
  };
}
