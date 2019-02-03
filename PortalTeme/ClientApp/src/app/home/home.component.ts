import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../authentication/services/auth.service';
import { Observable, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  constructor(private router: Router, private auth: AuthService) { }

  private authSub: Subscription;

  ngOnInit(): void {
    this.authSub = this.isAuthenticated.subscribe(isAuth => {
      if (!isAuth)
        this.router.navigate(['login']);
    });
  }

  get isAuthenticated(): Observable<boolean> {
    return this.auth.isAuthenticated();
  }

  get name(): Observable<string> {
    return this.auth.user$
      .pipe(map(u => `${u.firstName} ${u.lastName}`));
  }

  ngOnDestroy(): void {
    this.authSub && this.authSub.unsubscribe();
  }
}
