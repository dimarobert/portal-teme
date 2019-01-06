import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminMenuService {

  private state: BehaviorSubject<AdminMenuState>;

  constructor() {
    this.state = new BehaviorSubject(AdminMenuState.AdminMenu);
  }

  get menuState(): Observable<AdminMenuState> {
    return this.state;
  }

  public changeMenuState(state: AdminMenuState): void {
    this.state.next(state);
  }

}

export enum AdminMenuState {
  AdminMenu,
  EditCourseMenu
}
