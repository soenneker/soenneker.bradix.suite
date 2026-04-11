export function arrayRemove(array, item) {
  const next = [...array];
  const index = next.indexOf(item);
  if (index >= 0) {
    next.splice(index, 1);
  }
  return next;
}

export function getTabbableCandidates(container) {
  const nodes = [];
  const walker = document.createTreeWalker(container, NodeFilter.SHOW_ELEMENT, {
    acceptNode: (node) => {
      const isHiddenInput = node.tagName === "INPUT" && node.type === "hidden";
      if (node.disabled || node.hidden || isHiddenInput) {
        return NodeFilter.FILTER_SKIP;
      }

      return node.tabIndex >= 0 ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP;
    }
  });

  while (walker.nextNode()) {
    nodes.push(walker.currentNode);
  }

  return nodes;
}

function isHidden(node, upTo) {
  if (getComputedStyle(node).visibility === "hidden") {
    return true;
  }

  while (node) {
    if (upTo && node === upTo) {
      return false;
    }

    if (getComputedStyle(node).display === "none") {
      return true;
    }

    node = node.parentElement;
  }

  return false;
}

function findVisible(elements, container) {
  for (const element of elements) {
    if (!isHidden(element, container)) {
      return element;
    }
  }

  return null;
}

export function getTabbableEdges(container) {
  const candidates = getTabbableCandidates(container);
  const first = findVisible(candidates, container);
  const last = findVisible([...candidates].reverse(), container);
  return [first, last];
}

export function focusElement(element, select) {
  if (!element || typeof element.focus !== "function") {
    return;
  }

  const previous = document.activeElement;
  element.focus({ preventScroll: true });

  if (select && element !== previous && element instanceof HTMLInputElement && typeof element.select === "function") {
    element.select();
  }
}

export function focusFirst(candidates, select) {
  const previous = document.activeElement;
  for (const candidate of candidates) {
    focusElement(candidate, select);
    if (document.activeElement !== previous) {
      return true;
    }
  }

  return false;
}

export function removeLinks(items) {
  return items.filter((item) => item.tagName !== "A");
}
