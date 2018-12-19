import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule, ActivatedRouteSnapshot } from '@angular/router';

import { MaterialComponentsModule } from './modules/AngularMaterialImports/material-components.module';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LayoutModule } from '@angular/cdk/layout';
import { LoginPageComponent } from './authentication/login-page/login-page.component';
import { RegisterPageComponent } from './authentication/register-page/register-page.component';

import { NotFoundPageComponent } from './error-pages/not-found-page/not-found-page.component';
import { ErrorPageComponent } from './error-pages/error-page/error-page.component';
import { AccessDeniedPageComponent } from './error-pages/access-denied-page/access-denied-page.component';

import { ExternalUrlDirective } from './external-urls/external-url.directive';

import { KeysPipe } from './pipes/object-keys.pipe';

import { SettingsProvider, settingsProviderFactory } from './services/settings.provider';
import { externalUrlProvider, externalUrlRedirect } from './external-urls/external-url.provider';

import { AuthGuardService as AuthGuard } from './authentication/services/auth-guard.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginPageComponent,
    RegisterPageComponent,
    ErrorPageComponent,
    NotFoundPageComponent,
    AccessDeniedPageComponent,

    ExternalUrlDirective,

    KeysPipe,

  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    MaterialComponentsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutModule,
    // JwtModule.forRoot({
    //   jwtOptionsProvider: {
    //     provide: JWT_OPTIONS,
    //     useFactory: jwtOptionsFactory,
    //     deps: [TokenService]
    //   }
    // }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent, canActivate: [AuthGuard] },
      { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthGuard] },
      { path: 'login', component: LoginPageComponent },
      // { path: 'register', component: RegisterPageComponent },

      // TODO: these will not work because the authorization is made at application level in the MVC view. 
      { path: 'error', component: ErrorPageComponent },
      { path: 'access-denied', component: AccessDeniedPageComponent },

      { path: 'externalRedirect', resolve: { url: externalUrlProvider }, component: NotFoundPageComponent }
    ])
  ],
  providers: [
    SettingsProvider,
    { provide: APP_INITIALIZER, useFactory: settingsProviderFactory, deps: [SettingsProvider], multi: true },
    { provide: externalUrlProvider, useValue: externalUrlRedirect }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
