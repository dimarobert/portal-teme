import { Component, OnInit } from '@angular/core';
import { NavLink } from '../../../models/nav-link.model';
import { BehaviorSubject, Subscription, Observable } from 'rxjs';
import { Assignment, AssignmentType, AssignmentTask, AssignmentTaskCreateRequest, AssignmentTaskUpdateRequest } from '../../../models/assignment.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ModelServiceFactory } from '../../../services/model.service';
import { take, map } from 'rxjs/operators';
import { User } from '../../../models/course.model';

@Component({
  selector: 'app-assignment-edit-tasks-page',
  templateUrl: './assignment-edit-tasks-page.component.html',
  styleUrls: ['./assignment-edit-tasks-page.component.scss']
})
export class AssignmentEditTasksPageComponent implements OnInit {
  menuItems: BehaviorSubject<NavLink[]>;

  routeSub: Subscription;
  assignment: BehaviorSubject<Assignment>;
  courseStudents: BehaviorSubject<User[]>;

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
    this.courseStudents = new BehaviorSubject([]);
    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const assignmentId = params.get('assignmentId');

        this.modelSvcFactory.assignments.get(assignmentId)
          .pipe(take(1))
          .subscribe(assignResult => {
            if (assignResult.type == AssignmentType.SingleTask)
              this.router.navigate(['../'], { relativeTo: this.route });

            this.assignment.next(assignResult);

            this.modelSvcFactory.courses.getMembers(assignResult.course.id)
              .pipe(take(1))
              .subscribe(members => {
                this.courseStudents.next(members);
              })
          });
      });
  }

  get tasks(): Observable<AssignmentTask[]> {
    return this.assignment.pipe(map(a => a.tasks));
  }

  get isCustomAssigned(): Observable<boolean> {
    return this.assignment
      .pipe(map(a => a.type == AssignmentType.CustomAssignedTasks));
  }

  create(): (newTask: AssignmentTaskCreateRequest) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.createTask(task)
        .then(t => { });
    };
  }

  update(): (newTask: AssignmentTaskUpdateRequest) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.updateTask(task)
        .then(_ => {
          const assignment = this.assignment.value;
          if (assignment.type != AssignmentType.CustomAssignedTasks)
            return;
          const aTask = assignment.tasks.find(t => t.id == task.id);
          if (!aTask)
            return;

          aTask.studentsAssigned = [this.courseStudents.value.find(s => s.id == task.assignedTo)];

          this.assignment.next(assignment);
        });
    };
  }

  delete(): (newTask: AssignmentTask) => Promise<void> {
    return (task) => {
      return this.modelSvcFactory.assignments.deleteTask(task);
    };
  }

}
