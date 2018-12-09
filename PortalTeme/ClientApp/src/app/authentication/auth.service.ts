import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterModel, RegisterResponse } from './register-page/register.model';
import { LoginModel, LoginResponse } from './login-page/login.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  public register(model: RegisterModel) {
    return this.http.post<RegisterResponse>('/api/authentication/register', model);
  }

  public login(model: LoginModel){
    return this.http.post<LoginResponse>('/api/authentication/login', model);
  }

  public logout(){
    return this.http.post('/api/authentication/logout', {});
  }
}
