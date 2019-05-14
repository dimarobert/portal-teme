import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { nameof } from '../../type-guards/nameof.guard';

import { Year } from '../../models/year.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ColumnType, EditableColumnDefinition, DataTableColumns } from '../../models/column-definition.model';
import { ModelAccessor, BaseModelAccessor } from '../../models/model.accessor';
import { DataTable2Commands } from '../../components/datatable2/datatable2.component';

@Component({
  selector: 'app-academic-years',
  templateUrl: './academic-years.component.html',
  styleUrls: ['./academic-years.component.scss']
})
export class AcademicYearsComponent implements OnInit, DataTable2Commands {

  commands = this;

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: DataTableColumns;
  modelAccessor: ModelAccessor;
  data: Observable<Year[]>;
  loading$: Observable<boolean>;


  ngOnInit() {
    this.data = this.modelSvcFactory.years.model$;
    this.modelSvcFactory.years.refresh();
    this.loading$ = this.modelSvcFactory.years.loading$;

    this.modelAccessor = new BaseModelAccessor();

    this.columnDefs = new DataTableColumns([
      <EditableColumnDefinition>{
        id: nameof<Year>('name'),
        title: 'Name',
        type: ColumnType.Textbox
      }
    ]);

  }

  add(element: Year): void {
    this.modelSvcFactory.years.add(element);
  }

  save(oldElement: Year, element: Year): void {
    this.modelSvcFactory.years.save(oldElement, element);
  }

  update(element: Year): void {
    this.modelSvcFactory.years.update(element);
  }

  delete(element: Year): void {
    this.modelSvcFactory.years.delete(element);
  }

}
