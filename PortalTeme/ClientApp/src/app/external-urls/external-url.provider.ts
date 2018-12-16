import { InjectionToken, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';

export const externalUrlProvider = new InjectionToken('externalUrlRedirectResolver');

export function externalUrlRedirect(route: ActivatedRouteSnapshot) {
    const externalUrl = route.paramMap.get('externalUrl');
    location.href = externalUrl;
}