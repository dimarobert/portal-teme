import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { take } from 'rxjs/operators';
import { AssignmentType, UserAssignment, AssignmentTask } from '../../models/assignment.model';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-view-assignment-page',
  templateUrl: './view-assignment-page.component.html',
  styleUrls: ['./view-assignment-page.component.scss'],
  animations: []
})
export class ViewAssignmentPageComponent implements OnInit, OnDestroy {

  AssignmentType = AssignmentType;

  routeSub: Subscription;
  assignment: UserAssignment;

  datesShown: boolean = false;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  courseSlug: string;
  assignmentSlug: string;

  ngOnInit() {

    this.routeSub = combineLatest(
      this.route.parent.paramMap,
      this.route.paramMap
    ).subscribe(([course, assignment]) => {
      this.courseSlug = course.get('slug');
      this.assignmentSlug = assignment.get('assigSlug');

      this.getAssignment();
    });

  }

  private getAssignment() {
    this.modelSvcFactory.assignments.getBySlug(this.courseSlug, this.assignmentSlug)
      .pipe(take(1))
      .subscribe(assignmentResult => {
        this.assignment = assignmentResult;
      });
  }

  chooseTask(task: AssignmentTask) {
    this.modelSvcFactory.studentAssignedTasks.assignTaskToSelf(task.id)
      .then(_ => {
        this.getAssignment();
      });
  }

  get submissionsStarted(): boolean {
    return this.compareToNow(this.assignment.startDate) < 0;
  }

  get submissionsEnded(): boolean {
    return this.compareToNow(this.assignment.endDate) < 0;
  }

  get hasTasksList(): boolean {
    return this.assignment.type != AssignmentType.SingleTask
      && this.assignment.type != AssignmentType.CustomAssignedTasks;
  }

  get showTasksList(): boolean {
    return this.hasTasksList && this.assignment.assignedTask == null;
  }

  get userHasChosenTask(): boolean {
    return this.assignment.assignedTask != null;
  }

  taskCanBeChosen(task: AssignmentTask): boolean {
    if (this.userHasChosenTask || this.submissionsEnded)
      return false;

    if (this.assignment.type == AssignmentType.SingleTask || this.assignment.type == AssignmentType.CustomAssignedTasks)
      return false;

    if (this.assignment.type == AssignmentType.SingleChoiceList && task.studentsAssigned.length > 0)
      return false;

    if (this.assignment.type == AssignmentType.MultipleChoiceList && this.assignment.numberOfDuplicates <= task.studentsAssigned.length)
      return false;

    return true;
  }

  compareDates(date1: Date | number, date2: Date | number): number {
    return date1.valueOf() - date2.valueOf();
  }

  compareToNow(date: Date): number {
    return this.compareDates(date, Date.now());
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
