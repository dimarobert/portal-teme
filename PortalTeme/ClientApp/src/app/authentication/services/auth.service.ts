import { Injectable } from '@angular/core';
import { TokenService } from './token.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private tokenService: TokenService) { }

  public login(returnUrl: string) {
    let loginUrl = "/authentication/login";
    if (returnUrl)
      loginUrl = `${loginUrl}?returnUri=${encodeURIComponent(returnUrl)}`;
    location.href = loginUrl;
  }

  public isAuthenticated(): Observable<boolean> {
    return this.tokenService.getAccessToken()
      .pipe(
        map(result => result != null)
      );
  }
}
