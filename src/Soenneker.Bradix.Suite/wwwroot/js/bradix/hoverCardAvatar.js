const hoverCardSelectionHandlers = new WeakMap();
const avatarImageLoaders = new WeakMap();

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
  let active = false;

  const restoreSelection = () => {
    if (!active) {
      return;
    }

    active = false;
    content.style.userSelect = previousUserSelect;
    content.style.webkitUserSelect = previousWebkitUserSelect;
    document.body.style.userSelect = originalBodyUserSelect;
    document.body.style.webkitUserSelect = originalBodyUserSelect;
  };

  const handlePointerUp = () => {
    if (!active) {
      return;
    }

    restoreSelection();

    setTimeout(() => {
      const hasSelection = (document.getSelection()?.toString() || "") !== "";
      dotNetRef.invokeMethodAsync("HandleDocumentPointerUp", hasSelection);
    });
  };

  document.addEventListener("pointerup", handlePointerUp);
  hoverCardSelectionHandlers.set(content, {
    handlePointerUp,
    begin() {
      if (active) {
        return;
      }

      originalBodyUserSelect = document.body.style.userSelect || document.body.style.webkitUserSelect;
      document.body.style.userSelect = "none";
      document.body.style.webkitUserSelect = "none";
      content.style.userSelect = "text";
      content.style.webkitUserSelect = "text";
      active = true;
    },
    restoreSelection
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

  document.removeEventListener("pointerup", handlers.handlePointerUp);
  handlers.restoreSelection();
  hoverCardSelectionHandlers.delete(content);
}

export function registerAvatarImageLoadingStatus(src, crossOrigin, referrerPolicy, dotNetRef) {
  if (!dotNetRef) {
    return;
  }

  unregisterAvatarImageLoadingStatus(dotNetRef);

  const image = new window.Image();
  const notifyStatus = (status) => {
    setTimeout(() => {
      dotNetRef.invokeMethodAsync("HandleImageLoadingStatusChanged", status).catch(() => {});
    }, 0);
  };
  const handleLoad = () => {
    notifyStatus("loaded");
  };
  const handleError = () => {
    notifyStatus("error");
  };

  image.addEventListener("load", handleLoad);
  image.addEventListener("error", handleError);

  if (referrerPolicy) {
    image.referrerPolicy = referrerPolicy;
  }

  if (typeof crossOrigin === "string" && crossOrigin.length > 0) {
    image.crossOrigin = crossOrigin;
  }

  avatarImageLoaders.set(dotNetRef, { image, handleLoad, handleError });

  if (!src) {
    notifyStatus("error");
    return;
  }

  image.src = src;

  if (image.complete && image.naturalWidth > 0) {
    notifyStatus("loaded");
    return;
  }

  notifyStatus("loading");
}

export function unregisterAvatarImageLoadingStatus(dotNetRef) {
  const handlers = avatarImageLoaders.get(dotNetRef);
  if (!handlers) {
    return;
  }

  handlers.image.removeEventListener("load", handlers.handleLoad);
  handlers.image.removeEventListener("error", handlers.handleError);
  avatarImageLoaders.delete(dotNetRef);
}
