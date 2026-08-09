let keyboardInteractionMode = false;
let keyboardInteractionTrackingInitialized = false;

function ensureKeyboardInteractionTracking() {
  if (keyboardInteractionTrackingInitialized || typeof document === "undefined") {
    return;
  }

  keyboardInteractionTrackingInitialized = true;

  const handlePointerMove = () => {
    keyboardInteractionMode = false;
  };

  document.addEventListener("keydown", () => {
    keyboardInteractionMode = true;
    document.addEventListener("pointermove", handlePointerMove, { capture: true, once: true });
  }, { capture: true });

  document.addEventListener("pointerdown", () => {
    keyboardInteractionMode = false;
    document.removeEventListener("pointermove", handlePointerMove, true);
  }, { capture: true });
}

export function isKeyboardInteractionMode() {
  ensureKeyboardInteractionTracking();
  return keyboardInteractionMode;
}
