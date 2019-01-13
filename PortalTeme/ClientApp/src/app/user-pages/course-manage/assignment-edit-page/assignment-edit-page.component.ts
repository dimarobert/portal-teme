import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ModelServiceFactory } from '../../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment, AssignmentEdit } from '../../../models/assignment.model';
import { assertDataInRange } from '@angular/core/src/render3/state';

@Component({
  selector: 'app-assignment-edit-page',
  templateUrl: './assignment-edit-page.component.html',
  styleUrls: ['./assignment-edit-page.component.scss']
})
export class AssignmentEditPageComponent implements OnInit, OnDestroy {

  routeSub: Subscription;
  assignment: Subject<Assignment>;
  assignmentSnapshoot: Assignment;

  constructor(private route: ActivatedRoute, private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {
    this.update = this.update.bind(this);

    this.assignment = new Subject();
    this.routeSub = this.route.paramMap
      .subscribe(params => {
        const assignmentId = params.get('assignmentId');

        this.modelSvcFactory.assignments.get(assignmentId)
          .pipe(take(1))
          .subscribe(assignResult => {
            this.assignment.next(assignResult);
            this.assignmentSnapshoot = assignResult;
          });
      });
  }

  update(assignment: AssignmentEdit) {
    assignment.id = this.assignmentSnapshoot.id;
    this.modelSvcFactory.assignments.update(assignment);
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }
}
