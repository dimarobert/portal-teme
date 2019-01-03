import { Observable } from 'rxjs';

export interface ItemDatasource<T> {

    getItems(): Observable<T[]>;
    getTitle(item: T): string;
    getTitleByValue(value: any): string;
    getValue(item: T): any;
}