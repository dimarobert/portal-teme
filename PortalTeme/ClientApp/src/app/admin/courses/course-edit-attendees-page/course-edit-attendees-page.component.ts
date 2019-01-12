import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-course-edit-attendees-page',
  templateUrl: './course-edit-attendees-page.component.html',
  styleUrls: ['./course-edit-attendees-page.component.scss']
})
export class CourseEditAttendeesPageComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute) { }

  routeSub: Subscription;
  courseId: string;

  ngOnInit() {
    this.routeSub = this.route.parent.paramMap
      .subscribe(params => this.courseId = params.get('id'));
  }


  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
