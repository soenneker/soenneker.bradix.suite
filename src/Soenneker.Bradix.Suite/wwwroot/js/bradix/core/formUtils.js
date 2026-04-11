export function resolveAssociatedForm(element, formId = null) {
  if (!element) {
    return null;
  }

  if (formId && typeof document !== "undefined") {
    const form = document.getElementById(formId);
    if (form instanceof HTMLFormElement) {
      return form;
    }
  }

  return element.form || (typeof element.closest === "function" ? element.closest("form") : null);
}

export function isInvalidFormControl(element) {
  return !!element && "validity" in element && (
    (element.validity && element.validity.valid === false) ||
    element.getAttribute("aria-invalid") === "true"
  );
}

export function createFormValiditySnapshot(element) {
  const validity = element && element.validity ? element.validity : null;

  return {
    badInput: !!validity?.badInput,
    customError: !!validity?.customError,
    patternMismatch: !!validity?.patternMismatch,
    rangeOverflow: !!validity?.rangeOverflow,
    rangeUnderflow: !!validity?.rangeUnderflow,
    stepMismatch: !!validity?.stepMismatch,
    tooLong: !!validity?.tooLong,
    tooShort: !!validity?.tooShort,
    typeMismatch: !!validity?.typeMismatch,
    valid: validity ? !!validity.valid : true,
    valueMissing: !!validity?.valueMissing,
    validationMessage: element?.validationMessage || ""
  };
}

export function serializeFormDataSnapshot(form) {
  const values = {};

  if (!form || typeof FormData !== "function") {
    return { values };
  }

  const formData = new FormData(form);

  for (const [key, value] of formData.entries()) {
    if (!values[key]) {
      values[key] = [];
    }

    values[key].push(typeof value === "string" ? value : "");
  }

  return { values };
}
