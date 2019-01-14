import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

import { Course } from '../../../models/course.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { MatTabChangeEvent } from '@angular/material';
import { CourseEditAssistantsComponent } from '../../../components/courses/course-edit-assistants/course-edit-assistants.component';
import { CourseEditAttendeesComponent } from '../../../components/courses/course-edit-attendees/course-edit-attendees.component';
import { AssignmentsManageViewComponent } from '../../../components/assignments/assignments-manage-view/assignments-manage-view.component';

@Component({
  selector: 'app-course-manage-page',
  templateUrl: './course-manage-page.component.html',
  styleUrls: ['./course-manage-page.component.scss']
})
export class CourseManagePageComponent implements OnInit, OnDestroy {
  private routerParamsSub: Subscription;

  private courseSlug: string;
  course: Course;

  @ViewChild('assistants') assistants: CourseEditAssistantsComponent;
  @ViewChild('attendees') attendees: CourseEditAttendeesComponent;
  @ViewChild('assignments') assignments: AssignmentsManageViewComponent;

  isAssignmentsTab: boolean;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {

    this.isAssignmentsTab = true;

    this.routerParamsSub = this.route.parent.parent.paramMap.subscribe(params => {
      this.courseSlug = params.get('slug');
      if (!this.courseSlug)
        throw new Error('Could not find the slug path parameter.');

      this.modelSvcFactory.courses.getBySlug(this.courseSlug)
        .pipe(take(1))
        .subscribe(courseResult => {
          this.course = courseResult;
        });

    });
  }

  onTabChange(event: MatTabChangeEvent) {
    this.isAssignmentsTab = false;

    switch (event.index) {
      case 0:
        this.isAssignmentsTab = true;
        this.assignments.update();
        break;

      case 1:
        this.assistants.update();
        break;

      case 2:
        this.attendees.update();
        break;
    }
  }

  ngOnDestroy(): void {
    this.routerParamsSub.unsubscribe();
  }
}
