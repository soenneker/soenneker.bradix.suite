const tabbableFilter = {
    acceptNode: (node) => {
      const isHiddenInput = node.tagName === "INPUT" && node.type === "hidden";
      if (node.closest("[inert], [hidden]")) {
        return NodeFilter.FILTER_REJECT;
      }
      if (node.matches(":disabled") || isHiddenInput || node.hasAttribute("data-radix-focus-guard")) {
        return NodeFilter.FILTER_SKIP;
      }

      return node.tabIndex >= 0 ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP;
    }
  };

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
  const walker = document.createTreeWalker(container, NodeFilter.SHOW_ELEMENT, tabbableFilter);

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

export function getTabbableEdges(container) {
  const walker = document.createTreeWalker(container, NodeFilter.SHOW_ELEMENT, tabbableFilter);
  let first;
  while ((first = walker.nextNode())) {
    if (!isHidden(first, container)) break;
  }
  if (!first) return [null, null];

  // Walk directly to the final accepted descendant. Large menus and dialogs
  // need only their edges, not an array containing every focusable control.
  walker.currentNode = container;
  let last = walker.lastChild();
  while (walker.lastChild()) last = walker.currentNode;
  while (last && last !== first && isHidden(last, container)) last = walker.previousNode();
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

export function focusFirst(candidates, select, startIndex = 0, step = 1) {
  const previous = document.activeElement;
  for (let index = startIndex; index >= 0 && index < candidates.length; index += step) {
    const candidate = candidates[index];
    focusElement(candidate, select);
    if (document.activeElement !== previous) {
      return true;
    }
  }

  return false;
}

export function removeLinks(items) {
  let writeIndex = 0;
  for (let index = 0; index < items.length; index++) {
    const item = items[index];
    if (item.tagName !== "A") {
      items[writeIndex++] = item;
    }
  }

  items.length = writeIndex;
  return items;
}
