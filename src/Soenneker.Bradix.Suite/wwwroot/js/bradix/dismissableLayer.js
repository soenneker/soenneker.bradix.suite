import {
  createDismissablePointerSnapshot,
  createDismissableKeyboardSnapshot,
  createDismissableFocusSnapshot
} from "./core/eventSnapshots.js";

const dismissableBranches = new Set();
const dismissableLayers = [];
let dismissableLayerListenersRegistered = false;
let dismissableLayerPointerDownListenerRegistered = false;
let dismissableLayerPointerDownRegistrationScheduled = false;
let originalDismissableBodyPointerEvents = "";
let hasStoredDismissableBodyPointerEvents = false;

function updateDismissableLayerPointerEvents() {
  const highestDisabledIndex = [...dismissableLayers].map((layer) => layer.disableOutsidePointerEvents).lastIndexOf(true);

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

  if (!dismissableLayerPointerDownListenerRegistered && !dismissableLayerPointerDownRegistrationScheduled) {
    dismissableLayerPointerDownRegistrationScheduled = true;
    window.setTimeout(() => {
      document.addEventListener("pointerdown", (event) => {
        const topLayer = dismissableLayers[dismissableLayers.length - 1];

        if (!topLayer || !event.target) {
          return;
        }

        if (topLayer.isPointerInside) {
          topLayer.isPointerInside = false;
          return;
        }

        for (const branch of dismissableBranches) {
          if (branch.contains(event.target)) {
            return;
          }
        }

        const dispatchPointerDownOutside = () => {
          const snapshot = createDismissablePointerSnapshot(event);
          snapshot.activeElementInsideLayer = !!(document.activeElement && topLayer.element.contains(document.activeElement));
          topLayer.dotNetRef.invokeMethodAsync("HandlePointerDownOutsideAsync", snapshot).catch(() => {});
        };

        if (event.pointerType === "touch") {
          if (topLayer.handleDocumentClick) {
            document.removeEventListener("click", topLayer.handleDocumentClick);
          }

          topLayer.handleDocumentClick = dispatchPointerDownOutside;
          document.addEventListener("click", topLayer.handleDocumentClick, { once: true });
        } else {
          if (topLayer.handleDocumentClick) {
            document.removeEventListener("click", topLayer.handleDocumentClick);
            topLayer.handleDocumentClick = null;
          }

          dispatchPointerDownOutside();
        }
      });
      dismissableLayerPointerDownListenerRegistered = true;
      dismissableLayerPointerDownRegistrationScheduled = false;
    }, 0);
  }

  document.addEventListener("focusin", (event) => {
    const topLayer = dismissableLayers[dismissableLayers.length - 1];

    if (!topLayer || !event.target) {
      return;
    }

    if (topLayer.isFocusInside) {
      return;
    }

    for (const branch of dismissableBranches) {
      if (branch.contains(event.target)) {
        return;
      }
    }

    topLayer.dotNetRef.invokeMethodAsync("HandleFocusOutsideAsync", createDismissableFocusSnapshot(event)).catch(() => {});
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
      return;
    }

    const topLayer = dismissableLayers[dismissableLayers.length - 1];
    if (!topLayer) {
      return;
    }

    topLayer.dotNetRef.invokeMethodAsync("HandleEscapeKeyDownAsync", createDismissableKeyboardSnapshot(event)).then((shouldPreventDefault) => {
      if (shouldPreventDefault) {
        event.preventDefault();
      }
    }).catch(() => {});
  }, true);

  dismissableLayerListenersRegistered = true;
}

export function registerDismissableLayer(element, dotNetRef, disableOutsidePointerEvents) {
  if (!element) {
    return;
  }

  unregisterDismissableLayer(element);
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
    dotNetRef,
    disableOutsidePointerEvents: !!disableOutsidePointerEvents,
    isPointerInside: false,
    isFocusInside: false,
    handlePointerDownCapture,
    handleFocusInCapture,
    handleFocusOutCapture,
    handleDocumentClick: null
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

export function unregisterDismissableLayer(element) {
  const index = dismissableLayers.findIndex((item) => item.element === element);

  if (index < 0) {
    return;
  }

  const [layer] = dismissableLayers.splice(index, 1);

  if (layer?.handleDocumentClick) {
    document.removeEventListener("click", layer.handleDocumentClick);
  }

  if (element) {
    element.removeEventListener("pointerdown", layer.handlePointerDownCapture, true);
    element.removeEventListener("focusin", layer.handleFocusInCapture, true);
    element.removeEventListener("focusout", layer.handleFocusOutCapture, true);
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
