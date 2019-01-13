import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ModelServiceFactory } from '../../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment } from '../../../models/assignment.model';

@Component({
  selector: 'app-assignment-edit-page',
  templateUrl: './assignment-edit-page.component.html',
  styleUrls: ['./assignment-edit-page.component.scss']
})
export class AssignmentEditPageComponent implements OnInit, OnDestroy {

  routeSub: Subscription;
  assignment: Subject<Assignment>;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {
    this.assignment = new Subject();
    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const assignmentId = params.get('assignmentId');

        this.modelSvcFactory.assignments.get(assignmentId)
          .pipe(take(1))
          .subscribe(assignResult => {
            this.assignment.next(assignResult);
          });
      });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
