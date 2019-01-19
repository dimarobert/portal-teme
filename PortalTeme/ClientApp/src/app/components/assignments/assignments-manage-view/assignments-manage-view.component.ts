import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ModelServiceFactory } from '../../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment } from '../../../models/assignment.model';
import { BehaviorSubject } from 'rxjs';
import { DataTableComponent } from '../../datatable/datatable.component';
import { DataTableColumns } from '../../../models/column-definition.model';
import { nameof } from '../../../type-guards/nameof.guard';
import { CustomItemAccessor } from '../../../models/item.accesor';
import { ModelAccessor, BaseModelAccessor } from '../../../models/model.accessor';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-assignments-manage-view',
  templateUrl: './assignments-manage-view.component.html',
  styleUrls: ['./assignments-manage-view.component.scss']
})
export class AssignmentsManageViewComponent implements OnInit {

  @Input() courseId: string;

  columnDefs: DataTableColumns;
  assignments: BehaviorSubject<Assignment[]>;
  modelAccessor: ModelAccessor;

  initialSorting: { columnId: string };

  @ViewChild('assignmentsTable') assignmentsTable: DataTableComponent;

  constructor(private route: ActivatedRoute, private router: Router, private modelSvcFactory: ModelServiceFactory, private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.modelAccessor = new BaseModelAccessor();

    this.initialSorting = {
      columnId: nameof<Assignment>('startDate')
    }

    this.assignments = new BehaviorSubject([]);

    this.columnDefs = new DataTableColumns([
      {
        id: nameof<Assignment>('name'),
        title: 'Name'
      }, {
        id: nameof<Assignment>('startDate'),
        title: 'Start Date',
        itemAccessor: new CustomItemAccessor<Assignment>(item => {
          const date = new Date(item.startDate);
          return date.toLocaleDateString();
        })
      }, {
        id: nameof<Assignment>('endDate'),
        title: 'End Date',
        itemAccessor: new CustomItemAccessor<Assignment>(item => {
          const date = new Date(item.endDate);
          return date.toLocaleDateString();
        })
      }, {
        id: 'createdOn',
        title: 'Created on',
        itemAccessor: new CustomItemAccessor<Assignment>(item => item.dateAdded.toLocaleDateString())
      }, {
        id: 'lastUpdate',
        title: 'Last updated',
        itemAccessor: new CustomItemAccessor<Assignment>(item => item.lastUpdated.toLocaleDateString())
      }
    ]);

    this.getData();
  }

  private getData() {
    this.delete = this.delete.bind(this);
    this.edit = this.edit.bind(this);


    this.assignmentsTable.loading = true;

    this.modelSvcFactory.assignments.getByCourse(this.courseId)
      .pipe(take(1))
      .subscribe(courseAssignments => {
        this.assignments.next(courseAssignments);

        this.assignmentsTable.loading = false;
      });
  }

  update() {
    this.getData();
  }

  edit(assignment: Assignment) {
    this.router.navigate(['assignment', assignment.id], { relativeTo: this.route });
  }

  delete(assignment: Assignment): Promise<Assignment> {
    return this.modelSvcFactory.assignments.delete(assignment.id);
  }
}
