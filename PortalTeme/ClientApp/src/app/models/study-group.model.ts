import { NamedModel } from './base.model';

export interface StudyGroup extends NamedModel {
    domain: string;
    year: string;
}