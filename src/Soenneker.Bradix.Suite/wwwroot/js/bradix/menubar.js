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

    const selector = `[data-radix-menubar-content][data-bradix-menubar-id="${cssEscape(menubarId)}"][data-state="open"]`;
    const openContent = document.querySelector(selector);

    if (!(openContent instanceof HTMLElement)) {
      return;
    }

    const targetElement =
      target instanceof Element
        ? target
        : target.parentElement;

    if (targetElement?.closest(selector)) {
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
