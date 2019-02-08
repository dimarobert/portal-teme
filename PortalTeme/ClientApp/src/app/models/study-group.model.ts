import { NamedModel } from './base.model';

export interface StudyGroup extends NamedModel {
    code: string;
    domain: string;
    year: string;
}