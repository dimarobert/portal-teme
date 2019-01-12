import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ModelServiceFactory } from '../../../services/model.service';
import { take } from 'rxjs/operators';
import { Assignment } from '../../../models/assignment.model';
import { BehaviorSubject } from 'rxjs';
import { DataTableComponent } from '../../datatable/datatable.component';
import { DataTableColumns } from '../../../models/column-definition.model';
import { nameof } from '../../../type-guards/nameof.guard';
import { CustomItemAccessor } from '../../../models/item.accesor';

@Component({
  selector: 'app-assignments-manage-view',
  templateUrl: './assignments-manage-view.component.html',
  styleUrls: ['./assignments-manage-view.component.scss']
})
export class AssignmentsManageViewComponent implements OnInit {


  @Input() courseId: string;

  columnDefs: DataTableColumns;
  assignments: BehaviorSubject<Assignment[]>;

  @ViewChild('assignmentsTable') assignmentsTable: DataTableComponent;

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  ngOnInit() {
    this.assignments = new BehaviorSubject([]);

    this.columnDefs = new DataTableColumns([
      {
        id: nameof<Assignment>('name'),
        title: 'Name'
      }, {
        id: nameof<Assignment>('startDate'),
        title: 'Start Date'
      }, {
        id: nameof<Assignment>('endDate'),
        title: 'End Date'
      }, {
        id: 'timestamps',
        title: '',
        itemAccessor: new CustomItemAccessor<Assignment>(item => `${item.dateAdded} (updated: ${item.lastUpdated})`)
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

  protected edit(assignment: Assignment) {

  }

  protected delete(assignment: Assignment): Promise<Assignment> {
    return this.modelSvcFactory.assignments.delete(assignment.id);
  }
}
