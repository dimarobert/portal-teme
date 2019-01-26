import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApplicationSettings, UserSettings } from '../models/application-settings.model';
import { map } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';
import { DateAdapterService } from '../modules/AngularMaterialImports/date.adapter';

@Injectable()
export class SettingsProvider {

  constructor(private http: HttpClient) { }

  private _settingsSubject: BehaviorSubject<ApplicationSettings> = new BehaviorSubject<ApplicationSettings>({});

  load() {
    const settings: ApplicationSettings = window["portalTeme"].settings;
    this._settingsSubject.next(settings);
  }

  get user() {
    if (this._settingsSubject.value)
      return this._settingsSubject.value.user;
    return null;
  }

  get user$(): Observable<UserSettings> {
    return this._settingsSubject.pipe(
      map(s => !!s ? s.user : null)
    );
  }

  get isUserAuthenticated$(): Observable<boolean> {
    return this.user$.pipe(
      map(u => !!u)
    )
  }

}


export function settingsProviderFactory(provider: SettingsProvider, dateSvc: DateAdapterService) {
  return () => {
    dateSvc.setFirstDayOfWeek(1);
    provider.load();
  }
}
