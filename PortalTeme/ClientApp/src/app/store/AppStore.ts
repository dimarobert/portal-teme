import { BehaviorSubject, Observable } from 'rxjs';
import { AppState, ModelState } from './State';
import { Year } from '../models/year.model';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class AppStore {

    private store = new BehaviorSubject<AppState>({
        academicYears: this.initialModelState<Year>()
    });

    private initialModelState<T>(): ModelState<T> {
        return {
            items: null,
            loading: false
        }
    }

    public state$ = this.store as Observable<AppState>;

    public updateModelState<T>(modelName: string, newState: ModelState<T>) {
        this.store.next({ ...this.store.value, [modelName]: newState });
    }
}