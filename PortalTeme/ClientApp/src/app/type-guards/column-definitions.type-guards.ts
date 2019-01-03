import { ColumnDefinition, DatasourceColumnDefinition } from '../models/column-definition.model';

export function isDatasourceColumnDefinition<T>(column: ColumnDefinition): column is DatasourceColumnDefinition<T> {
    return !!column['datasource'];
}
