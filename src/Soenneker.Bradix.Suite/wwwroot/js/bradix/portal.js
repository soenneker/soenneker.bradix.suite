const portalMounts = new WeakMap();

export function mountPortal(element, containerSelector, containerElement) {
  if (!(element instanceof Node)) {
    return;
  }

  unmountPortal(element);

  const parent = element.parentNode;
  const nextSibling = element.nextSibling;
  const container = containerElement instanceof HTMLElement
    ? containerElement
    : containerSelector
      ? document.querySelector(containerSelector)
      : document.body;

  if (!(container instanceof Node)) {
    return;
  }

  container.appendChild(element);
  portalMounts.set(element, { parent, nextSibling });
}

export function unmountPortal(element) {
  if (!(element instanceof Node)) {
    return;
  }

  const mount = portalMounts.get(element);

  if (!mount) {
    return;
  }

  if (mount.parent) {
    if (mount.nextSibling && mount.nextSibling.parentNode === mount.parent) {
      mount.parent.insertBefore(element, mount.nextSibling);
    } else {
      mount.parent.appendChild(element);
    }
  }

  portalMounts.delete(element);
}
