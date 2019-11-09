import { Component, OnInit, ViewChild, Input, ChangeDetectionStrategy, AfterViewInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortable } from '@angular/material/sort';
import { Observable, merge, Subject } from 'rxjs';
import { startWith, map, scan, distinctUntilChanged } from 'rxjs/operators';

import { ObservableDataSource } from '../../datasources/observable.datasource';
import { ColumnDefinition, ColumnType, DataTableColumns } from '../../models/column-definition.model';
import { ModelAccessor } from "../../models/model.accessor";
import { CellState } from './components/table-editable-cell/table-editable-cell.component';

interface RowState {
  initialData: any;

  isEdit: boolean;
  editForm: FormGroup;

  cells: Cells
}

interface Cells {
  [key: string]: CellState;
}

interface TableState {
  rows: RowState[];
}

@Component({
  selector: 'app-data-table2',
  templateUrl: './datatable2.component.html',
  styleUrls: ['./datatable2.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTableComponent2 implements OnInit, AfterViewInit {

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

  dataSource: MatTableDataSource<RowState>;
  hasData: boolean = true;

  errors: { [key: string]: string[] } = {};

  @ViewChild(MatSort, { static: false }) sort: MatSort;

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

  private programaticCommands$: Subject<{ cmd: string, [key: string]: any }> = new Subject();

  private commands$: Observable<{ cmd: string, [key: string]: any }>;

  private currentState$: Observable<TableState>;

  private rows$: Observable<RowState[]>;

  ngOnInit() {
    this.validateInput();

    this.loading = this.loading || false;

    this.initializeObservables();
  }

  ngAfterViewInit(): void {
    this.dataSource = new ObservableDataSource<RowState>(this.rows$);
    this.dataSource.sort = this.sort;
    const initialSort: MatSortable = { id: 'name', disableClear: false, start: 'asc' };
    if (this.initialSorting) {
      initialSort.id = this.initialSorting.columnId;
      initialSort.start = this.initialSorting.startDirection || 'asc';
    }
    this.sort.sort(initialSort);
    const origSDA = this.dataSource.sortingDataAccessor;
    this.dataSource.sortingDataAccessor = (data, headerId) => {
      return origSDA(data.initialData, headerId);
    };
  }

  private initializeObservables(): void {
    this.commands$ = merge(
      this.programaticCommands$.asObservable(),
      this.data.pipe(map(rows => ({ cmd: 'updateRows', rows: rows })))
    );

    this.currentState$ = this.commands$.pipe(
      startWith(<TableState>{
        rows: []
      }),
      scan((cState: TableState, command: { cmd: string, [key: string]: any }): TableState => {

        if (command.cmd == 'updateRows') {
          const newRows: any[] = command.rows;
          const rows = newRows.map<RowState>(row => {

            const currentRowState = cState.rows.find(e => e.initialData == row);

            // preserve the previous state as the row did not change
            if (currentRowState)
              return currentRowState;

            // this is a new or changed row. create new state
            let isEdit = false;
            let editForm: FormGroup;

            // force new (not saved) rows in edit mode
            if (this.modelAccessor.isNew(row)) {
              isEdit = true;
              editForm = this.createForm(row);
            }

            let cells = this.columnDefs.columns.reduce<Cells>((map, col) => {
              map[col.id] = {
                isEdit: isEdit,
                column: col,
                rowItem: row,
                control: isEdit ? editForm.get(col.id) : null
              };
              return map;
            }, {});

            return {
              isEdit: isEdit,
              initialData: row,
              editForm: editForm,
              cells: cells
            }

          });

          return { ...cState, rows };
        } else if (command.cmd == 'setEdit') {
          const row: RowState = command.state;

          const editForm = this.createForm(row.initialData);
          const editedRow = { ...row, editForm, isEdit: true };
          editedRow.cells = Object.keys(editedRow.cells).reduce<Cells>((acc, colName) => {
            const cell = editedRow.cells[colName];
            acc[colName] = { ...cell, isEdit: true, control: editForm.get(cell.column.id) };
            return acc;
          }, {});

          const rowIndex = cState.rows.indexOf(row);
          let rows = cState.rows.filter(r => r != row);
          rows.splice(rowIndex, 0, editedRow);
          return { ...cState, rows };

        } else if (command.cmd == 'cancelEdit') {
          const row: RowState = command.state;

          const editedRow = { ...row, editForm: null, isEdit: false };
          editedRow.cells = Object.keys(editedRow.cells).reduce<Cells>((acc, colName) => {
            acc[colName] = { ...editedRow.cells[colName], isEdit: false, control: null };
            return acc;
          }, {});

          const rowIndex = cState.rows.indexOf(row);
          let rows = cState.rows.filter(r => r != row);
          rows.splice(rowIndex, 0, editedRow);
          return { ...cState, rows };
        }

        return cState;
      })
    );

    this.rows$ = this.currentState$.pipe(map(state => state.rows), distinctUntilChanged());
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

  hasAnyError(): boolean {
    return Object.keys(this.errors).length > 0;
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

  edit(state: RowState) {
    this.programaticCommands$.next({ cmd: 'setEdit', state: state });
  }

  cancelEdit(state: RowState) {
    this.programaticCommands$.next({ cmd: 'cancelEdit', state: state });

    this.errors = {};
  }

  saveElement(state: RowState) {
    this.errors = {};
    let form = state.editForm;
    let itemToSave = this.modelAccessor.create(state.initialData);

    this.columnDefs.columns.forEach(column => {
      const value = form.get(column.id).value;
      column.itemAccessor.setPropertyToItem(itemToSave, column, value);
    });

    if (!this.modelAccessor.isNew(itemToSave))
      this.cancelEdit(state);
    this.executeSaveOrUpdate(state.initialData, itemToSave);
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

  deleteElement(state: RowState) {
    this.commands.delete(state.initialData);
  }

}

export interface DataTable2Commands {
  add: (element: any) => void;
  save: (oldElement: any, element: any) => void;
  update: (newElement: any) => void;
  delete: (element: any) => void;
}