import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { take } from 'rxjs/operators';

import { BaseModel } from '../models/base.model';
import { Year } from '../models/year.model';
import { StudyDomain } from '../models/study-domain.model';
import { StudyGroup } from '../models/study-group.model';
import { CourseDefinition } from '../models/course-definition.model';
import { Course } from '../models/course.model';

@Injectable({
  providedIn: 'root'
})
export class ModelServiceFactory {

  constructor(private http: HttpClient) { }

  private _yearsService: ModelService<Year> = null;
  public get years(): ModelService<Year> {
    return this._yearsService || (this._yearsService = new ModelService<Year>('AcademicYears', this.http));
  }

  private _coursesService: ModelService<CourseDefinition> = null;
  public get courses(): ModelService<CourseDefinition> {
    return this._coursesService || (this._coursesService = new ModelService<CourseDefinition>('CourseDefinitions', this.http));
  }

  private _studyDomainsService: ModelService<StudyDomain> = null;
  public get studyDomains(): ModelService<StudyDomain> {
    return this._studyDomainsService || (this._studyDomainsService = new ModelService<StudyDomain>('StudyDomains', this.http));
  }

  private _studyGroupsService: ModelService<StudyGroup> = null;
  public get studyGroups(): ModelService<StudyGroup> {
    return this._studyGroupsService || (this._studyGroupsService = new ModelService<StudyGroup>('Groups', this.http));
  }

  public get coursesOwners(): ModelService<Course> {
    return <any>{
      getAll: () => of([
        <Course>{
          id: '1',
          courseDef: {
            id: 'GUID string',
            name: 'Course 0'
          },
          professor: { professorId: 'GUID string', firstName: 'Radu', lastName: 'Niculcea' },
          assistants: [{ assistantId: 'GUID string', firstName: 'Robert', lastName: 'Dima' }],
          groups: [{ groupId: 'GUID string', name: 'Gruap 143' }],
          students: [{ studentId: 'GUID string', firstName: 'Cristian', lastName: 'Popescu' }]
        }
      ])
    };
  }
}

@Injectable({
  providedIn: 'root'
})
export class ModelService<TModel extends BaseModel> {

  constructor(private apiController: string, private http: HttpClient) { }

  private get apiRoot(): string {
    return `/api/${this.apiController}`;
  }

  public getAll(): Observable<TModel[]> {
    return this.http.get<TModel[]>(this.apiRoot)
  }

  public get(modelId: string): Observable<TModel> {
    return this.http.get<TModel>(`${this.apiRoot}/${modelId}`);
  }

  public save(model: TModel): Promise<TModel> {
    return this.http.post<TModel>(this.apiRoot, model)
      .pipe(take(1))
      .toPromise();
  }

  public delete(model: TModel): Promise<TModel> {
    return this.http.delete<TModel>(`${this.apiRoot}/${model.id}`)
      .pipe(take(1))
      .toPromise();
  }
}

@Injectable({
  providedIn: 'root'
})
export class CachedModelService<TModel extends BaseModel> extends ModelService<TModel> {
  // TODO: Support for multiple users editing the data. 
  // This is a must because the app is a SPA and a full refresh would be required to view other users changes.

  private modelCache: BehaviorSubject<TModel[]> = new BehaviorSubject<TModel[]>(null);
  public getAll(): Observable<TModel[]> {

    if (this.modelCache.value == null) {
      super.getAll()
        .pipe(take(1))
        .subscribe(result => {
          this.modelCache.next(result);
        });
    }

    return this.modelCache;
  }

  private modelOneCache: { [id: string]: BehaviorSubject<TModel> }
  public get(modelId: string): Observable<TModel> {

    if (this.modelOneCache[modelId] != null) {
      return this.modelOneCache[modelId];
    }

    const cacheSub = this.modelOneCache[modelId] = new BehaviorSubject<TModel>(null);
    if (this.modelCache.value != null) {
      const fromAllCache = this.modelCache.value.find(model => model.id == modelId);
      if (fromAllCache != null) {
        cacheSub.next(fromAllCache);
        return cacheSub;
      }
    }

    super.get(modelId)
      .pipe(take(1))
      .subscribe(value => {
        cacheSub.next(value);
      });

    return cacheSub;
  }

  public save(model: TModel): Promise<TModel> {
    return super.save(model)
      .then(resultModel => {
        this.updateCache(resultModel);
        return resultModel;
      });
  }

  private updateCache(resultModel: TModel): void {
    let newValue = this.modelCache.value.slice();
    newValue.push(resultModel);
    this.modelCache.next(newValue);

    let oneSub = this.modelOneCache[resultModel.id];
    oneSub && oneSub.next(resultModel);
  }

  public delete(model: TModel): Promise<TModel> {
    return super.delete(model)
      .then(resultModel => {
        this.deleteFromCache(model);
        return resultModel;
      });
  }


  private deleteFromCache(model: TModel) {
    let newValue = this.modelCache.value.slice();
    var idx = newValue.findIndex(m => m.id == model.id);
    if (idx > -1) {
      newValue.splice(idx, 1);
      this.modelCache.next(newValue);
    }

    let oneSub = this.modelOneCache[model.id];
    if (oneSub != null) {
      oneSub.complete();
      delete this.modelOneCache[model.id];
    }
  }
}
