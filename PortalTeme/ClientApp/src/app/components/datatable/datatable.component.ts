import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject, Observable } from 'rxjs';

import { ObservableDataSource } from '../../datasources/observable.datasource';
import { isHttpErrorResponse } from '../../type-guards/errors.type-guard';
import { ColumnDefinition, ColumnType, DataTableColumns, EditableColumnDefinition } from '../../models/column-definition.model';
import { ItemAccessor } from "../../models/item.accesor";
import { ModelAccessor } from "../../models/model.accessor";
import { ItemDatasource } from '../../datasources/item-datasource';
import { isDatasourceColumnDefinition, isEditableColumnDefinition } from '../../type-guards/column-definitions.type-guards';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { startWith, map } from 'rxjs/operators';

@Component({
  selector: 'app-data-table',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss']
})
export class DataTableComponent implements OnInit {

  ColumnType = ColumnType;

  constructor(private breakpointObserver: BreakpointObserver) { }

  @Input() modelAccessor: ModelAccessor;
  @Input() columnDefs: DataTableColumns;
  @Input() data: BehaviorSubject<any[]>;
  @Input() emptyDataMessage: string;

  @Input() canAdd: boolean;
  @Input() canEdit: boolean;
  @Input() canDelete: boolean;

  @Input() save: (element: any) => Promise<any>;
  @Input() update: (element: any) => Promise<any>;
  @Input() delete: (element: any) => Promise<any>;

  @Input() customEdit: boolean;
  @Input() customEditAction: (element: any) => void;

  @Input() loading: boolean;

  get displayedColumns(): string[] {
    var cols = this.columnDefs.columns.map(c => c.id);
    if (this.hasActions)
      cols.push('actions');
    return cols;
  };

  dataSource: MatTableDataSource<any>;
  hasData: boolean;

  errors: { [key: string]: string[] };

  @ViewChild(MatSort) sort: MatSort;

  activeForms: Map<object, FormGroup> = new Map<object, FormGroup>();

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 650px)')
    .pipe(
      // mobile first design
      // this is used as an workaround for https://github.com/angular/material2/issues/13852
      startWith(<BreakpointState>{ matches: true }),
      map(result => result.matches)
    );

  isSmallScreen$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 831px) AND (min-width: 650px)')
    .pipe(
      // mobile first design
      // this is used as an workaround for https://github.com/angular/material2/issues/13852
      startWith(<BreakpointState>{ matches: true }),
      map(result => result.matches)
    );

  get hasActions(): boolean {
    return this.canAdd || this.canEdit || this.canDelete;
  }

  get hasEditCapabilities(): boolean {
    return this.canAdd || this.canEdit;
  }

  isInEditMode(element: any): boolean {
    if (!this.hasEditCapabilities)
      return false;

    return this.modelAccessor.isNew(element) || this.getForm(element) != null;
  }

  ngOnInit() {
    this.validateInput();

    this.loading = this.loading || false;

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

  sortBy(column: ColumnDefinition) {
    const sortable = this.sort.sortables.get(column.id);
    sortable.disableClear = true;
    this.sort.sort(sortable);
  }

  private validateInput() {
    if (this.data == null)
      throw new Error('Invalid configuration. The data property is null.');
    if (this.columnDefs == null)
      throw new Error('Invalid configuration. The columnDefs property is null.');
    if (this.canAdd && this.save == null)
      throw new Error('Invalid configuration. The canAdd property is true but the save callback is null.');
    if (this.canEdit && this.save == null && this.update == null)
      throw new Error('Invalid configuration. The canEdit property is true but the save and update callbacks are null.');
    if (this.canDelete && this.delete == null)
      throw new Error('Invalid configuration. The canDelete property is true but the delete callback is null.');

    if (this.canEdit && this.customEdit)
      throw new Error('Invalid configuration. The canEdit and customEdit properties cannot be both true.');
    if (this.customEdit && this.customEditAction == null)
      throw new Error('Invalid configuration. The customEdit property is true but the customEditAction callback is null.');
  }

  getDatasource(column: ColumnDefinition): ItemDatasource<any> {
    if (isDatasourceColumnDefinition(column)) {
      return column.datasource;
    }
    throw new Error(`Column ${column.id} is not a DatasourceColumnDefinition`);
  }

  getEditableColumn(column: ColumnDefinition): EditableColumnDefinition {
    if (isEditableColumnDefinition(column))
      return column;
    throw new Error(`Column ${column.id} is not an EditableColumnDefinition`);
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
    return this.activeForms.get(row);
  }

  getFormControl(row: object, field: string): AbstractControl {
    return this.getForm(row).get(field);
  }

  add() {
    const newRow = {};
    const newAddForm = new FormGroup({});

    this.columnDefs.columns.forEach(column => {
      newAddForm.addControl(column.id, new FormControl(''));
    });

    this.activeForms.set(newRow, newAddForm);

    var newData = this.data.value.slice();
    newData.push(newRow);
    this.data.next(newData);
    this.hasData = true;
  }

  edit(element: any) {
    const newEditForm = new FormGroup({});

    this.columnDefs.columns.forEach(column => {
      newEditForm.addControl(column.id, new FormControl(element[column.id]));
    });

    this.activeForms.set(element, newEditForm);
  }

  cancelEdit(element: any) {
    this.activeForms.delete(element);
    this.errors = {};
  }

  remove(element: any) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
    this.hasData = newData.length > 0;

    this.activeForms.delete(element);
    this.errors = {};
  }

  saveElement(element: any) {
    this.errors = {};
    let form = this.getForm(element);
    let itemToSave = this.modelAccessor.create(element);

    this.columnDefs.columns.forEach(column => {
      const value = form.get(column.id).value;
      column.itemAccessor.setPropertyToItem(itemToSave, column, value);
    });

    this.executeSaveOrUpdate(itemToSave)
      .then(sGroup => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = sGroup;
        this.data.next(newData);
        this.hasData = newData.length > 0;

        this.activeForms.delete(element);
      })
      .catch(error => {
        if (isHttpErrorResponse(error)) {
          this.errors = error.error;
          for (var err in this.errors) {
            const control = form.get(err);
            if (!control)
              continue;

            control.setErrors({
              server: 'Server validation failed'
            });
            control.markAsTouched();
          }
        }
      });
  }

  private executeSaveOrUpdate(value: any): Promise<any> {
    if (this.update == null)
      return this.save(value);
    return this.modelAccessor.isNew(value)
      ? this.save(value)
      : this.update(value);
  }

  deleteElement(element: any) {
    this.delete(element)
      .then(sGroup => {
        this.remove(element);
      });
  }

}
