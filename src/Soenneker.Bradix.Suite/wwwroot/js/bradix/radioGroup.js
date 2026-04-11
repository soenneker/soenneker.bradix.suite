const radioGroupItemHandlers = new WeakMap();

export function registerRadioGroupItemKeys(element) {
  if (!element) {
    return;
  }

  unregisterRadioGroupItemKeys(element);

  const handler = (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", handler);
  radioGroupItemHandlers.set(element, handler);
}

export function unregisterRadioGroupItemKeys(element) {
  const handler = radioGroupItemHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("keydown", handler);
  radioGroupItemHandlers.delete(element);
}
