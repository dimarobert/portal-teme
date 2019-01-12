import { CourseAssistant } from './../../../models/course.model';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common'

import { Subscription, Subject } from 'rxjs';
import { take } from 'rxjs/operators';

import { Course } from '../../../models/course.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { MatTabChangeEvent } from '@angular/material';
import { CourseEditAssistantsComponent } from '../../../components/courses/course-edit-assistants/course-edit-assistants.component';
import { CourseEditAttendeesComponent } from '../../../components/courses/course-edit-attendees/course-edit-attendees.component';

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

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory, private _location: Location) { }

  ngOnInit() {

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

  backClicked() {
    this._location.back();
  }

  onTabChange(event: MatTabChangeEvent) {
    if (event.index == 0)
      this.assistants.update();
    else if (event.index == 1)
      this.attendees.update();
  }

  ngOnDestroy(): void {
    this.routerParamsSub.unsubscribe();
  }
}
