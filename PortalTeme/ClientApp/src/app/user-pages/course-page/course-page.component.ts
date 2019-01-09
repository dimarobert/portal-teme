import { Component, OnInit } from '@angular/core';
import { MenuService, CollapseOption } from '../../services/menu.service';
import { Course } from '../../models/course.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-course-page',
  templateUrl: './course-page.component.html',
  styleUrls: ['./course-page.component.scss']
})
export class CoursePageComponent implements OnInit {

  constructor(private route: ActivatedRoute, private menuService: MenuService, private modelSvcFactory: ModelServiceFactory) { }

  course: Course;
  private courseSlug: string;

  ngOnInit() {
    this.menuService.setCourseCollapse(CollapseOption.Open);

    this.courseSlug = this.route.snapshot.paramMap.get('slug');

    this.modelSvcFactory.courses.getBySlug(this.courseSlug)
      .pipe(take(1))
      .subscribe(courseResult => {
        this.course = courseResult;
      });
  }

}
