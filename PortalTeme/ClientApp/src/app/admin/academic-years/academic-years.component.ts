import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { nameof } from '../../type-guards/nameof.guard';

import { Year } from '../../models/year.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ColumnType, EditableColumnDefinition, DataTableColumns } from '../../models/column-definition.model';
import { ModelAccessor, BaseModelAccessor } from '../../models/model.accessor';

@Component({
  selector: 'app-academic-years',
  templateUrl: './academic-years.component.html',
  styleUrls: ['./academic-years.component.scss']
})
export class AcademicYearsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: DataTableColumns;
  modelAccessor: ModelAccessor;
  data: BehaviorSubject<Year[]>;


  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);

    this.modelAccessor = new BaseModelAccessor();

    this.columnDefs = new DataTableColumns([
      <EditableColumnDefinition>{
        id: nameof<Year>('name'),
        title: 'Name',
        type: ColumnType.Textbox
      }
    ]);

    this.modelSvcFactory.years.getAll()
      .pipe(take(1))
      .subscribe(response => {
        this.data.next(response);
      });
  }

  save(element: Year): Promise<Year> {
    return this.modelSvcFactory.years.save(element);
  }

  delete(element: Year): Promise<Year> {
    return this.modelSvcFactory.years.delete(element.id);
  }

}
