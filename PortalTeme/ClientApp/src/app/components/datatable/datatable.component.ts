import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { ObservableDataSource } from '../../datasources/observable.datasource';
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

  constructor() { }

  @Input() columnDefs: ColumnDefinition[] = [];
  @Input() itemAccessor: ItemAccessor<any>;
  @Input() data: BehaviorSubject<any[]>;
  @Input() emptyDataMessage: string;

  @Input() canAdd: boolean;
  @Input() canEdit: boolean;
  @Input() canDelete: boolean;

  @Input() save: (element: any) => Promise<any>;
  @Input() delete: (element: any) => Promise<any>;

  get displayedColumns(): string[] {
    var cols = this.columnDefs.map(c => c.id);
    if (this.hasActions)
      cols.push('actions');
    return cols;
  };

  dataSource: MatTableDataSource<any>;
  hasData: boolean;

  errors: { [key: string]: string[] };

  @ViewChild(MatSort) sort: MatSort;

  addForms: Map<object, FormGroup> = new Map<object, FormGroup>();

  get hasActions(): boolean {
    return this.canAdd || this.canEdit || this.canDelete;
  }

  ngOnInit() {
    this.validateInput();

    this.errors = {};
    this.hasData = true;
    let initialData = true;

    this.data.subscribe(items => {
      if (initialData && items.length == 0) {
        initialData = false;
      } else
        this.hasData = items.length > 0;
    });

    this.dataSource = new ObservableDataSource<any>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  private validateInput() {
    if (this.data == null)
      throw new Error('Invalid configuration. The data property is null.');
    if (this.columnDefs == null)
      throw new Error('Invalid configuration. The columnDefs property is null.');
    if (this.itemAccessor == null)
      throw new Error('Invalid configuration. The itemAccessor property is null.');
    if (this.canAdd && this.save == null)
      throw new Error('Invalid configuration. The canAdd property is true but the save callback is null.');
    if (this.canEdit && this.save == null)
      throw new Error('Invalid configuration. The canEdit property is true but the save callback is null.');
    if (this.canDelete && this.delete == null)
      throw new Error('Invalid configuration. The canDelete property is true but the delete callback is null.');
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
    const control = this.getFormControl(element, field);
    return control.touched && control.invalid;
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
    newData.push(newRow);
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

  saveElement(element: any) {
    this.errors = {};
    let form = this.getForm(element);
    let value = {};

    this.columnDefs.forEach(column => {
      value[column.id] = form.get(column.id).value;
    });

    this.save(value)
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

  deleteElement(element: any) {
    this.delete(element)
      .then(sGroup => {
        this.remove(element);
      });
  }

}
