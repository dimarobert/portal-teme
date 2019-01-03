import { NamedModel } from '../models/base.model';
import { ItemDatasource } from './item-datasource';
import { Observable, BehaviorSubject } from 'rxjs';

export class NamedModelItemDatasource<T extends NamedModel> implements ItemDatasource<T> {

    constructor(private data: BehaviorSubject<T[]>, private modelName: string) { }

    getItems(): Observable<T[]> {
        return this.data;
    }

    getTitle(item: T): string {
        return item.name;
    }

    getValue(item: T) {
        return item.id;
    }

    getTitleByValue(value: any): string {
        const item = this.data.value.find(item => item.id == value);
        if (!item)
            return `<invalid ${this.modelName}>`;
        throw new Error(`Element with id '${value}' was not found in the current items.`);

        return this.getTitle(item);
    }

}