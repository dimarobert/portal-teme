import { Component, OnInit, Type, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminMenuService, AdminMenuState } from '../services/admin-menu.service';
import { CourseEditRouterComponent } from '../courses/course-edit-router/course-edit-router.component';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material';
import { Subscription, BehaviorSubject } from 'rxjs';

@Component({
  selector: 'admin-nav-menu',
  templateUrl: './admin-nav-menu.component.html',
  styleUrls: ['./admin-nav-menu.component.scss']
})
export class AdminNavMenuComponent implements OnInit, OnDestroy {

  @ViewChild('tabsGroup', { static: false }) tabsGroup: MatTabGroup;

  private adminLinks: NavLink[];
  private editCourseLinks: NavLink[];

  AdminMenuState = AdminMenuState;

  get menuState() {
    return this.adminMenuSvc.menuState;
  }

  constructor(private adminMenuSvc: AdminMenuService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.loadAdminLinks();
    this.loadCourseLinks();
  }

  ngOnDestroy(): void {
    (this._menuItemsSub && this._menuItemsSub.unsubscribe());
  }

  private _menuItems: BehaviorSubject<NavLink[]>;
  private _menuItemsSub: Subscription;
  get menuItems() {
    this._menuItems = this._menuItems || new BehaviorSubject(this.adminLinks);
    this._menuItemsSub = this.menuState.subscribe(state => {
      let oldMenu = this._menuItems.value;
      if (state == AdminMenuState.EditCourseMenu) {
        this._menuItems.next(this.editCourseLinks);
      } else if (state == AdminMenuState.AdminMenu) {
        this._menuItems.next(this.adminLinks);
      }

      if (oldMenu != this._menuItems.value)
        this.updateSelectedTab(this._menuItems.value);
    });

    return this._menuItems;
  }

  updateSelectedTab(currentMenu: NavLink[]) {
    const currentLinkIndex = currentMenu.findIndex(link => {
      const urlTree = this.router.createUrlTree(['/admin', ...this.getAdminRelativeCommands(link)]);
      return this.router.isActive(urlTree, link.exact);
    });

    if (currentLinkIndex > -1)
      this.tabsGroup.selectedIndex = currentLinkIndex;
  }

  onTabChange(event: MatTabChangeEvent) {
    if (event.index < 0 || event.index > this.menuItems.value.length)
      return;

    const link = this.menuItems.value[event.index];
    this.router.navigate(this.getAdminRelativeCommands(link), { relativeTo: this.route });
  }

  private loadAdminLinks() {
    this.adminLinks = [
      new NavLink({
        label: 'Years',
        commands: ['years'],
        icon: 'calendar_today'
      }),
      new NavLink({
        label: 'Study Domains',
        commands: ['study-domains'],
        icon: 'class'
      }),
      new NavLink({
        label: 'Study Groups',
        commands: ['study-groups'],
        icon: 'chrome_reader_mode'
      }),
      new NavLink({
        label: 'Course Definitions',
        commands: ['course-definitions'],
        icon: 'extension'
      }),
      new NavLink({
        label: 'Courses',
        commands: ['courses'],
        icon: 'art_track'
      })
    ];
  }

  private loadCourseLinks() {
    this.editCourseLinks = [
      new NavLink({
        label: 'Back',
        commands: ['courses'],
        exact: true,
        icon: 'arrow_left'
      }),
      new NavLink({
        label: 'Basic Information',
        commands: ['./'],
        exact: true,
        relativeTo: CourseEditRouterComponent,
        icon: 'supervisor_account'
      }),
      new NavLink({
        label: 'Assistants',
        commands: ['assistants'],
        relativeTo: CourseEditRouterComponent,
        icon: 'record_voice_over'
      }),
      new NavLink({
        label: 'Attendees',
        commands: ['attendees'],
        relativeTo: CourseEditRouterComponent,
        icon: 'people'
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

  get icon(): string {
    return this.options.icon;
  }
}

interface NavLinkOptions {
  label: string;
  commands: any[];

  icon?: string;
  relativeTo?: Type<any>;
  exact?: boolean;
  action?: () => void;
}