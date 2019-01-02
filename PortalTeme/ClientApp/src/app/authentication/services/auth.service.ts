import { Injectable } from '@angular/core';
import { TokenService } from './token.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SettingsProvider } from '../../services/settings.provider';
import { UserSettings } from '../../models/application-settings.model';
import { AuthConstants } from '../constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private tokenService: TokenService, private settings: SettingsProvider) { }

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

  public isInRole$(role: string): Observable<boolean> {
    return this.settings.user$
      .pipe(map(user => this.isInRoleInternal(user, role)));
  }

  public isInRole(role: string): boolean {
    return this.isInRoleInternal(this.settings.user, role);
  }

  private isInRoleInternal(user: UserSettings, role: string): boolean {
    if (user == null)
      return false;

    return (user.roles.indexOf(role) > -1);
  }

  public canAddCourse(): Observable<boolean> {
    return this.settings.user$
      .pipe(map(user =>
        this.isInRoleInternal(user, AuthConstants.professorRoleName) ||
        this.isInRoleInternal(user, AuthConstants.adminRoleName)
      ));
  }
}
