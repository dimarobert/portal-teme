import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';
import { flatMap } from 'rxjs/operators';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

  private unauthenticatedApis = new UnauthenticatedApiList([
    {
      method: 'GET',
      url: '/authentication/token'
    }
  ]);

  constructor(private tokenService: TokenService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.unauthenticatedApis.contains(req))
      return next.handle(req);

    return this.tokenService.getAccessToken()
      .pipe(
        flatMap(authToken => {
          if (authToken == null)
            return next.handle(req);

          const authenticatedRequest = req.clone({
            headers: req.headers.set('Authorization', `Bearer ${authToken}`)
          });

          return next.handle(authenticatedRequest);

        })
      );
  }

}

class UnauthenticatedApiList {

  private checker = new UnauthenticatedApiChecker();

  constructor(private apiList: UnauthenticatedApi[]) { }

  contains(req: HttpRequest<any>): boolean {
    for (let api of this.apiList) {
      if (this.checker.check(api, req))
        return true;
    }

    return false;
  }

}

class UnauthenticatedApiChecker {

  check(apiOpts: UnauthenticatedApi, req: HttpRequest<any>): boolean {
    if (this.isFuncCheck(apiOpts)) {
      return apiOpts.checkRequest(req);
    } else {

      let atLeastOneCheck = false;

      if (apiOpts.method != null) {
        atLeastOneCheck = true;
        if (req.method.toUpperCase() !== apiOpts.method.toUpperCase())
          return false;
      }

      if (apiOpts.url != null) {
        atLeastOneCheck = true;

        const lowerRequestUrl = req.url.toLowerCase();
        const lowerUrl = apiOpts.url.toLowerCase();
        if (!this.checkUrl(lowerRequestUrl, lowerUrl, apiOpts.urlCheck))
          return false;
      }

      return atLeastOneCheck && true;
    }
  }

  private isFuncCheck(apiOpts: UnauthenticatedApi): apiOpts is UnauthenticatedApiFunc {
    return !!(apiOpts as any).checkRequest;
  }

  private checkUrl(lowerRequestUrl: string, lowerUrl: string, urlCheck?: UrlCheckOption): boolean {

    if (urlCheck == null)
      return lowerRequestUrl === lowerUrl;

    switch (urlCheck) {
      case 'equal':
        return lowerRequestUrl === lowerUrl;

      case 'startsWith':
        return lowerRequestUrl.startsWith(lowerUrl);

      case 'contains':
        return lowerRequestUrl.indexOf(lowerUrl) > -1;

    }

    return false;
  }
}

type UrlCheckOption = 'equal' | 'startsWith' | 'contains';

type UnauthenticatedApi = UnauthenticatedApiProps | UnauthenticatedApiFunc;

class UnauthenticatedApiProps {
  method: string;

  url: string;
  urlCheck?: UrlCheckOption;
}

class UnauthenticatedApiFunc {
  checkRequest: (req: HttpRequest<any>) => boolean;
}