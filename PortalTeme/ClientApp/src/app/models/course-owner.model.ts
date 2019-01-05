import { BaseModel } from './base.model';

export interface CourseOwnerDefinition extends BaseModel {
    year: string;
    owner: string;
    name: string;
    numOfAssistants: number;
}

