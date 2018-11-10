import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { RegisterError } from './register.model';
import { NgForm, Validator, AbstractControl, ValidationErrors, FormGroup, FormControl, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent implements OnInit {

  public validationErrors: { key: string, value: string[] }[];

  registerForm: FormGroup;

  constructor(private route: ActivatedRoute, private authSvc: AuthService, private location: Location) { }

  ngOnInit() {

    this.registerForm = new FormGroup({
      email: new FormControl(''),
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
    }).subscribe(response => {
      let returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
      this.location.go(returnUrl);
    }, (error: HttpErrorResponse) => {
      let regError: RegisterError = error.error;
      this.validationErrors = Object.entries(regError).map(e => { return { key: e[0], value: e[1] }; });
    });
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
