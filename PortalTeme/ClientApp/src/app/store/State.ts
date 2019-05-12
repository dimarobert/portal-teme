import { Year } from '../models/year.model';

export class AppState {
    academicYears: ModelState<Year>;
}

export class ModelState<TModel>{
    items: TModel[];
    loading: boolean;
}