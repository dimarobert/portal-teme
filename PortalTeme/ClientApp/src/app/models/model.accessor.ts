import { BaseModel } from './base.model';
import { StudyGroupRef } from './course.model';

export interface ModelAccessor {

    create(element: any): any;
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

    create(item: BaseModel): BaseModel {
        return {
            id: item.id || ''
        };
    }
}

export class CourseGroupModelAccessor implements ModelAccessor {
    isNew(item: StudyGroupRef): boolean {
        return !item.courseId && !item.courseId;
    }

    create(item: StudyGroupRef): StudyGroupRef {
        return {
            courseId: item.courseId || '',
            groupId: item.groupId || '',
            name: null
        };
    }
}