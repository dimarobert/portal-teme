import { ItemDatasource } from '../datasources/item-datasource';
import { NamedModel } from './base.model';
import { isDatasourceColumnDefinition } from '../type-guards/column-definitions.type-guards';

export interface ColumnDefinition {
    id: string;
    title: string;
    type: ColumnType;
}

export interface DatasourceColumnDefinition<T> extends ColumnDefinition {
    datasource: ItemDatasource<T>;
}

export enum ColumnType {
    Textbox = 0,
    Select = 1
}

export interface ItemAccessor<T> {

    /**
     * This returns true if the item is newly added and the inputs must be shown.
     * @param item 
     */
    isNew(item: T): boolean;

    getPropertyText(item: T, column: ColumnDefinition): string;
    getPropertyValue(item: T, column: ColumnDefinition): any;
}

export class NamedModelItemAccessor<T extends NamedModel> implements ItemAccessor<T> {

    getPropertyText(item: T, column: ColumnDefinition): string {
        if (!Object.keys(item).find(key => key === column.id))
            throw new Error(`The item does not contain the ${column.id} property. (item.id=${item.id})`);

        const value = this.getPropertyValue(item, column);
        if (isDatasourceColumnDefinition(column)) {
            return column.datasource.getTitleByValue(value);
        }
        return value;
    }

    getPropertyValue(item: T, column: ColumnDefinition): any {
        return item[column.id];
    }

    isNew(item: T): boolean {
        return !item.id;
    }

}