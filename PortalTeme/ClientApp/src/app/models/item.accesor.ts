import { ColumnDefinition } from './column-definition.model';
import { NamedModel } from './base.model';
import { isDatasourceColumnDefinition } from '../type-guards/column-definitions.type-guards';
export interface ItemAccessor {
    getPropertyText(item: any, column: ColumnDefinition): string;
    getPropertyValue(item: any, column: ColumnDefinition): any;
    setPropertyToItem(item: any, column: ColumnDefinition, value: any): void;
}


export class NamedModelItemAccessor implements ItemAccessor {

    getPropertyText(item: NamedModel, column: ColumnDefinition): string {
        const value = this.getProperty(item, column.id);
        if (isDatasourceColumnDefinition(column)) {
            return column.datasource.getTitleByValue(value);
        }
        return value;
    }

    getPropertyValue(item: NamedModel, column: ColumnDefinition): any {
        return this.getProperty(item, column.id);
    }

    setPropertyToItem(item: any, column: ColumnDefinition, value: any) {
        item[column.id] = value;
    }

    protected getProperty(item: any, propertyName: string) {
        if (!Object.keys(item).find(key => key === propertyName))
            throw new Error(`The item does not contain the ${propertyName} property. (item.id=${item.id})`);

        return item[propertyName];
    }

}

export class CustomItemAccessor<TModel> extends NamedModelItemAccessor {
    constructor(private getText?: (item: TModel) => string, private getValue?: (item: TModel) => any, private setValue?: (item: TModel, value: any) => void) {
        super();
    }

    getPropertyText(item: any, column: ColumnDefinition): string {
        if (!this.getText)
            return super.getPropertyText(item, column);
        return this.getText(item);
    }

    getPropertyValue(item: any, column: ColumnDefinition) {
        if (!this.getValue && !this.getText)
            return super.getPropertyValue(item, column);
        return (this.getValue || this.getText)(item);
    }

    setPropertyToItem(item: any, column: ColumnDefinition, value: any) {
        if (!this.setValue)
            return super.setPropertyToItem(item, column, value);
        this.setValue(item, value);
    }
}

export class RelatedItemAccessor<TRelated> extends NamedModelItemAccessor {
    constructor(private getText: (item: TRelated) => string, private getValue?: (item: TRelated) => any, private setValue?: (item: TRelated, value: any) => void) {
        super();
    }

    getPropertyText(item: any, column: ColumnDefinition): string {
        const related = this.getProperty(item, column.id);
        return this.getText(related);
    }

    getPropertyValue(item: any, column: ColumnDefinition) {
        const related = this.getProperty(item, column.id);
        return (this.getValue || this.getText)(related);
    }

    setPropertyToItem(item: any, column: ColumnDefinition, value: any) {
        const related = this.getProperty(item, column.id);
        this.setValue(related, value);
    }
}