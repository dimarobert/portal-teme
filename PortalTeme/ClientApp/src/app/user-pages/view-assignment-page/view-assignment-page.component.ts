import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ModelServiceFactory } from '../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment } from '../../models/assignment.model';

@Component({
  selector: 'app-view-assignment-page',
  templateUrl: './view-assignment-page.component.html',
  styleUrls: ['./view-assignment-page.component.scss']
})
export class ViewAssignmentPageComponent implements OnInit, OnDestroy {

  routeSub: Subscription;
  assignmentSlug: string;
  assignment: Assignment;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {

    this.routeSub = this.route.paramMap
      .subscribe(params => {
        this.assignmentSlug = params.get('assigSlug');

        this.modelSvcFactory.assignments.getBySlug(this.assignmentSlug)
          .pipe(take(1))
          .subscribe(assignmentResult => {
            this.assignment = assignmentResult;
          });
      });

  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
