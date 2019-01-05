import { BaseModel } from './base.model';

export interface ModelAccessor {
    /**
     * This returns true if the item is newly added and the inputs must be shown.
     * @param item
     */
    isNew(item: any): boolean;
}

export class BaseModelAccessor implements ModelAccessor {
    isNew(item: BaseModel): boolean {
        return !item.id;
    }
}