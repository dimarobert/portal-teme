import { Component, ViewChild, Type, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { MatSidenav } from '@angular/material';

import { trigger, state, style, transition, animate } from '@angular/animations';

import { Observable, Subject, Subscription } from 'rxjs';
import { startWith, map, take } from 'rxjs/operators';

import { SettingsProvider } from '../../services/settings.provider';
import { AuthService } from '../../authentication/services/auth.service';
import { AuthConstants } from '../../authentication/constants';
import { CourseEdit } from '../../models/course.model';
import { ModelServiceFactory } from '../../services/model.service';
import { MenuService } from '../../services/menu.service';
import { CdkAccordionItem } from '@angular/cdk/accordion';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  animations: [
    trigger('bodyExpansion', [
      state('collapsed', style({ height: '0px', display: 'none' })),
      state('expanded', style({ height: '*', display: 'block' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4,0.0,0.2,1)')),
    ]),
    trigger('openCloseArrowIcon', [
      state('closed', style({})),
      state('open', style({
        transform: 'rotate(-180deg)'
      })),
      transition('open <=> closed', [
        animate('.2s')
      ])
    ])
  ]
})
export class NavMenuComponent implements AfterViewInit, OnInit, OnDestroy {

  @ViewChild('sidenav') sidenav: MatSidenav;
  @ViewChild('panelCourses') panelCourses: CdkAccordionItem;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      // mobile first design
      // this is used as an workaround for https://github.com/angular/material2/issues/13852
      startWith(<BreakpointState>{ matches: true }),
      map(result => result.matches)
    );

  courses: Subject<CourseEdit[]>;
  coursesMenuStateSub: Subscription;
  coursesRefreshSub: Subscription;

  constructor(private router: Router, private breakpointObserver: BreakpointObserver,
    private menuService: MenuService,
    private settings: SettingsProvider, private auth: AuthService, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit(): void {

    this.courses = new Subject();

    this.isAuthenticated$
      .pipe(take(1))
      .subscribe(isAuth => {
        if (!isAuth)
          return;

        this.getCourses();
      });

  }

  private getCourses() {
    this.modelSvcFactory.courses.getAllRef()
      .pipe(take(1))
      .subscribe(courseList => {
        this.courses.next(courseList.filter(c => c.courseDef.slug != null));
      });
  }

  ngAfterViewInit(): void {
    this.coursesMenuStateSub = this.menuService.watchCoursesCollapseChanges()
      .subscribe(state => {
        if (this.panelCourses)
          this.panelCourses.expanded = state;
      });

    this.coursesRefreshSub = this.menuService.watchCoursesRefresh()
      .subscribe(_ => {
        this.getCourses();
      })
  }

  ngOnDestroy(): void {
    this.coursesMenuStateSub.unsubscribe();
    this.coursesRefreshSub.unsubscribe();
  }

  get isAuthenticated$() {
    return this.settings.isUserAuthenticated$;
  }

  get isAdmin$(): Observable<boolean> {
    return this.auth.isInRole$(AuthConstants.adminRoleName);
  }

  get userDisplayName$() {
    return this.settings.isUserAuthenticated$.pipe(
      map(isAuth => {
        if (!isAuth)
          return 'Anonymous';

        const { firstName, lastName, email } = this.settings.user;
        if (!firstName && !lastName)
          return email;
        return `${firstName || ''} ${lastName || ''}`.trim();
      })
    );
  }

  get returnUrl(): string {
    return this.router.url;
  };

  logout() {
    location.href = '/authentication/logout';
  }

  toggleSidenav() {
    if (this.breakpointObserver.isMatched(Breakpoints.Handset)) {
      this.sidenav.toggle();
    }
  }
}
