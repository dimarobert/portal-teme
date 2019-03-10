import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  coursesCollapseChanges: BehaviorSubject<boolean>;
  refreshCoursesMenu: Subject<void>;

  constructor() {
    this.coursesCollapseChanges = new BehaviorSubject(false);
    this.refreshCoursesMenu = new Subject();
  }

  public watchCoursesCollapseChanges(): Observable<boolean> {
    return this.coursesCollapseChanges;
  }

  public setCourseCollapse(option: CollapseOption) {
    switch (option) {
      case CollapseOption.Open:
        this.coursesCollapseChanges.next(true);
        break;

      case CollapseOption.Close:
        this.coursesCollapseChanges.next(false);
        break;

      case CollapseOption.Toggle:
        this.coursesCollapseChanges.next(!this.coursesCollapseChanges.value);
        break;

    }
  }

  public refreshCourses() {
    this.refreshCoursesMenu.next();
  }

  public watchCoursesRefresh(): Observable<void> {
    return this.refreshCoursesMenu;
  }

}

export enum CollapseOption {
  Open,
  Close,
  Toggle
}
