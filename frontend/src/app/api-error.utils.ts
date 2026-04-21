import { HttpErrorResponse } from '@angular/common/http';

export function extractApiErrorMessage(error: unknown, fallbackMessage: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallbackMessage;
  }

  if (typeof error.error === 'string' && error.error.trim()) {
    return error.error;
  }

  if (error.error && typeof error.error === 'object' && 'errors' in error.error) {
    const validationErrors = error.error.errors;

    if (validationErrors && typeof validationErrors === 'object') {
      const firstMessage = Object.values(validationErrors)
        .flatMap((value) => Array.isArray(value) ? value : [])
        .find((value): value is string => typeof value === 'string' && value.trim().length > 0);

      if (firstMessage) {
        return firstMessage;
      }
    }
  }

  return fallbackMessage;
}
