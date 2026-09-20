import {
  createDismissablePointerSnapshot,
  createDismissableKeyboardSnapshot,
  createDismissableFocusSnapshot
} from "./core/eventSnapshots.js";

const dismissableBranches = new Set();
const dismissableLayers = [];
const dismissableLayerUnregistrations = new Map();
let dismissableLayerListenersRegistered = false;
let dismissableLayerPointerDownListenerRegistered = false;
let originalDismissableBodyPointerEvents = "";
let hasStoredDismissableBodyPointerEvents = false;

function invokeDotNetSafely(layer, methodName, ...args) {
  if (!isRegisteredLayer(layer)) {
    return Promise.resolve(undefined);
  }

  let invocation;
  try {
    invocation = Promise.resolve(layer.dotNetRef?.invokeMethodAsync?.(methodName, ...args))
      .catch(() => undefined);
  } catch {
    return Promise.resolve(undefined);
  }

  layer.pendingInvocations.add(invocation);
  invocation.then(() => layer.pendingInvocations.delete(invocation));
  return invocation;
}

function isRegisteredLayer(layer) {
  return !!layer && dismissableLayers.includes(layer);
}

function isBranchTarget(target, layer) {
  if (!target) {
    return false;
  }

  for (const branch of dismissableBranches) {
    if (branch.contains(target) && (!layer || branch.contains(layer.element) || layer.element?.contains?.(branch))) {
      return true;
    }
  }

  return false;
}

function updateDismissableLayerPointerEvents() {
  let highestDisabledIndex = -1;

  for (let index = dismissableLayers.length - 1; index >= 0; index -= 1) {
    if (dismissableLayers[index].disableOutsidePointerEvents) {
      highestDisabledIndex = index;
      break;
    }
  }

  if (highestDisabledIndex >= 0) {
    if (!hasStoredDismissableBodyPointerEvents) {
      originalDismissableBodyPointerEvents = document.body.style.pointerEvents || "";
      hasStoredDismissableBodyPointerEvents = true;
    }

    document.body.style.pointerEvents = "none";
  } else if (hasStoredDismissableBodyPointerEvents) {
    document.body.style.pointerEvents = originalDismissableBodyPointerEvents;
    originalDismissableBodyPointerEvents = "";
    hasStoredDismissableBodyPointerEvents = false;
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

  dismissableBranches.forEach((branch) => {
    branch.style.pointerEvents = highestDisabledIndex >= 0 ? "auto" : "";
  });
}

function ensureDismissableLayerListeners() {
  if (dismissableLayerListenersRegistered) {
    return;
  }

  if (!dismissableLayerPointerDownListenerRegistered) {
    const dispatchPointerDownOutside = (topLayer, event) => {
      if (!isRegisteredLayer(topLayer)) {
        return;
      }

      const snapshot = createDismissablePointerSnapshot(event);
      snapshot.activeElementInsideLayer = !!(document.activeElement && topLayer.element.contains(document.activeElement));
      invokeDotNetSafely(topLayer, "HandlePointerDownOutside", snapshot);
    };

    const handlePointerDownLikeEvent = (event) => {
      const topLayer = dismissableLayers[dismissableLayers.length - 1];

      if (!topLayer || !event.target) {
        return;
      }

      // Match Radix more closely by only enabling outside pointer handling
      // after the gesture that mounted the layer has fully completed.
      if (!topLayer.isPointerDownOutsideEnabled) {
        topLayer.isPointerInside = false;
        return;
      }

      const isInsideLayer = !!topLayer.element?.contains?.(event.target);

      if (isInsideLayer || isBranchTarget(event.target, topLayer)) {
        topLayer.isPointerInside = false;
        return;
      }

      dispatchPointerDownOutside(topLayer, event);
      topLayer.isPointerInside = false;
    };

    document.addEventListener("pointerdown", handlePointerDownLikeEvent, true);

    dismissableLayerPointerDownListenerRegistered = true;
  }

  document.addEventListener("focusin", (event) => {
    const topLayer = dismissableLayers[dismissableLayers.length - 1];

    if (!topLayer || !event.target) {
      return;
    }

    if (!topLayer.isFocusOutsideEnabled) {
      return;
    }

    if (topLayer.isFocusInside) {
      return;
    }

    if (isBranchTarget(event.target, topLayer)) {
      return;
    }

    invokeDotNetSafely(topLayer, "HandleFocusOutside", createDismissableFocusSnapshot(event));
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
      return;
    }

    const topLayer = dismissableLayers[dismissableLayers.length - 1];
    if (!topLayer) {
      return;
    }

    invokeDotNetSafely(topLayer, "HandleEscapeKeyDown", createDismissableKeyboardSnapshot(event)).then((shouldPreventDefault) => {
      if (shouldPreventDefault) {
        event.preventDefault();
      }
    });
  }, true);

  dismissableLayerListenersRegistered = true;
}

export function registerDismissableLayer(element, dotNetRef, disableOutsidePointerEvents, registrationId) {
  if (!element) {
    return;
  }

  const previousUnregistration = unregisterDismissableLayer(element, registrationId);
  ensureDismissableLayerListeners();

  const handlePointerDownCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isPointerInside = true;
    }
  };
  const handleFocusInCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isFocusInside = true;
    }
  };
  const handleFocusOutCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isFocusInside = false;
    }
  };

  element.addEventListener("pointerdown", handlePointerDownCapture, true);
  element.addEventListener("focusin", handleFocusInCapture, true);
  element.addEventListener("focusout", handleFocusOutCapture, true);

  dismissableLayers.push({
    element,
    registrationId,
    dotNetRef,
    pendingInvocations: new Set(previousUnregistration ? [previousUnregistration] : []),
    disableOutsidePointerEvents: !!disableOutsidePointerEvents,
    isPointerInside: false,
    isFocusInside: false,
    isPointerDownOutsideEnabled: true,
    isFocusOutsideEnabled: true,
    handlePointerDownCapture,
    handleFocusInCapture,
    handleFocusOutCapture
  });
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

export function unregisterDismissableLayer(element, registrationId) {
  const key = registrationId || element;
  const index = dismissableLayers.findIndex((item) => registrationId ? item.registrationId === registrationId : item.element === element);

  if (index < 0) {
    return dismissableLayerUnregistrations.get(key);
  }

  const [layer] = dismissableLayers.splice(index, 1);
  element = layer.element;

  if (element) {
    element.removeEventListener("pointerdown", layer.handlePointerDownCapture, true);
    element.removeEventListener("focusin", layer.handleFocusInCapture, true);
    element.removeEventListener("focusout", layer.handleFocusOutCapture, true);
    element.style.pointerEvents = "";
  }

  updateDismissableLayerPointerEvents();

  // The component awaits unregister before disposing its DotNetObjectReference.
  // Stop new events immediately, then let already-dispatched callbacks finish.
  const completion = Promise.all(layer.pendingInvocations).then(() => {
    if (dismissableLayerUnregistrations.get(key) === completion) {
      dismissableLayerUnregistrations.delete(key);
    }
  });
  dismissableLayerUnregistrations.set(key, completion);
  return completion;
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
