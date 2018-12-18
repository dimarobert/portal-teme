import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ModelErrors } from '../../http.models';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SettingsProvider } from '../../services/settings.provider';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-login-page',
  template: ''
})
export class LoginPageComponent implements OnInit {

  public serverError: ModelErrors;

  loginForm: FormGroup;

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


    // this.loginForm = new FormGroup({
    //   email: new FormControl('', Validators.email),
    //   password: new FormControl(''),
    //   rememberMe: new FormControl(false)
    // });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  get rememberMe() {
    return this.loginForm.get('rememberMe');
  }

  // login() {
  //   var user: LoginModel = {
  //     email: this.email.value,
  //     password: this.password.value,
  //     rememberMe: this.rememberMe.value
  //   };

  //   this.authSvc.login(user)
  //     .toPromise()
  //     .then(async response => {
  //       switch (response.status) {
  //         case AuthorizationStatus.Success:
  //           await this.settings.load();
  //           await this.navigateToRedirectUrl();
  //           break;

  //         case AuthorizationStatus.TwoFactorRequired:
  //           //TODO: Two Factor auth
  //           break;
  //       }
  //     })
  //     .catch((error: HttpErrorResponse) => {
  //       const authError: LoginResponse = error.error;
  //       switch (authError.status) {
  //         case AuthorizationStatus.LockedOut:
  //           this.serverError = { 'Error': ["Your account has been locked out."] };
  //           break;

  //         case AuthorizationStatus.InvalidCredentials:
  //           this.serverError = authError.errors;
  //           break;
  //       }
  //     });
  // }

  async navigateToRedirectUrl() {
    const returnUrl: string = this.route.snapshot.queryParams['returnUrl'] || '/';
    await this.router.navigateByUrl(returnUrl);
  }

}
