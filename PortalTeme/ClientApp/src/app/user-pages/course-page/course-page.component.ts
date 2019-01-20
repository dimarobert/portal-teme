import { Component, OnInit, OnDestroy } from '@angular/core';
import { Course, User } from '../../models/course.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Assignment } from '../../models/assignment.model';

@Component({
  selector: 'app-course-page',
  templateUrl: './course-page.component.html',
  styleUrls: ['./course-page.component.scss'],
  animations: [
    trigger('openClose', [
      state('closed', style({
        height: 0
      })),
      state('open', style({})),
      transition('open <=> closed', [
        animate('.2s')
      ])
    ]),
    trigger('openCloseArrowIcon', [
      state('closed', style({})),
      state('open', style({
        transform: 'rotate(180deg)'
      })),
      transition('open <=> closed', [
        animate('.2s')
      ])
    ])
  ]
})
export class CoursePageComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  showCourseDetails: boolean = false;

  course: Course;
  private courseSlug: string;
  private routerParamsSub: Subscription;

  ngOnInit() {

    this.routerParamsSub = this.route.paramMap.subscribe(params => {
      this.courseSlug = params.get('slug');

      this.modelSvcFactory.courses.getBySlug(this.courseSlug)
        .pipe(take(1))
        .subscribe(courseResult => {
          this.course = courseResult;
          this.course.assignments = this.course.assignments.sort((first, second) => first.endDate.valueOf() - second.endDate.valueOf());
        });

    });
  }

  toggleShowCourseDetails() {
    this.showCourseDetails = !this.showCourseDetails;
  }

  get professorText(): string {
    return `Prof. ${this.course.professor.firstName} ${this.course.professor.lastName}`;
  }

  get assistantsTitleText(): string {
    const plural = this.course.assistants.length > 1 ? 's' : '';
    return `Assistant${plural}:`;
  }

  getAssistantText(assistant: User): string {
    return `${assistant.firstName} ${assistant.lastName}`;
  }

  ngOnDestroy(): void {
    this.routerParamsSub.unsubscribe();
  }

}
