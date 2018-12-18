import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { AntiforgeryService } from './antiforgery.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private antiforgery: AntiforgeryService) { }

  public login(returnUrl: string) {
    let loginUrl = "/authentication/login";
    if (returnUrl)
      loginUrl = `${loginUrl}?returnUri=${encodeURIComponent(returnUrl)}`;
    location.href = loginUrl;
  }

  public logout() {
    return this.http.post('/authentication/logout', {});
  }

  private cachedAccessToken: string = null;

  public async getAccessToken(refresh: boolean = false) {
    if (!refresh && this.cachedAccessToken)
      return this.cachedAccessToken;

    const antiforgeryToken = this.antiforgery.getAntiforgeryToken();

    let headers = {};
    headers[this.antiforgery.getHeaderName()] = antiforgeryToken;

    const response: HttpResponse<any> = await this.http.get('/authentication/token', {
      observe: 'response',
      headers: new HttpHeaders(headers)
    }).toPromise();

    this.cachedAccessToken = response.headers.get('AccessToken') || null;
    return this.cachedAccessToken;
  }
}
