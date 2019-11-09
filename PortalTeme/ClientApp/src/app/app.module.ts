import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER, Provider } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { LayoutModule } from '@angular/cdk/layout';
import { FlexLayoutModule } from '@angular/flex-layout'

import { MaterialComponentsModule } from './modules/AngularMaterialImports/material-components.module';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { LoginPageComponent } from './authentication/login-page/login-page.component';

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
import { StudyDomainsComponent } from './admin/study-domains/study-domains.component';
import { StudyGroupsComponent } from './admin/study-groups/study-groups.component';
import { DataTableComponent } from './components/datatable/datatable.component';
import { CourseEditBasicComponent } from './admin/courses/course-edit-basic/course-edit-basic.component';
import { ViewCoursesComponent } from './admin/courses/view-courses/view-courses.component';
import { CourseCreateComponent } from './admin/courses/course-create/course-create.component';
import { CourseEditRouterComponent } from './admin/courses/course-edit-router/course-edit-router.component';
import { CoursePageComponent } from './user-pages/course-page/course-page.component';
import { CourseRouterComponent } from './user-pages/course-router/course-router.component';
import { CourseManagePageComponent } from './user-pages/course-manage/course-manage-page/course-manage-page.component';
import { CourseManageRouterComponent } from './user-pages/course-manage/course-manage-router/course-manage-router.component';
import { ViewAssignmentPageComponent } from './user-pages/view-assignment-page/view-assignment-page.component';
import { CourseEditAssistantsPageComponent } from './admin/courses/course-edit-assistants-page/course-edit-assistants-page.component';
import { CourseEditAssistantsComponent } from './components/courses/course-edit-assistants/course-edit-assistants.component';
import { CourseEditAttendeesPageComponent } from './admin/courses/course-edit-attendees-page/course-edit-attendees-page.component';
import { CourseEditAttendeesComponent } from './components/courses/course-edit-attendees/course-edit-attendees.component';
import { AssignmentsManageViewComponent } from './components/assignments/assignments-manage-view/assignments-manage-view.component';
import { AssignmentEditPageComponent } from './user-pages/course-manage/assignment-edit-page/assignment-edit-page.component';
import { NewAssignmentPageComponent } from './user-pages/course-manage/new-assignment-page/new-assignment-page.component';
import { AssignmentEditFormComponent } from './components/assignments/assignment-edit-form/assignment-edit-form.component';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { EnumKeysPipe } from './pipes/enum-keys.pipe';
import { AssignmentCardComponent } from './components/assignments/assignment-card/assignment-card.component';
import { MAT_HAMMER_OPTIONS } from '@angular/material/core';
import { TabbedMenuComponent } from './components/tabbed-menu/tabbed-menu.component';
import { AssignmentEditTasksPageComponent } from './user-pages/course-manage/assignment-edit-tasks-page/assignment-edit-tasks-page.component';
import { AssignmentTasksEditComponent } from './components/assignments/assignment-tasks-edit/assignment-tasks-edit.component';
import { AssignmentTasksEditFormComponent } from './components/assignments/assignment-tasks-edit-form/assignment-tasks-edit-form.component';
import { DateAdapterService } from './modules/AngularMaterialImports/date.adapter';
import { CreateSubmissionPageComponent } from './user-pages/create-submission-page/create-submission-page.component';
import { DropzoneFileUploadComponent } from './components/dropzone-file-upload/dropzone-file-upload.component';
import { DiskSizePipe } from './pipes/disk-size.pipe';
import { GradeSubmissionDialog } from './user-pages/view-assignment-page/grade-assignment.dialog';
import { DataTableComponent2 } from './components/datatable2/datatable2.component';
import { TableEditableCellComponent } from './components/datatable2/components/table-editable-cell/table-editable-cell.component';

const httpInterceptorProviders: Provider[] = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true }
];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    TabbedMenuComponent,

    DataTableComponent,
    DataTableComponent2,
    ExternalUrlDirective,

    CourseEditAssistantsComponent,
    CourseEditAttendeesComponent,

    AssignmentEditFormComponent,

    HomeComponent,
    LoginPageComponent,
    ErrorPageComponent,
    NotFoundPageComponent,
    AccessDeniedPageComponent,

    KeysPipe,
    EnumKeysPipe,
    SafeHtmlPipe,
    DiskSizePipe,

    CoursesRouterComponent,
    CourseEditRouterComponent,
    CourseRouterComponent,
    CourseManageRouterComponent,


    AdminPageComponent,
    AdminNavMenuComponent,
    AcademicYearsComponent,
    StudyDomainsComponent,
    StudyGroupsComponent,
    CourseDefinitionsComponent,

    ViewCoursesComponent,
    CourseCreateComponent,
    CourseEditBasicComponent,
    CourseEditAssistantsPageComponent,
    CourseEditAttendeesPageComponent,

    CoursePageComponent,

    CourseManagePageComponent,
    NewAssignmentPageComponent,
    ViewAssignmentPageComponent,
    AssignmentsManageViewComponent,
    AssignmentEditPageComponent,
    AssignmentCardComponent,
    AssignmentEditTasksPageComponent,
    AssignmentTasksEditComponent,
    AssignmentTasksEditFormComponent,
    GradeSubmissionDialog,

    CreateSubmissionPageComponent,
    DropzoneFileUploadComponent,
    TableEditableCellComponent

  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    MaterialComponentsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutModule,
    FlexLayoutModule,

    CKEditorModule,

    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'login', component: LoginPageComponent },

      {
        path: 'course/:slug', component: CourseRouterComponent, canActivate: [AuthGuard],
        children: [
          { path: '', component: CoursePageComponent, canActivate: [AuthGuard], pathMatch: 'full' },
          {
            path: 'manage', component: CourseManageRouterComponent, canActivate: [AuthGuard],
            data: {
              roles: ['Professor']
            },
            children: [
              { path: '', component: CourseManagePageComponent, pathMatch: 'full' },
              { path: 'new-assignment', component: NewAssignmentPageComponent },
              { path: 'assignment/:assignmentId', component: AssignmentEditPageComponent },
              { path: 'assignment/:assignmentId/tasks', component: AssignmentEditTasksPageComponent }
            ]
          },
          { path: ':assigSlug', component: ViewAssignmentPageComponent },
          { path: ':assigSlug/new-submission', component: CreateSubmissionPageComponent }
        ]
      },
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
              { path: 'create', component: CourseCreateComponent },
              {
                path: ':id', component: CourseEditRouterComponent, children: [
                  { path: '', component: CourseEditBasicComponent, pathMatch: 'full' },
                  { path: 'assistants', component: CourseEditAssistantsPageComponent },
                  { path: 'attendees', component: CourseEditAttendeesPageComponent }
                ]
              }
            ]
          }
        ]
      },

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
    { provide: APP_INITIALIZER, useFactory: settingsProviderFactory, deps: [SettingsProvider, DateAdapterService], multi: true },
    ...httpInterceptorProviders,
    { provide: externalUrlProvider, useValue: externalUrlRedirect },
    {
      provide: MAT_HAMMER_OPTIONS,
      useValue: { cssProps: { userSelect: true } },
    }
  ],
  entryComponents: [GradeSubmissionDialog],
  bootstrap: [AppComponent]
})
export class AppModule { }
