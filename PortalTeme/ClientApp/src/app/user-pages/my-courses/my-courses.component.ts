import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../authentication/services/auth.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { Course } from '../../models/course.model';

@Component({
  selector: 'app-my-courses',
  templateUrl: './my-courses.component.html',
  styleUrls: ['./my-courses.component.scss']
})
export class MyCoursesComponent implements OnInit {

  constructor(private auth: AuthService, private modelSvcFactory: ModelServiceFactory) { }


  courses: BehaviorSubject<Course[]>;

  ngOnInit() {
    this.courses = new BehaviorSubject([]);

    this.getData();
  }

  getData() {
    const courses$ = this.modelSvcFactory.courses.getAllRef();
  }




}
