import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {Location } from '@angular/common'

import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

import { Course } from '../../../models/course.model';
import { ModelServiceFactory } from '../../../services/model.service';


@Component({
  selector: 'app-course-manage-page',
  templateUrl: './course-manage-page.component.html',
  styleUrls: ['./course-manage-page.component.scss']
})
export class CourseManagePageComponent implements OnInit, OnDestroy {
  private routerParamsSub: Subscription;

  private courseSlug: string;
  course: Course;

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

  ngOnDestroy(): void {
    this.routerParamsSub.unsubscribe();
  }
}
