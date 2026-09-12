const hoverCardSelectionHandlers = new WeakMap();

export function disableHoverCardContentTabNavigation(content) {
  if (!content) {
    return;
  }

  const walker = document.createTreeWalker(content, NodeFilter.SHOW_ELEMENT, {
    acceptNode: (node) => node.tabIndex >= 0 ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP
  });

  while (walker.nextNode()) {
    walker.currentNode.setAttribute("tabindex", "-1");
  }
}

export function registerHoverCardSelectionContainment(content, dotNetRef) {
  if (!content || !dotNetRef) {
    return;
  }

  unregisterHoverCardSelectionContainment(content);

  const previousUserSelect = content.style.userSelect;
  const previousWebkitUserSelect = content.style.webkitUserSelect;
  let originalBodyUserSelect = "";
  let originalBodyWebkitUserSelect = "";
  let active = false;
  let disposed = false;
  let releaseTimer = 0;

  const restoreSelection = () => {
    if (!active) {
      return;
    }

    active = false;
    document.removeEventListener("pointerup", handlePointerUp);
    document.removeEventListener("pointercancel", handlePointerUp);
    content.style.userSelect = previousUserSelect;
    content.style.webkitUserSelect = previousWebkitUserSelect;
    document.body.style.userSelect = originalBodyUserSelect;
    document.body.style.webkitUserSelect = originalBodyWebkitUserSelect;
  };

  const notifyReleased = () => {
    releaseTimer = 0;
    if (disposed || active) return;
    const hasSelection = (document.getSelection()?.toString() || "") !== "";
    dotNetRef.invokeMethodAsync("HandleDocumentPointerUp", hasSelection).catch(console.error);
  };

  const handlePointerUp = () => {
    if (!active) {
      return;
    }

    restoreSelection();

    releaseTimer = setTimeout(notifyReleased);
  };

  hoverCardSelectionHandlers.set(content, {
    begin() {
      if (active || disposed) {
        return;
      }

      if (releaseTimer) clearTimeout(releaseTimer);
      releaseTimer = 0;
      originalBodyUserSelect = document.body.style.userSelect;
      originalBodyWebkitUserSelect = document.body.style.webkitUserSelect;
      document.body.style.userSelect = "none";
      document.body.style.webkitUserSelect = "none";
      content.style.userSelect = "text";
      content.style.webkitUserSelect = "text";
      active = true;
      document.addEventListener("pointerup", handlePointerUp);
      document.addEventListener("pointercancel", handlePointerUp);
    },
    dispose() {
      disposed = true;
      if (releaseTimer) clearTimeout(releaseTimer);
      releaseTimer = 0;
      restoreSelection();
    }
  });
}

export function beginHoverCardSelectionContainment(content) {
  const handlers = hoverCardSelectionHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.begin();
}

export function unregisterHoverCardSelectionContainment(content) {
  const handlers = hoverCardSelectionHandlers.get(content);
  if (!handlers) {
    return;
  }

  handlers.dispose();
  hoverCardSelectionHandlers.delete(content);
}

export function registerAvatarImageLoadingStatus(src, crossOrigin, referrerPolicy, dotNetRef) {
}

export function unregisterAvatarImageLoadingStatus(dotNetRef) {
}
