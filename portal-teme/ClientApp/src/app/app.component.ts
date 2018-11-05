import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

  @ViewChild('sidenav') sidenav;

  constructor(private breakpointObserver: BreakpointObserver) { }


  toggleSidenav() {
    if (this.breakpointObserver.isMatched(Breakpoints.Handset)) {
      this.sidenav.toggle();
    }
  }
}
