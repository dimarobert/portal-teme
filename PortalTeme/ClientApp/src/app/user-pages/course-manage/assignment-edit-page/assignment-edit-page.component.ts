import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Subject, BehaviorSubject, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ModelServiceFactory } from '../../../services/model.service';
import { take, map } from 'rxjs/operators';
import { Assignment, AssignmentEdit, AssignmentType } from '../../../models/assignment.model';
import { NavLink } from '../../../models/nav-link.model';

@Component({
  selector: 'app-assignment-edit-page',
  templateUrl: './assignment-edit-page.component.html',
  styleUrls: ['./assignment-edit-page.component.scss']
})
export class AssignmentEditPageComponent implements OnInit, OnDestroy {

  routeSub: Subscription;
  assignment: BehaviorSubject<Assignment>;

  menuItems: BehaviorSubject<NavLink[]>;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {
    this.menuItems = new BehaviorSubject<NavLink[]>([
      new NavLink({
        label: 'Details',
        commands: ['./'],
        exact: true,
        icon: 'assignment'
      }),
      new NavLink({
        label: 'Task List',
        commands: ['tasks'],
        exact: true,
        icon: 'subject'
      }),
    ]);

    this.update = this.update.bind(this);

    this.assignment = new BehaviorSubject(null);
    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const assignmentId = params.get('assignmentId');

        this.modelSvcFactory.assignments.get(assignmentId)
          .pipe(take(1))
          .subscribe(assignResult => {
            this.assignment.next(assignResult);
          });
      });
  }

  update(assignment: Assignment) {
    assignment.id = this.assignment.value.id;
    return this.modelSvcFactory.assignments.update(assignment)
      .then(_ => {
        this.assignment.next(assignment);
      });
  }

  private _hasTaskList: BehaviorSubject<boolean>;
  private _assignSub: Subscription;
  get hasTaskList(): Observable<boolean> {
    this._hasTaskList = this._hasTaskList || new BehaviorSubject(false);

    this._assignSub = this.assignment
      .subscribe(a => {
        this._hasTaskList.next(a.type != AssignmentType.SingleTask);
      });

    return this._hasTaskList;
  }

  ngOnDestroy(): void {
    this.routeSub && this.routeSub.unsubscribe();
    this._assignSub && this._assignSub.unsubscribe();
  }
}
