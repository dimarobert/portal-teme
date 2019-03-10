import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin, BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { isHttpErrorResponse } from '../../../type-guards/errors.type-guard';
import { CourseDefinition } from '../../../models/course-definition.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { User } from '../../../models/course.model';
import { MenuService } from '../../../services/menu.service';

@Component({
  selector: 'app-course-create',
  templateUrl: './course-create.component.html',
  styleUrls: ['./course-create.component.scss']
})
export class CourseCreateComponent implements OnInit {

  createCourseForm: FormGroup;

  courses: BehaviorSubject<CourseDefinition[]>;
  professors: BehaviorSubject<User[]>;
  errors: { [key: string]: string[] };

  constructor(private modelSvcFactory: ModelServiceFactory, private route: ActivatedRoute, private router: Router, private menuService: MenuService) { }

  ngOnInit() {
    this.errors = {};
    this.courses = new BehaviorSubject([]);
    this.professors = new BehaviorSubject([]);

    const courseDefs$ = this.modelSvcFactory.courseDefinitions.getAll();
    const professors$ = this.modelSvcFactory.users.getProfessors();

    forkJoin(
      courseDefs$.pipe(take(1)),
      professors$.pipe(take(1))
    ).subscribe(results => {
      this.courses.next(results[0]);
      this.professors.next(results[1]);
    })

    this.createCourseForm = new FormGroup({
      course: new FormControl(),
      professor: new FormControl(),
    })
  }

  get course() {
    return this.createCourseForm.get('course');
  }

  get professor() {
    return this.createCourseForm.get('professor');
  }

  hasAnyError(): boolean {
    return Object.keys(this.errors).length > 0;
  }

  save() {
    this.errors = {};
    this.modelSvcFactory.courses.save({
      courseDef: {
        id: this.course.value
      },
      professor: {
        id: this.professor.value
      }
    })
      .then(result => {
        this.router.navigate(['../', result.id, 'assistants'], { relativeTo: this.route });
        this.menuService.refreshCourses();
      }).catch(error => {
        if (isHttpErrorResponse(error)) {
          this.errors = error.error;
          for (var err in this.errors) {
            const control = this.createCourseForm.get(err);
            if (!control)
              continue;

            control.setErrors({
              server: true
            });
            control.markAsTouched();
          }
        }
      });
  }

}
