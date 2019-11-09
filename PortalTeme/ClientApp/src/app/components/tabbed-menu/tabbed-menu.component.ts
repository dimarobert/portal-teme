import { Component, OnInit, ViewChild, OnDestroy, Input, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { MatTabChangeEvent, MatTabGroup } from '@angular/material';
import { Subscription, BehaviorSubject, Observable } from 'rxjs';
import { NavLink } from '../../models/nav-link.model';

@Component({
  selector: 'app-tabbed-menu',
  templateUrl: './tabbed-menu.component.html',
  styleUrls: ['./tabbed-menu.component.scss']
})
export class TabbedMenuComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('tabsGroup', { static: false }) tabsGroup: MatTabGroup;

  @Input() menuItems: Observable<NavLink[]>;

  constructor(private route: ActivatedRoute, private router: Router) { }

  private _menuItemsSub: Subscription;
  private _currentMenu: NavLink[];

  ngOnInit() {
    this._currentMenu = [];
  }

  ngAfterViewInit(): void {
    this._menuItemsSub = this.menuItems.subscribe(menu => {
      if (this._currentMenu != menu)
        this.updateSelectedTab(menu);
      this._currentMenu = menu;
    });
  }

  ngOnDestroy(): void {
    (this._menuItemsSub && this._menuItemsSub.unsubscribe());
  }

  updateSelectedTab(currentMenu: NavLink[]) {
    const currentLinkIndex = currentMenu.findIndex(link => {
      const urlTree = this.router.createUrlTree([...this.getAdminRelativeCommands(link)], { relativeTo: this.route });
      return this.router.isActive(urlTree, link.exact);
    });

    if (currentLinkIndex > -1)
      this.tabsGroup.selectedIndex = currentLinkIndex;
  }

  onTabChange(event: MatTabChangeEvent) {
    if (event.index < 0 || event.index > this._currentMenu.length)
      return;

    const link = this._currentMenu[event.index];
    this.router.navigate(this.getAdminRelativeCommands(link), { relativeTo: this.route });
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
