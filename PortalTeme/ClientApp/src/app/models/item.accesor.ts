import { ColumnDefinition } from './column-definition.model';
import { NamedModel } from './base.model';
import { isDatasourceColumnDefinition } from '../type-guards/column-definitions.type-guards';
export interface ItemAccessor {
    getPropertyText(item: any, column: ColumnDefinition): string;
    getPropertyValue(item: any, column: ColumnDefinition): any;
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

    protected getProperty(item: any, propertyName: string) {
        if (!Object.keys(item).find(key => key === propertyName))
            throw new Error(`The item does not contain the ${propertyName} property. (item.id=${item.id})`);

        return item[propertyName];
    }
}

export class RelatedItemAccessor<TRelated> extends NamedModelItemAccessor {
    constructor(private getText: (item: TRelated) => string, private getValue?: (item: TRelated) => any) {
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
}