import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { forkJoin, BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { isHttpErrorResponse } from '../../../type-guards/errors.type-guard';

import { CourseDefinition } from '../../../models/course-definition.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { User, Course } from '../../../models/course.model';
import { MenuService } from '../../../services/menu.service';

@Component({
  selector: 'app-course-edit-basic',
  templateUrl: 'course-edit-basic.component.html',
  styleUrls: ['course-edit-basic.component.scss']
})
export class CourseEditBasicComponent implements OnInit {

  editCourseForm: FormGroup;

  courses: BehaviorSubject<CourseDefinition[]>;
  professors: BehaviorSubject<User[]>;
  errors: { [key: string]: string[] };

  formReady: boolean = false;

  constructor(private modelSvcFactory: ModelServiceFactory, private route: ActivatedRoute, private router: Router, private menuService: MenuService) { }

  private editedCourse: Course;
  private courseId: string;

  ngOnInit() {
    this.errors = {};

    this.courses = new BehaviorSubject([]);
    this.professors = new BehaviorSubject([]);

    this.courseId = this.route.snapshot.paramMap.get('id');

    const course$ = this.modelSvcFactory.courses.get(this.courseId);
    const courseDefs$ = this.modelSvcFactory.courseDefinitions.getAll();
    const professors$ = this.modelSvcFactory.users.getProfessors();

    forkJoin(
      courseDefs$.pipe(take(1)),
      professors$.pipe(take(1)),
      course$.pipe(take(1))
    ).subscribe(results => {
      this.courses.next(results[0]);
      this.professors.next(results[1]);

      this.editedCourse = results[2];
      this.course.setValue(this.editedCourse.courseDef.id);
      this.professor.setValue(this.editedCourse.professor.id);
      this.formReady = true;
    });

    this.editCourseForm = new FormGroup({
      course: new FormControl(''),
      professor: new FormControl(''),
    });
  }

  get course() {
    return this.editCourseForm.get('course');
  }

  get professor() {
    return this.editCourseForm.get('professor');
  }

  hasAnyError(): boolean {
    return Object.keys(this.errors).length > 0;
  }

  update() {
    this.errors = {};
    this.modelSvcFactory.courses.update({
      id: this.courseId,
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
            const control = this.editCourseForm.get(err);
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

