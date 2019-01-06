import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER, Provider } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

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
import { AuthenticationInterceptor } from './authentication/services/authentication.interceptor';
import { CourseDefinitionsComponent } from './admin/course-definitions/course-definitions.component';
import { CoursesRouterComponent } from './admin/courses/courses-router.component';
import { AcademicYearsComponent } from './admin/academic-years/academic-years.component';
import { AdminPageComponent } from './admin/admin-page/admin-page.component';
import { AdminNavMenuComponent } from './admin/admin-nav-menu/admin-nav-menu.component';
import { MyCoursesComponent } from './user-pages/my-courses/my-courses.component';
import { StudyDomainsComponent } from './admin/study-domains/study-domains.component';
import { StudyGroupsComponent } from './admin/study-groups/study-groups.component';
import { DataTableComponent } from './components/datatable/datatable.component';
import { CourseEditBasicComponent } from './admin/courses/course-edit-basic/course-edit-basic.component';
import { ViewCoursesComponent } from './admin/courses/view-courses/view-courses.component';
import { CourseEditAssistantsComponent } from './admin/courses/course-edit-assistants/course-edit-assistants.component';
import { CourseCreateComponent } from './admin/courses/course-create/course-create.component';
import { CourseEditAttendeesComponent } from './admin/courses/course-edit-attendees/course-edit-attendees.component';
import { CourseEditRouterComponent } from './admin/courses/course-edit-router/course-edit-router.component';

const httpInterceptorProviders: Provider[] = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true }
];

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

    CourseDefinitionsComponent,
    CoursesRouterComponent,
    CourseEditBasicComponent,
    CourseEditRouterComponent,

    AcademicYearsComponent,

    AdminPageComponent,

    AdminNavMenuComponent,

    MyCoursesComponent,

    StudyDomainsComponent,
    StudyGroupsComponent,

    DataTableComponent,

    ViewCoursesComponent,

    CourseEditAssistantsComponent,

    CourseCreateComponent,

    CourseEditAttendeesComponent
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
      { path: 'my-courses', component: MyCoursesComponent, canActivate: [AuthGuard] },
      {
        path: 'admin', component: AdminPageComponent, canActivate: [AuthGuard],
        data: { roles: ['Admin'] },
        children: [
          { path: '', redirectTo: 'years', pathMatch: 'full' },
          { path: 'years', component: AcademicYearsComponent },
          { path: 'study-domains', component: StudyDomainsComponent },
          { path: 'study-groups', component: StudyGroupsComponent },
          { path: 'course-definitions', component: CourseDefinitionsComponent },
          { path: 'courses', component: ViewCoursesComponent },
          {
            path: 'course', component: CoursesRouterComponent, children: [
              {
                path: 'create', component: CourseCreateComponent, children: [
                  // { path: '', component: CourseBasicComponent, pathMatch: 'full' },
                ]
              },
              {
                path: ':id', component: CourseEditRouterComponent, children: [
                  { path: '', component: CourseEditBasicComponent, pathMatch: 'full' },
                  { path: 'assistants', component: CourseEditAssistantsComponent },
                  { path: 'attendees', component: CourseEditAttendeesComponent }
                ]
              }
            ]
          }
        ]
      },

      { path: 'login', component: LoginPageComponent },

      // TODO: these will not work because the authorization is made at application level in the MVC view. 
      { path: 'error', component: ErrorPageComponent },
      { path: 'access-denied', component: AccessDeniedPageComponent },

      { path: 'externalRedirect', resolve: { url: externalUrlProvider }, component: NotFoundPageComponent }
    ], {
        relativeLinkResolution: 'corrected'
      })
  ],
  providers: [
    SettingsProvider,
    { provide: APP_INITIALIZER, useFactory: settingsProviderFactory, deps: [SettingsProvider], multi: true },
    ...httpInterceptorProviders,
    { provide: externalUrlProvider, useValue: externalUrlRedirect }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
