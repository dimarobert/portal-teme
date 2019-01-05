import { ColumnDefinition, DatasourceColumnDefinition, EditableColumnDefinition } from '../models/column-definition.model';

export function isDatasourceColumnDefinition<T>(column: ColumnDefinition): column is DatasourceColumnDefinition<T> {
    return column.hasOwnProperty('datasource') && column['datasource'] != null;
}

export function isEditableColumnDefinition<T>(column: ColumnDefinition): column is EditableColumnDefinition {
    return column.hasOwnProperty('type');
}