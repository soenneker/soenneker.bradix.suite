const scrollAreaRootHandlers = new WeakMap();
const scrollAreaViewportHandlers = new WeakMap();
const scrollAreaScrollbarHandlers = new WeakMap();

function invokeDotNetSafe(dotNetRef, methodName, ...args) {
  dotNetRef.invokeMethodAsync(methodName, ...args).catch(() => {});
}

export function registerScrollAreaRoot(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterScrollAreaRoot(element);

  const pointerenter = () => {
    invokeDotNetSafe(dotNetRef, "HandleHoverChanged", true);
  };

  const pointerleave = () => {
    invokeDotNetSafe(dotNetRef, "HandleHoverChanged", false);
  };

  element.addEventListener("pointerenter", pointerenter);
  element.addEventListener("pointerleave", pointerleave);
  scrollAreaRootHandlers.set(element, { pointerenter, pointerleave });
}

export function unregisterScrollAreaRoot(element) {
  const handlers = scrollAreaRootHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("pointerenter", handlers.pointerenter);
  element.removeEventListener("pointerleave", handlers.pointerleave);
  scrollAreaRootHandlers.delete(element);
}

export function registerScrollAreaViewport(viewport, content, dotNetRef) {
  if (!viewport) {
    return;
  }

  unregisterScrollAreaViewport(viewport);

  const notify = () => {
    const contentElement = content || viewport.firstElementChild;
    invokeDotNetSafe(
      dotNetRef,
      "HandleViewportMetricsChanged",
      viewport.scrollLeft,
      viewport.scrollTop,
      contentElement ? contentElement.scrollWidth : viewport.scrollWidth,
      contentElement ? contentElement.scrollHeight : viewport.scrollHeight,
      viewport.offsetWidth,
      viewport.offsetHeight
    );
  };

  const scroll = () => notify();
  viewport.addEventListener("scroll", scroll);

  const viewportResizeObserver = new ResizeObserver(() => {
    requestAnimationFrame(notify);
  });
  viewportResizeObserver.observe(viewport);

  let contentResizeObserver = null;
  if (content) {
    contentResizeObserver = new ResizeObserver(() => {
      requestAnimationFrame(notify);
    });
    contentResizeObserver.observe(content);
  }

  requestAnimationFrame(notify);
  scrollAreaViewportHandlers.set(viewport, { scroll, viewportResizeObserver, contentResizeObserver, content });
}

export function unregisterScrollAreaViewport(viewport) {
  const handlers = scrollAreaViewportHandlers.get(viewport);

  if (!handlers) {
    return;
  }

  viewport.removeEventListener("scroll", handlers.scroll);
  handlers.viewportResizeObserver.disconnect();

  if (handlers.contentResizeObserver) {
    handlers.contentResizeObserver.disconnect();
  }

  scrollAreaViewportHandlers.delete(viewport);
}

export function registerScrollAreaScrollbar(scrollbar, thumb, viewport, orientation, dir, dotNetRef) {
  if (!scrollbar || !viewport) {
    return;
  }

  unregisterScrollAreaScrollbar(scrollbar);
  const thumbElement = thumb instanceof HTMLElement ? thumb : null;

  const getPaddingValue = (style, property) => {
    const raw = style ? style[property] : "0";
    const value = Number.parseInt(raw || "0", 10);
    return Number.isFinite(value) ? value : 0;
  };

  const notify = () => {
    const style = getComputedStyle(scrollbar);
    const paddingStart = orientation === "horizontal"
      ? getPaddingValue(style, "paddingLeft")
      : getPaddingValue(style, "paddingTop");
    const paddingEnd = orientation === "horizontal"
      ? getPaddingValue(style, "paddingRight")
      : getPaddingValue(style, "paddingBottom");

    invokeDotNetSafe(
      dotNetRef,
      "HandleScrollbarMetricsChanged",
      orientation,
      scrollbar.clientWidth,
      scrollbar.clientHeight,
      paddingStart,
      paddingEnd
    );
  };

  const getThumbSize = () => {
    if (thumbElement) {
      return orientation === "horizontal" ? thumbElement.offsetWidth : thumbElement.offsetHeight;
    }

    return 0;
  };

  const getScrollPosition = (pointerPosition, pointerOffset) => {
    const style = getComputedStyle(scrollbar);
    const paddingStart = orientation === "horizontal"
      ? getPaddingValue(style, "paddingLeft")
      : getPaddingValue(style, "paddingTop");
    const paddingEnd = orientation === "horizontal"
      ? getPaddingValue(style, "paddingRight")
      : getPaddingValue(style, "paddingBottom");
    const thumbSize = getThumbSize();
    const scrollbarSize = orientation === "horizontal" ? scrollbar.clientWidth : scrollbar.clientHeight;
    const viewportSize = orientation === "horizontal" ? viewport.offsetWidth : viewport.offsetHeight;
    const contentSize = orientation === "horizontal" ? viewport.scrollWidth : viewport.scrollHeight;
    const maxScrollPos = contentSize - viewportSize;
    const thumbCenter = thumbSize / 2;
    const offset = pointerOffset || thumbCenter;
    const thumbOffsetFromEnd = thumbSize - offset;
    const minPointerPos = paddingStart + offset;
    const maxPointerPos = scrollbarSize - paddingEnd - thumbOffsetFromEnd;

    if (maxPointerPos <= minPointerPos || maxScrollPos <= 0) {
      return 0;
    }

    const ratio = (pointerPosition - minPointerPos) / (maxPointerPos - minPointerPos);
    const clamped = Math.min(1, Math.max(0, ratio));

    if (orientation === "horizontal" && dir === "rtl") {
      return -maxScrollPos + clamped * maxScrollPos;
    }

    return clamped * maxScrollPos;
  };

  const isScrollingWithinScrollbarBounds = (scrollPos, maxScrollPos) => {
    return scrollPos > 0 && scrollPos < maxScrollPos;
  };

  let activePointerOffset = 0;
  let activePointerId = null;
  let previousBodyWebkitUserSelect = "";
  let previousViewportScrollBehavior = "";
  const pointerdown = (event) => {
    if (event.button !== 0) {
      return;
    }

    event.preventDefault();

    const targetThumb = thumbElement &&
      event.target instanceof Node &&
      thumbElement.contains(event.target)
      ? thumbElement
      : null;

    if (targetThumb) {
      const thumbRect = targetThumb.getBoundingClientRect();
      activePointerOffset = orientation === "horizontal"
        ? event.clientX - thumbRect.left
        : event.clientY - thumbRect.top;
    } else {
      activePointerOffset = 0;
    }

    activePointerId = typeof event.pointerId === "number" ? event.pointerId : null;
    previousBodyWebkitUserSelect = document.body.style.webkitUserSelect || "";
    previousViewportScrollBehavior = viewport.style.scrollBehavior || "";
    document.body.style.webkitUserSelect = "none";
    viewport.style.scrollBehavior = "auto";

    if (activePointerId !== null && typeof scrollbar.setPointerCapture === "function") {
      try {
        scrollbar.setPointerCapture(activePointerId);
      } catch {
      }
    }

    const scrollbarRect = scrollbar.getBoundingClientRect();
    const pointerPosition = orientation === "horizontal"
      ? event.clientX - scrollbarRect.left
      : event.clientY - scrollbarRect.top;

    const nextScrollPosition = getScrollPosition(pointerPosition, activePointerOffset);

    if (orientation === "horizontal") {
      viewport.scrollLeft = nextScrollPosition;
    } else {
      viewport.scrollTop = nextScrollPosition;
    }

    const pointermove = (moveEvent) => {
      const movePosition = orientation === "horizontal"
        ? moveEvent.clientX - scrollbarRect.left
        : moveEvent.clientY - scrollbarRect.top;
      const moveScrollPosition = getScrollPosition(movePosition, activePointerOffset);

      if (orientation === "horizontal") {
        viewport.scrollLeft = moveScrollPosition;
      } else {
        viewport.scrollTop = moveScrollPosition;
      }
    };

    const pointerup = () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
      document.body.style.webkitUserSelect = previousBodyWebkitUserSelect;
      viewport.style.scrollBehavior = previousViewportScrollBehavior;

      if (activePointerId !== null &&
        typeof scrollbar.hasPointerCapture === "function" &&
        typeof scrollbar.releasePointerCapture === "function") {
        try {
          if (scrollbar.hasPointerCapture(activePointerId)) {
            scrollbar.releasePointerCapture(activePointerId);
          }
        } catch {
        }
      }

      activePointerId = null;
    };

    document.addEventListener("pointermove", pointermove);
    document.addEventListener("pointerup", pointerup);

    const handlers = scrollAreaScrollbarHandlers.get(scrollbar);
    if (handlers) {
      handlers.pointermove = pointermove;
      handlers.pointerup = pointerup;
    }
  };

  const wheel = (event) => {
    const target = event.target;
    const isScrollbarWheel = target instanceof HTMLElement && scrollbar.contains(target);
    if (!isScrollbarWheel) {
      return;
    }

    const maxScrollPos = orientation === "horizontal"
      ? viewport.scrollWidth - viewport.offsetWidth
      : viewport.scrollHeight - viewport.offsetHeight;
    const nextScrollPos = orientation === "horizontal"
      ? viewport.scrollLeft + event.deltaX
      : viewport.scrollTop + event.deltaY;

    if (orientation === "horizontal") {
      viewport.scrollLeft = nextScrollPos;
    } else {
      viewport.scrollTop = nextScrollPos;
    }

    if (isScrollingWithinScrollbarBounds(nextScrollPos, maxScrollPos)) {
      event.preventDefault();
    }
  };

  const resizeObserver = new ResizeObserver(() => {
    requestAnimationFrame(notify);
  });
  resizeObserver.observe(scrollbar);
  if (thumbElement) {
    resizeObserver.observe(thumbElement);
  }

  requestAnimationFrame(notify);
  scrollbar.addEventListener("pointerdown", pointerdown);
  document.addEventListener("wheel", wheel, { passive: false });
  scrollAreaScrollbarHandlers.set(scrollbar, { pointerdown, pointermove: null, pointerup: null, wheel, resizeObserver });
}

export function unregisterScrollAreaScrollbar(scrollbar) {
  const handlers = scrollAreaScrollbarHandlers.get(scrollbar);

  if (!handlers) {
    return;
  }

  scrollbar.removeEventListener("pointerdown", handlers.pointerdown);
  handlers.resizeObserver.disconnect();
  document.removeEventListener("wheel", handlers.wheel, { passive: false });

  if (handlers.pointermove) {
    document.removeEventListener("pointermove", handlers.pointermove);
  }

  if (handlers.pointerup) {
    document.removeEventListener("pointerup", handlers.pointerup);
  }

  scrollAreaScrollbarHandlers.delete(scrollbar);
}
