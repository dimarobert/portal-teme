import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private auth: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    if (route.data.roles)
      return of(this.checkIfInRole(route.data.roles));
    else
      return this.checkIfAuthenticated(state);
  }

  checkIfInRole(roles: string[]): boolean {
    for (let role of roles) {
      if (this.auth.isInRole(role))
        return true;
    }
    return false;
  }

  private checkIfAuthenticated(state: RouterStateSnapshot): Observable<boolean> {
    return this.auth.isAuthenticated()
      .pipe(map(isAuth => {
        if (!isAuth)
          this.router.navigate(['login'], { queryParams: { returnUrl: state.url } });
        return isAuth;
      }));
  }
}
