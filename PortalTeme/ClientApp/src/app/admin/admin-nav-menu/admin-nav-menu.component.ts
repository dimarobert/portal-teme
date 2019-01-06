import { Component, OnInit, Type } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminMenuService, AdminMenuState } from '../services/admin-menu.service';
import { CourseEditRouterComponent } from '../courses/course-edit-router/course-edit-router.component';

@Component({
  selector: 'admin-nav-menu',
  templateUrl: './admin-nav-menu.component.html',
  styleUrls: ['./admin-nav-menu.component.scss']
})
export class AdminNavMenuComponent implements OnInit {

  adminLinks: NavLink[];
  editCourseLinks: NavLink[];

  AdminMenuState = AdminMenuState;

  get menuState() {
    return this.adminMenuSvc.menuState;
  }

  constructor(private adminMenuSvc: AdminMenuService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.loadAdminLinks();
    this.loadCourseLinks();
  }

  private loadAdminLinks() {
    this.adminLinks = [
      new NavLink({
        label: 'Years',
        commands: ['years']
      }),
      new NavLink({
        label: 'Study Domains',
        commands: ['study-domains']
      }),
      new NavLink({
        label: 'Study Groups',
        commands: ['study-groups']
      }),
      new NavLink({
        label: 'Course Definitions',
        commands: ['course-definitions']
      }),
      new NavLink({
        label: 'Courses',
        commands: ['courses']
      })
    ];
  }

  private loadCourseLinks() {
    this.editCourseLinks = [
      new NavLink({
        label: 'Back',
        commands: ['courses'],
        exact: true
      }),
      new NavLink({
        label: 'Basic Information',
        commands: ['./'],
        exact: true,
        relativeTo: CourseEditRouterComponent
      }),
      new NavLink({
        label: 'Assistants',
        commands: ['assistants'],
        relativeTo: CourseEditRouterComponent
      }),
      new NavLink({
        label: 'Attendees',
        commands: ['attendees'],
        relativeTo: CourseEditRouterComponent
      })
    ];
  }

  getAdminRelativeCommands(link: NavLink) {
    let commands = [];
    let child = this.route;
    if (link.relative)
      while (child.component != link.relativeTo) {
        child = child.firstChild;
        if (child == null) {
          console.error('', link, this.route.snapshot);
          throw new Error('invalid link configuration');
        }
        commands.push(child.snapshot.url.map(url => url.toString()).join('/'));
      }

    if (link.commands.length > 1 || link.commands[0] !== './')
      commands.push(...link.commands);
    return commands;
  }
}


class NavLink implements NavLinkOptions {

  constructor(private options: NavLinkOptions) { }

  get label(): string {
    return this.options.label;
  }

  get commands(): any[] {
    return this.options.commands;
  }

  get relative(): boolean {
    return this.options.relativeTo != null;
  }

  get relativeTo(): Type<any> {
    return this.options.relativeTo;
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
  commands: any[];

  relativeTo?: Type<any>;
  exact?: boolean;
  action?: () => void;
}