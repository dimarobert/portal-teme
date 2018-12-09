import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { MaterialComponentsModule } from './material-components.module';
import { LayoutModule } from '@angular/cdk/layout';
import { LoginPageComponent } from './authentication/login-page/login-page.component';
import { RegisterPageComponent } from './authentication/register-page/register-page.component';
import { LogoutComponent } from './authentication/logout/logout.component';
import { KeysPipe } from './pipes/object-keys.pipe';

import { SettingsProvider, settingsProviderFactory } from './services/settings.provider';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginPageComponent,
    RegisterPageComponent,
    LogoutComponent,

    KeysPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    MaterialComponentsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'login', component: LoginPageComponent },
      { path: 'register', component: RegisterPageComponent },
      { path: 'logout', component: LogoutComponent }
    ])
  ],
  providers: [
    SettingsProvider,
    { provide: APP_INITIALIZER, useFactory: settingsProviderFactory, deps: [SettingsProvider], multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
