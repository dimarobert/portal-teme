import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Observable, forkJoin, BehaviorSubject } from 'rxjs';

import { CourseDefinition } from '../../../models/course-definition.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { User } from '../../../models/course.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-course-basic',
  templateUrl: 'basic.component.html',
  styleUrls: ['basic.component.scss']
})
export class CourseBasicComponent implements OnInit {

  createCourseForm: FormGroup;

  courses: BehaviorSubject<CourseDefinition[]>;
  professors: BehaviorSubject<User[]>;

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {

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

  save() {

  }
}

