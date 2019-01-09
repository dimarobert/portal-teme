import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SettingsProvider } from '../../services/settings.provider';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-login-page',
  template: ''
})
export class LoginPageComponent implements OnInit {


  constructor(private route: ActivatedRoute, private authSvc: AuthService, private settings: SettingsProvider, private router: Router) { }

  ngOnInit() {
    this.settings.isUserAuthenticated$
      .pipe(take(1))
      .toPromise()
      .then(async isAuth => {
        if (isAuth)
          return await this.navigateToRedirectUrl();

        const returnUrl: string = this.route.snapshot.queryParams['returnUrl'];
        this.authSvc.login(returnUrl);
      });
  }

  async navigateToRedirectUrl() {
    const returnUrl: string = this.route.snapshot.queryParams['returnUrl'] || '/';
    await this.router.navigateByUrl(returnUrl);
  }

}
