import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../authentication/services/auth.service';
import { Observable } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { Course } from '../../models/course.model';

@Component({
  selector: 'app-my-courses',
  templateUrl: './my-courses.component.html',
  styleUrls: ['./my-courses.component.scss']
})
export class MyCoursesComponent implements OnInit {

  constructor(private auth: AuthService, private modelSvcFactory: ModelServiceFactory) { }


  ngOnInit() {

  }

  //get courses(): Observable<Course>{

  //}

  get canAddCourses$(): Observable<boolean> {
    return this.auth.canAddCourse();
  }


}
