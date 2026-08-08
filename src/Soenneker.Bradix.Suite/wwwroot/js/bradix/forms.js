import {
  resolveAssociatedForm,
  isInvalidFormControl,
  createFormValiditySnapshot,
  serializeFormDataSnapshot
} from "./core/formUtils.js";

const checkboxRootHandlers = new WeakMap();
const associatedFormResetHandlers = new WeakMap();
const formRootHandlers = new WeakMap();

function isDisposedDotNetReferenceError(error) {
  const message = typeof error === "string" ? error : error?.message;

  return typeof message === "string" &&
    message.includes("There is no tracked object with id") &&
    message.includes("DotNetObjectReference") &&
    message.includes("disposed");
}

export function registerCheckboxRoot(element, dotNetRef, formId = null) {
  if (!element) {
    return;
  }

  unregisterCheckboxRoot(element);
  element.setAttribute("data-bradix-checkbox-root", "");

  const preventEnterActivation = (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", preventEnterActivation, true);
  element.addEventListener("keypress", preventEnterActivation, true);
  element.addEventListener("keyup", preventEnterActivation, true);

  const form = resolveAssociatedForm(element, formId);
  let reset = null;

  if (form && dotNetRef) {
    reset = () => {
      dotNetRef.invokeMethodAsync("HandleFormReset");
    };

    form.addEventListener("reset", reset);
  }

  checkboxRootHandlers.set(element, { preventEnterActivation, form, reset, dotNetRef });
}

export function unregisterCheckboxRoot(element) {
  const handlers = checkboxRootHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("keydown", handlers.preventEnterActivation, true);
  element.removeEventListener("keypress", handlers.preventEnterActivation, true);
  element.removeEventListener("keyup", handlers.preventEnterActivation, true);
  element.removeAttribute("data-bradix-checkbox-root");

  if (handlers.form && handlers.reset) {
    handlers.form.removeEventListener("reset", handlers.reset);
  }

  checkboxRootHandlers.delete(element);
}

export function registerFormRoot(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterFormRoot(element);

  const registration = {
    dotNetRef,
    invalidDispatchQueued: false
  };

  const dispatchInvalidControls = () => {
    registration.invalidDispatchQueued = false;

    if (formRootHandlers.get(element) !== registration) {
      return;
    }

    const controls = element.elements || [];
    const invalidControlNames = [];
    const seenControlNames = new Set();
    let firstInvalid = null;

    for (let index = 0; index < controls.length; index++) {
      const candidate = controls[index];

      if (!(candidate instanceof HTMLElement) || !isInvalidFormControl(candidate)) {
        continue;
      }

      firstInvalid ||= candidate;

      const name = candidate.getAttribute("name") || candidate.getAttribute("id") || "";
      if (name && !seenControlNames.has(name)) {
        seenControlNames.add(name);
        invalidControlNames.push(name);
      }
    }

    if (firstInvalid && typeof firstInvalid.focus === "function") {
      firstInvalid.focus();
    }

    if (registration.dotNetRef) {
      void registration.dotNetRef.invokeMethodAsync("HandleInvalidControls", invalidControlNames).catch((error) => {
        if (formRootHandlers.get(element) !== registration && isDisposedDotNetReferenceError(error)) {
          return;
        }

        throw error;
      });
    }
  };

  const invalid = (event) => {
    event.preventDefault();

    if (registration.invalidDispatchQueued) {
      return;
    }

    registration.invalidDispatchQueued = true;
    queueMicrotask(dispatchInvalidControls);
  };

  registration.invalid = invalid;
  element.addEventListener("invalid", invalid, true);
  formRootHandlers.set(element, registration);
}

export function unregisterFormRoot(element) {
  const handlers = formRootHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("invalid", handlers.invalid, true);
  formRootHandlers.delete(element);
}

export function getFormControlValidity(element) {
  if (!element || !("validity" in element) || !element.validity) {
    return createFormValiditySnapshot(null);
  }

  return createFormValiditySnapshot(element);
}

export function getFormControlState(element) {
  return {
    value: element && "value" in element ? element.value || "" : "",
    validity: getFormControlValidity(element),
    formData: serializeFormDataSnapshot(element ? (element.form || (typeof element.closest === "function" ? element.closest("form") : null)) : null)
  };
}

export function setFormControlCustomValidity(element, validationMessage) {
  if (!element || typeof element.setCustomValidity !== "function") {
    return;
  }

  element.setCustomValidity(validationMessage || "");
}

export function clearFormCustomValidity(form) {
  if (!form || !form.elements) {
    return;
  }

  Array.from(form.elements).forEach((element) => {
    if (element && typeof element.setCustomValidity === "function") {
      element.setCustomValidity("");
    }
  });
}

export function focusServerInvalidFormControl(element) {
  if (!element) {
    return false;
  }

  const form = element.form || (typeof element.closest === "function" ? element.closest("form") : null);
  if (!form) {
    return false;
  }

  const invalidElements = Array.from(form.elements || []).filter((candidate) => {
    return candidate instanceof HTMLElement && isInvalidFormControl(candidate);
  });

  const firstInvalid = invalidElements[0];

  if (firstInvalid === element && typeof element.focus === "function") {
    element.focus();
    return true;
  }

  return false;
}

export function registerAssociatedFormReset(element, dotNetRef, formId = null) {
  if (!element || !dotNetRef) {
    return;
  }

  unregisterAssociatedFormReset(element);

  const form = resolveAssociatedForm(element, formId);
  if (!form) {
    return;
  }

  const reset = () => {
    dotNetRef.invokeMethodAsync("HandleFormReset");
  };

  form.addEventListener("reset", reset);
  associatedFormResetHandlers.set(element, { form, reset });
}

export function unregisterAssociatedFormReset(element) {
  const handlers = associatedFormResetHandlers.get(element);
  if (!handlers) {
    return;
  }

  handlers.form.removeEventListener("reset", handlers.reset);
  associatedFormResetHandlers.delete(element);
}

export function isFormControl(element, formId = null) {
  if (!element) {
    return false;
  }

  return !!resolveAssociatedForm(element, formId);
}

export function requestFormSubmit(associatedElement, formId) {
  let form = null;

  if (formId) {
    const candidate = associatedElement?.ownerDocument?.getElementById(formId) || document.getElementById(formId);
    if (candidate instanceof HTMLFormElement) {
      form = candidate;
    }
  }

  form ||= associatedElement?.form || (typeof associatedElement?.closest === "function" ? associatedElement.closest("form") : null);

  if (form && typeof form.requestSubmit === "function") {
    form.requestSubmit();
  }
}
