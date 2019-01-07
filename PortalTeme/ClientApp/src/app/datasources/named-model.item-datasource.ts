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

        return this.getTitle(item);
    }

}

export class CustomModelItemDatasource<T> implements ItemDatasource<T> {

    constructor(private options: CustomModelItemDatasourceOptions<T>) { }

    getItems(): Observable<T[]> {
        return this.options.getItems();
    }

    getTitle(item: T): string {
        return this.options.getTitle(item);
    }

    getValue(item: T) {
        return this.options.getValue(item);
    }

    getTitleByValue(value: any): string {
        const item = this.options.getItems().value.find(item => this.options.findByValue(item, value));
        if (!item)
            return `<invalid ${this.options.modelName}>`;

        return this.getTitle(item);
    }

}

export interface CustomModelItemDatasourceOptions<T> {
    modelName: string;
    getItems: () => BehaviorSubject<T[]>;
    getTitle: (item: T) => string;
    getValue: (item: T) => any;
    findByValue: (item: T, value: any) => boolean;
}