import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment, AssignmentType, UserAssignment, AssignmentTask } from '../../models/assignment.model';
import { SettingsProvider } from '../../services/settings.provider';
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

  constructor(private route: ActivatedRoute, private settingsProvider: SettingsProvider, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {

    this.routeSub = combineLatest(
      this.route.parent.paramMap,
      this.route.paramMap
    ).subscribe(([course, assignment]) => {
      const courseSlug = course.get('slug');
      const assignmentSlug = assignment.get('assigSlug');

      this.modelSvcFactory.assignments.getBySlug(courseSlug, assignmentSlug)
        .pipe(take(1))
        .subscribe(assignmentResult => {
          this.assignment = assignmentResult;
        });
    });

  }

  chooseTask(task: AssignmentTask) {
    
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
