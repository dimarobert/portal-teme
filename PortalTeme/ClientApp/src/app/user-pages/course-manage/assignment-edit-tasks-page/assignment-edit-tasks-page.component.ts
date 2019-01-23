import { Component, OnInit } from '@angular/core';
import { NavLink } from '../../../models/nav-link.model';
import { BehaviorSubject, Subscription, Observable } from 'rxjs';
import { Assignment, AssignmentType, AssignmentTask, AssignmentTaskEdit } from '../../../models/assignment.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ModelServiceFactory } from '../../../services/model.service';
import { take, map } from 'rxjs/operators';

@Component({
  selector: 'app-assignment-edit-tasks-page',
  templateUrl: './assignment-edit-tasks-page.component.html',
  styleUrls: ['./assignment-edit-tasks-page.component.scss']
})
export class AssignmentEditTasksPageComponent implements OnInit {
  menuItems: BehaviorSubject<NavLink[]>;

  routeSub: Subscription;
  assignment: BehaviorSubject<Assignment>;

  constructor(private route: ActivatedRoute, private router: Router, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {

    this.menuItems = new BehaviorSubject<NavLink[]>([
      new NavLink({
        label: 'Details',
        commands: ['../'],
        exact: true,
        icon: 'assignment'
      }),
      new NavLink({
        label: 'Task List',
        commands: ['./'],
        exact: true,
        icon: 'subject'
      }),
    ]);

    this.getData();

  }
  getData(): any {
    this.assignment = new BehaviorSubject(null);
    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const assignmentId = params.get('assignmentId');

        this.modelSvcFactory.assignments.get(assignmentId)
          .pipe(take(1))
          .subscribe(assignResult => {
            if (assignResult.type == AssignmentType.SingleTask)
              this.router.navigate(['../'], { relativeTo: this.route });

            this.assignment.next(assignResult);
          });
      });
  }

  get tasks(): Observable<AssignmentTask[]> {
    return this.assignment.pipe(map(a => a.tasks));
  }

  create(): (newTask: AssignmentTaskEdit) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.createTask(task)
        .then(t => { });
    };
  }

  update(): (newTask: AssignmentTask) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.updateTask(task);
    };
  }

  delete(): (newTask: AssignmentTask) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.deleteTask(task);
    };
  }

}
