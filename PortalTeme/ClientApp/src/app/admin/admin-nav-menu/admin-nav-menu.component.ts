import { Component, OnInit } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../services/admin-menu.service';

@Component({
  selector: 'admin-nav-menu',
  templateUrl: './admin-nav-menu.component.html',
  styleUrls: ['./admin-nav-menu.component.scss']
})
export class AdminNavMenuComponent implements OnInit {

  adminLinks: NavLink[];
  createCourseLinks: NavLink[];

  AdminMenuState = AdminMenuState;

  get menuState() {
    return this.adminMenuSvc.menuState;
  }

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.loadAdminLinks();
    this.loadCourseLinks();
  }

  private loadAdminLinks() {
    this.adminLinks = [
      new NavLink({
        label: 'Years',
        path: '/admin/years',
        index: 0
      }),
      new NavLink({
        label: 'Study Domains',
        path: '/admin/study-domains',
        index: 0
      }),
      new NavLink({
        label: 'Study Groups',
        path: '/admin/study-groups',
        index: 0
      }),
      new NavLink({
        label: 'Course Definitions',
        path: '/admin/course-definitions',
        index: 0
      }),
      new NavLink({
        label: 'Courses',
        path: '/admin/courses',
        index: 0
      })
    ];
  }

  private loadCourseLinks() {
    this.createCourseLinks = [
      new NavLink({
        label: 'Back',
        path: '/admin/courses',
        exact: true,
        index: 0,
        action: () => {
          this.adminMenuSvc.changeMenuState(AdminMenuState.AdminMenu);
        }
      }),
      new NavLink({
        label: 'Basic Information',
        path: '/admin/course/create',
        exact: true,
        index: 0
      }),
      new NavLink({
        label: 'Assistants',
        path: '/admin/course/create/assistants',
        index: 0
      }),
      new NavLink({
        label: 'Attendees',
        path: '/admin/course/create/attendees',
        index: 0
      })
    ];
  }
}


class NavLink implements NavLinkOptions {

  constructor(private options: NavLinkOptions) { }

  get label(): string {
    return this.options.label;
  }

  get path(): string {
    return this.options.path;
  }

  get index(): number {
    return this.options.index;
  }

  get exact(): boolean {
    return this.options.exact || false;
  }

  get action(): () => void {
    return this.options.action || (() => { });
  }
}

interface NavLinkOptions {
  label: string;
  path: string;
  index: number;

  exact?: boolean;
  action?: () => void;
}