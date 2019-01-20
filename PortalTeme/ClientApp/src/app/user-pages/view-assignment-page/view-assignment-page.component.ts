import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment, AssignmentType, UserAssignment } from '../../models/assignment.model';
import { SettingsProvider } from '../../services/settings.provider';

@Component({
  selector: 'app-view-assignment-page',
  templateUrl: './view-assignment-page.component.html',
  styleUrls: ['./view-assignment-page.component.scss']
})
export class ViewAssignmentPageComponent implements OnInit, OnDestroy {

  AssignmentType = AssignmentType;

  routeSub: Subscription;
  assignment: UserAssignment;

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

  get hasTasksList(): boolean {
    return this.assignment.type != AssignmentType.SingleTask
      && this.assignment.type != AssignmentType.CustomAssignedTasks;
  }

  get showTasksList(): boolean {
    return this.hasTasksList && this.assignment.assignedTask == null;
  }

  get showUserSubmissions(): boolean {
    return this.assignment.assignedTask != null;
  }
  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
