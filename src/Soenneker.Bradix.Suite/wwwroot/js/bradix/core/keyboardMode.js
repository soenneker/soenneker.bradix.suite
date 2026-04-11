let keyboardInteractionMode = false;
let keyboardInteractionTrackingInitialized = false;

function ensureKeyboardInteractionTracking() {
  if (keyboardInteractionTrackingInitialized || typeof document === "undefined") {
    return;
  }

  keyboardInteractionTrackingInitialized = true;

  document.addEventListener("keydown", () => {
    keyboardInteractionMode = true;
  }, { capture: true });

  const handlePointer = () => {
    keyboardInteractionMode = false;
  };

  document.addEventListener("pointerdown", handlePointer, { capture: true });
  document.addEventListener("pointermove", handlePointer, { capture: true });
}

export function isKeyboardInteractionMode() {
  ensureKeyboardInteractionTracking();
  return keyboardInteractionMode;
}
