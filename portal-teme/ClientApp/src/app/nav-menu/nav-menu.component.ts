import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      // mobile first design
      // this is used as an workaround for https://github.com/angular/material2/issues/13852
      startWith(<BreakpointState>{ matches: true }),
      map(result => result.matches)
    );

  @ViewChild('sidenav') sidenav;

  get returnUrl(): string {
    return this.router.url;
  };
  
  constructor(private router: Router, private breakpointObserver: BreakpointObserver) { }


  toggleSidenav() {
    if (this.breakpointObserver.isMatched(Breakpoints.Handset)) {
      this.sidenav.toggle();
    }
  }
}
