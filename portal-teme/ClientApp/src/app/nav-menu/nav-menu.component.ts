import { Component, ViewChild, Type } from '@angular/core';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginPageComponent } from '../authentication/login-page/login-page.component';
import { RegisterPageComponent } from '../authentication/register-page/register-page.component';

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
    var firstChild = this.route.children[0];
    if (firstChild && this.isLoginOrRegisterPage(firstChild.component)) {
      return this.route.snapshot.queryParams['returnUrl'] || '/';
    }
    return this.router.url;
  };

  isLoginOrRegisterPage(component: string | Type<any>) {
    return component == LoginPageComponent || component == RegisterPageComponent;
  }

  constructor(private route: ActivatedRoute, private router: Router, private breakpointObserver: BreakpointObserver) { }


  toggleSidenav() {
    if (this.breakpointObserver.isMatched(Breakpoints.Handset)) {
      this.sidenav.toggle();
    }
  }
}
