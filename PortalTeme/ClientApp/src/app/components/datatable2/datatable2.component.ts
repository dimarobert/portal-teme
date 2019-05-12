import { Component, OnInit, ViewChild, Input, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatTableDataSource, MatSort, MatSortable } from '@angular/material';
import { Observable } from 'rxjs';

import { ObservableDataSource } from '../../datasources/observable.datasource';
import { ColumnDefinition, ColumnType, DataTableColumns, EditableColumnDefinition } from '../../models/column-definition.model';
import { ModelAccessor } from "../../models/model.accessor";
import { ItemDatasource } from '../../datasources/item-datasource';
import { isDatasourceColumnDefinition, isEditableColumnDefinition } from '../../type-guards/column-definitions.type-guards';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { startWith, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-data-table2',
  templateUrl: './datatable2.component.html',
  styleUrls: ['./datatable2.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTableComponent2 implements OnInit {

  ColumnType = ColumnType;

  constructor(private breakpointObserver: BreakpointObserver) { }

  @Input() modelAccessor: ModelAccessor;
  @Input() columnDefs: DataTableColumns;
  @Input() data: Observable<any[]>;
  @Input() emptyDataMessage: string;

  @Input() canAdd: boolean;
  @Input() canEdit: boolean;
  @Input() canDelete: boolean;

  @Input() commands: DataTable2Commands;

  @Input() customEdit: boolean;
  @Input() customEditAction: (element: any) => void;

  @Input() loading: boolean;

  @Input() initialSorting: { columnId: string, startDirection?: 'asc' | 'desc' };

  get displayedColumns(): string[] {
    var cols = this.columnDefs.columns.map(c => c.id);
    if (this.hasActions)
      cols.push('actions');
    return cols;
  };

  dataSource: MatTableDataSource<any>;
  hasData: boolean = true;

  errors: { [key: string]: string[] } = {};

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

    this.dataSource = new ObservableDataSource<any>(
      this.data
        .pipe(tap(items => {
          this.hasData = items.length > 0;
          this.updateAddForms(items);
        }))
    );
    this.dataSource.sort = this.sort;
    const initialSort: MatSortable = { id: 'name', disableClear: false, start: 'asc' };
    if (this.initialSorting) {
      initialSort.id = this.initialSorting.columnId;
      initialSort.start = this.initialSorting.startDirection || 'asc';
    }
    this.sort.sort(initialSort);
  }

  protected sortBy(column: ColumnDefinition) {
    const sortable = this.sort.sortables.get(column.id);
    sortable.disableClear = true;
    this.sort.sort(sortable);
  }

  private validateInput() {
    if (this.data == null)
      throw new Error('Invalid configuration. The data property is null.');
    if (this.columnDefs == null)
      throw new Error('Invalid configuration. The columnDefs property is null.');
    if (this.canAdd && this.commands.save == null)
      throw new Error('Invalid configuration. The canAdd property is true but the save callback is null.');
    if (this.canEdit && this.commands.save == null && this.commands.update == null)
      throw new Error('Invalid configuration. The canEdit property is true but the save and update callbacks are null.');
    if (this.canDelete && this.commands.delete == null)
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

  private updateAddForms(items: any[]) {
    const oldForms = new Map<object, FormGroup>(this.activeForms);
    this.activeForms = new Map<object, FormGroup>();
    items.forEach(item => {
      let form = oldForms.get(item);
      const isNewItem = this.modelAccessor.isNew(item);
      if (form) {
        this.activeForms.set(item, form);
      } else if (isNewItem) {
        form = this.createForm(item);
        this.activeForms.set(item, form);
      }
    });
  }

  private createForm(item: any): FormGroup {
    const newForm = new FormGroup({});

    this.columnDefs.columns.forEach(column => {
      newForm.addControl(column.id, new FormControl(item[column.id]));
    });

    return newForm;
  }

  add() {
    const newItem = {};
    this.commands.add(newItem);
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

  saveElement(element: any) {
    this.errors = {};
    let form = this.getForm(element);
    let itemToSave = this.modelAccessor.create(element);

    this.columnDefs.columns.forEach(column => {
      const value = form.get(column.id).value;
      column.itemAccessor.setPropertyToItem(itemToSave, column, value);
    });

    if (!this.modelAccessor.isNew(itemToSave))
      this.cancelEdit(element);
    this.executeSaveOrUpdate(element, itemToSave);
    // .then(sGroup => {
    //   var newData = this.data.value.slice();
    //   var index = newData.indexOf(element);
    //   newData[index] = sGroup;
    //   this.data.next(newData);
    //   this.hasData = newData.length > 0;

    //   this.activeForms.delete(element);
    // })
    // .catch(error => {
    //   if (isHttpErrorResponse(error)) {
    //     this.errors = error.error;
    //     for (var err in this.errors) {
    //       const control = form.get(err);
    //       if (!control)
    //         continue;

    //       control.setErrors({
    //         server: 'Server validation failed'
    //       });
    //       control.markAsTouched();
    //     }
    //   }
    // });
  }

  private executeSaveOrUpdate(oldValue: any, newValue: any): void {
    if (this.commands.update == null)
      return this.commands.save(oldValue, newValue);
    return this.modelAccessor.isNew(newValue)
      ? this.commands.save(oldValue, newValue)
      : this.commands.update(newValue);
  }

  deleteElement(element: any) {
    this.commands.delete(element);
  }

}

export interface DataTable2Commands {
  add: (element: any) => void;
  save: (oldElement: any, element: any) => void;
  update: (newElement: any) => void;
  delete: (element: any) => void;
}