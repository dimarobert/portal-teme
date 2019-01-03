import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { StudyGroup } from '../../models/study-group.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ObservableDataSource } from '../../datasources/observable.datasource';
import { StudyDomain } from '../../models/study-domain.model';
import { isHttpErrorResponse } from '../../type-guards/errors.type-guard';
import { ColumnDefinition, ColumnType, ItemAccessor } from '../../models/column-definition.model';
import { ItemDatasource } from '../../datasources/item-datasource';
import { isDatasourceColumnDefinition } from '../../type-guards/column-definitions.type-guards';

@Component({
  selector: 'app-data-table',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss']
})
export class DataTableComponent implements OnInit {

  ColumnType = ColumnType;

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  @Input() columnDefs: ColumnDefinition[] = [];
  @Input() dataSource: MatTableDataSource<StudyGroup>;
  @Input() itemAccessor: ItemAccessor<StudyDomain>;
  @Input() data: BehaviorSubject<StudyGroup[]>;

  displayedColumns: string[] = [];

  hasData: boolean;

  errors: { [key: string]: string[] };

  @ViewChild(MatSort) sort: MatSort;

  addForms: Map<object, FormGroup> = new Map<object, FormGroup>();

  ngOnInit() {
    this.errors = {};
    this.hasData = true;

    this.displayedColumns = [...this.columnDefs.map(c => c.id), 'actions'];

    this.dataSource = new ObservableDataSource<StudyGroup>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  getDatasource(column: ColumnDefinition): ItemDatasource<any> {
    if (isDatasourceColumnDefinition(column)) {
      return column.datasource;
    }
    throw new Error(`Column ${column.id} is not a DatasourceColumnDefinition`);
  }

  hasAnyError(): boolean {
    return Object.keys(this.errors).length > 0;
  }

  hasError(element: object, field: string): boolean {
    return this.getFormControl(element, field).invalid;
  }

  getError(element: object, field: string): string {
    let control = this.getFormControl(element, field);
    if (control.valid)
      return '';

    if (control.hasError('server'))
      return control.getError('server');

    if (control.hasError('required'))
      return `The ${field} field is required.`;

    return '';
  }

  getForm(row: object): AbstractControl {
    return this.addForms.get(row);
  }

  getFormControl(row: object, field: string): AbstractControl {
    return this.getForm(row).get(field);
  }

  add() {
    const newRow = {};
    const newAddForm = new FormGroup({});

    this.columnDefs.forEach(column => {
      newAddForm.addControl(column.id, new FormControl(''));
    });

    this.addForms.set(newRow, newAddForm);

    var newData = this.data.value.slice();
    newData.push(<any>newRow);
    this.data.next(newData);
    this.hasData = true;
  }

  remove(element: any) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
    this.hasData = newData.length > 0;

    this.addForms.delete(element);
  }

  save(element: StudyGroup) {
    this.errors = {};
    let form = this.getForm(element);
    let value: any = {};

    this.columnDefs.forEach(column => {
      value[column.id] = form.get(column.id).value;
    });

    this.modelSvcFactory.studyGroups.save(value)
      .then(sGroup => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = sGroup;
        this.data.next(newData);
        this.hasData = newData.length > 0;

        this.addForms.delete(element);
      })
      .catch(error => {
        if (isHttpErrorResponse(error)) {
          this.errors = error.error;
          for (var err in this.errors) {
            const control = form.get(err);
            if (!control)
              continue;

            control.setErrors({
              server: 'Server validation failed' //this.errors[err][0]
            });
            control.markAsTouched();
          }
        }
      });
  }

  delete(element: StudyGroup) {
    this.modelSvcFactory.studyGroups.delete(element)
      .then(sGroup => {
        this.remove(element);
      });
  }

}
