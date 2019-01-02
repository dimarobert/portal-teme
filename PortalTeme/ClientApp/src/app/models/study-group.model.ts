import { BaseModel } from './base.model';

export interface StudyGroup extends BaseModel {

    name: string;
    domain: string;
    year: string;
}