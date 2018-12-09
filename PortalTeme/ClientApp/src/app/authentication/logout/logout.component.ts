import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SettingsProvider } from '../../services/settings.provider';
import { AuthService } from '../auth.service';
import { take } from 'rxjs/operators';
import { from } from 'rxjs';

@Component({
  selector: 'app-logout',
  template: 'Logging you out...'
})
export class LogoutComponent implements OnInit {

  constructor(private authSvc: AuthService, private settings: SettingsProvider, private router: Router) { }

  ngOnInit() {
    this.settings.isUserAuthenticated$
      .pipe(take(1))
      .toPromise()
      .then(async isAuth => {
        if (isAuth) {
          await this.authSvc.logout().toPromise();
          await this.settings.load();
        }
        from(this.router.navigate(['/']));
        await this.router.navigate(['/']);
      });
  }
}
