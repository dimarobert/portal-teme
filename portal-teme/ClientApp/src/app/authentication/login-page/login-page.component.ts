import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthorizationStatus, LoginResponse } from './login.model';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {

  public email: string;
  public password: string;
  public rememberMe: boolean;

  public serverError: string;

  constructor(private route: ActivatedRoute, private authSvc: AuthService, private location: Location) { }

  ngOnInit() { }

  login() {
    this.authSvc.login({
      email: this.email,
      password: this.password,
      rememberMe: this.rememberMe
    }).subscribe(response => {
      switch (response.status) {
        case AuthorizationStatus.Success:
          let returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
          this.location.go(returnUrl);
          break;

        case AuthorizationStatus.TwoFactorRequired:
          //TODO: Two Factor auth
          break;

      }
    }, (error: HttpErrorResponse) => {
      const authError: LoginResponse = error.error;
      switch (authError.status) {

        case AuthorizationStatus.LockedOut:
          this.serverError = "Your account has been locked out.";
          break;

        case AuthorizationStatus.InvalidCredentials:
          this.serverError = authError.message;
          break;
      }
    });
  }

}
