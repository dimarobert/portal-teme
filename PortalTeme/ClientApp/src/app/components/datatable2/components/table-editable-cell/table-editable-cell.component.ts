import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AbstractControl } from '@angular/forms';

import { ColumnDefinition, ColumnType, EditableColumnDefinition } from '../../../../models/column-definition.model';
import { isEditableColumnDefinition, isDatasourceColumnDefinition } from '../../../../type-guards/column-definitions.type-guards';
import { ItemDatasource } from '../../../../datasources/item-datasource';
import { Observable } from 'rxjs';

export interface CellState {
  isEdit: boolean;

  column: ColumnDefinition;
  rowItem: any;

  control: AbstractControl;
}

@Component({
  selector: 'app-table-editable-cell',
  templateUrl: './table-editable-cell.component.html',
  styleUrls: ['./table-editable-cell.component.scss']
})
export class TableEditableCellComponent implements OnInit {

  ColumnType = ColumnType;

  @Input() isHandset$: Observable<boolean>;

  @Input() cellState: CellState;

  @Output() sortBy: EventEmitter<ColumnDefinition> = new EventEmitter(false);

  constructor() { }

  ngOnInit() {
  }

  get editableColumn(): EditableColumnDefinition {
    if (isEditableColumnDefinition(this.cellState.column))
      return this.cellState.column;
    throw new Error(`Column ${this.cellState.column.id} is not an EditableColumnDefinition`);
  }

  get datasourceColumn(): ItemDatasource<any> {
    if (isDatasourceColumnDefinition(this.cellState.column)) {
      return this.cellState.column.datasource;
    }
    throw new Error(`Column ${this.cellState.column.id} is not a DatasourceColumnDefinition`);
  }

  get hasError(): boolean {
    const control = this.cellState.control;
    return control.touched && control.invalid;
  }

  get controlError(): string {
    let control = this.cellState.control;
    if (control.valid)
      return '';

    if (control.hasError('server'))
      return control.getError('server');

    if (control.hasError('required'))
      return `The ${this.cellState.column.title} field is required.`;

    return '';
  }

}
