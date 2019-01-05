import { ItemDatasource } from '../datasources/item-datasource';
import { ItemAccessor, NamedModelItemAccessor } from './item.accesor';

export class DataTableColumns {

    constructor(public columns: ColumnDefinition[], private defaults?: DataTableColumnsDefaults) {
        defaults = defaults || <any>{};
        if (defaults.itemAccesor == null) {
            defaults.itemAccesor = new NamedModelItemAccessor();
        }

        for (var column of columns) {
            if (column.itemAccessor == null) {
                column.itemAccessor = defaults.itemAccesor;
            }
        }
    }
}

export interface DataTableColumnsDefaults {
    itemAccesor: ItemAccessor;
}

export interface ColumnDefinition {
    id: string;
    title: string;
    itemAccessor?: ItemAccessor;
}

export interface EditableColumnDefinition extends ColumnDefinition {
    type: ColumnType;
}

export interface DatasourceColumnDefinition<T> extends EditableColumnDefinition {
    datasource: ItemDatasource<T>;
}

export enum ColumnType {
    Textbox = 0,
    Select = 1
}
