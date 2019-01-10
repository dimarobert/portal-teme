import { Component, OnInit, OnDestroy } from '@angular/core';
import { MenuService, CollapseOption } from '../../services/menu.service';
import { Course } from '../../models/course.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-course-page',
  templateUrl: './course-page.component.html',
  styleUrls: ['./course-page.component.scss']
})
export class CoursePageComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, private menuService: MenuService, private modelSvcFactory: ModelServiceFactory) { }

  course: Course;
  private courseSlug: string;
  private routerParamsSub: Subscription;
  ngOnInit() {
    this.menuService.setCourseCollapse(CollapseOption.Open);

    this.routerParamsSub = this.route.paramMap.subscribe(params => {
      this.courseSlug = params.get('slug');

      this.modelSvcFactory.courses.getBySlug(this.courseSlug)
        .pipe(take(1))
        .subscribe(courseResult => {
          this.course = courseResult;
        });

    });
  }

  ngOnDestroy(): void {
    this.routerParamsSub.unsubscribe();
  }

}
