import { cssEscape } from "./core/dom.js";

const menubarDocumentDismissHandlers = new WeakMap();

export function registerMenubarDocumentDismiss(element, dotNetRef, menubarId) {
  if (!element || !dotNetRef || !menubarId) {
    return;
  }

  unregisterMenubarDocumentDismiss(element);

  const pointerdown = (event) => {
    const target = event.target;

    if (!(target instanceof Node)) {
      return;
    }

    if (element.contains(target)) {
      return;
    }

    const openContents = Array.from(
      document.querySelectorAll(`[data-radix-menubar-content][data-bradix-menubar-id="${cssEscape(menubarId)}"][data-state="open"]`)
    );

    if (openContents.length === 0) {
      return;
    }

    if (openContents.some((content) => content instanceof HTMLElement && content.contains(target))) {
      return;
    }

    dotNetRef.invokeMethodAsync("HandleDocumentPointerDownOutsideAsync").catch(() => {});
  };

  document.addEventListener("pointerdown", pointerdown, true);
  menubarDocumentDismissHandlers.set(element, { pointerdown });
}

export function unregisterMenubarDocumentDismiss(element) {
  if (!element) {
    return;
  }

  const handlers = menubarDocumentDismissHandlers.get(element);

  if (!handlers) {
    return;
  }

  document.removeEventListener("pointerdown", handlers.pointerdown, true);
  menubarDocumentDismissHandlers.delete(element);
}
