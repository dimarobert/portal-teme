import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  coursesCollapseChanges: BehaviorSubject<boolean>;

  constructor() {
    this.coursesCollapseChanges = new BehaviorSubject(false);
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

}

export enum CollapseOption {
  Open,
  Close,
  Toggle
}
