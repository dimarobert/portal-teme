import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FormGroup, FormControl, ValidatorFn, Validators } from '@angular/forms';
import { ModelErrors } from '../../http.models';
import { RegisterResponse, RegisterStatus } from './register.model';
import { SettingsProvider } from '../../services/settings.provider';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent implements OnInit {

  public serverErrors: ModelErrors;

  registerForm: FormGroup;

  constructor(private route: ActivatedRoute, private authSvc: AuthService, private settings: SettingsProvider, private router: Router) { }

  ngOnInit() {
    this.settings.isUserAuthenticated$
      .pipe(take(1))
      .toPromise()
      .then(async isAuth => {
        if (isAuth)
          await this.navigateToRedirectUrl();
      });

    this.registerForm = new FormGroup({
      email: new FormControl('', Validators.email),
      passwordGroup: new FormGroup({
        password: new FormControl(''),
        confirmPassword: new FormControl('')
      }, {
          validators: compareWith('password', 'confirmPassword')
        })
    });
  }

  get email() {
    return this.registerForm.get('email');
  }

  get passwordGroup() {
    return this.registerForm.get('passwordGroup');
  }

  get password() {
    return this.passwordGroup.get('password');
  }

  get confirmPassword() {
    return this.passwordGroup.get('confirmPassword');
  }

  register() {
    this.authSvc.register({
      email: this.email.value,
      password: this.password.value,
      confirmPassword: this.confirmPassword.value
    }).subscribe(async response => {
      await this.navigateToRedirectUrl();
    }, (error: HttpErrorResponse) => {
      let regError: RegisterResponse = error.error;
      switch (regError.status) {
        case RegisterStatus.Error:
          this.serverErrors = regError.errors;
          break;
      }
    });
  }

  async navigateToRedirectUrl() {
    const returnUrl: string = this.route.snapshot.queryParams['returnUrl'] || '/';
    await this.router.navigateByUrl(returnUrl);
  }
}

export function compareWith(targetControlName: string, checkControlName: string): ValidatorFn {
  return (group: FormGroup): { [key: string]: any } | null => {
    const target = group.controls[targetControlName];
    const comparand = group.controls[checkControlName];

    if (target.dirty && comparand.dirty) {
      const isMatch = target.value === comparand.value;
      if (!isMatch && target.valid && comparand.valid) {
        comparand.setErrors({ 'compareWith': targetControlName });
        return { 'compareWith': checkControlName };
      }

      if (isMatch && comparand.hasError('compareWith')) {
        comparand.setErrors(null);
      }
    }

    return null;
  };
}
