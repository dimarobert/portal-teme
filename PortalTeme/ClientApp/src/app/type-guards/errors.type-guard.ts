import { HttpErrorResponse } from '@angular/common/http';

export function isHttpErrorResponse(error: any): error is HttpErrorResponse {
    return error.name === 'HttpErrorResponse';
}
