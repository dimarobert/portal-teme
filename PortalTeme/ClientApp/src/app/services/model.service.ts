import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { take, map } from 'rxjs/operators';

import { BaseModel, EditModel } from '../models/base.model';
import { Year } from '../models/year.model';
import { StudyDomain } from '../models/study-domain.model';
import { StudyGroup } from '../models/study-group.model';
import { CourseDefinition } from '../models/course-definition.model';
import { Course, CourseEdit, User, CourseGroup, CourseAssistant, CourseStudent, CourseRelation } from '../models/course.model';
import { Assignment, AssignmentEdit } from '../models/assignment.model';

@Injectable({
  providedIn: 'root'
})
export class ModelServiceFactory {

  constructor(private http: HttpClient) { }

  private _yearsService: ModelService<Year> = null;
  public get years(): ModelService<Year> {
    return this._yearsService || (this._yearsService = new ModelService<Year>('AcademicYears', this.http));
  }

  private _courseDefinitionsService: ModelService<CourseDefinition> = null;
  public get courseDefinitions(): ModelService<CourseDefinition> {
    return this._courseDefinitionsService || (this._courseDefinitionsService = new ModelService<CourseDefinition>('CourseDefinitions', this.http));
  }

  private _studyDomainsService: ModelService<StudyDomain> = null;
  public get studyDomains(): ModelService<StudyDomain> {
    return this._studyDomainsService || (this._studyDomainsService = new ModelService<StudyDomain>('StudyDomains', this.http));
  }

  private _studyGroupsService: ModelService<StudyGroup> = null;
  public get studyGroups(): ModelService<StudyGroup> {
    return this._studyGroupsService || (this._studyGroupsService = new ModelService<StudyGroup>('Groups', this.http));
  }


  private _coursesService: ModelWithSlugService<Course, CourseEdit> = null;
  public get courses(): ModelWithSlugService<Course, CourseEdit> {
    return this._coursesService || (this._coursesService = new ModelWithSlugService<Course, CourseEdit>('Courses', this.http));
  }

  private _courseRelationsService: CourseRelationsService = null;
  public get courseRelations(): CourseRelationsService {
    return this._courseRelationsService || (this._courseRelationsService = new CourseRelationsService(this.http));
  }

  private _usersService: UsersService = null;
  public get users(): UsersService {
    return this._usersService || (this._usersService = new UsersService(this.http));
  }

  private _assignmentsService: AssignmentsService = null;
  public get assignments(): AssignmentsService {
    return this._assignmentsService || (this._assignmentsService = new AssignmentsService('Assignments', this.http));
  }
}

export class ModelServiceBase<TModel extends BaseModel> {

  constructor(private apiController: string, protected http: HttpClient) { }

  protected get apiRoot(): string {
    return `/api/${this.apiController}`;
  }

  public getAll(): Observable<TModel[]> {
    return this.http.get<TModel[]>(this.apiRoot)
  }

  public get(modelId: string): Observable<TModel> {
    return this.http.get<TModel>(`${this.apiRoot}/${modelId}`);
  }

  public delete(modelId: string): Promise<TModel> {
    return this.http.delete<TModel>(`${this.apiRoot}/${modelId}`)
      .pipe(take(1))
      .toPromise();
  }
}

export class ModelService<TModel extends BaseModel> extends ModelServiceBase<TModel> {
  public save(model: TModel): Promise<TModel> {
    return this.http.post<TModel>(this.apiRoot, model)
      .pipe(take(1))
      .toPromise();
  }

  public update(model: TModel): Promise<TModel> {
    return this.http.put<TModel>(`${this.apiRoot}/${model.id}`, model)
      .pipe(
        take(1),
        map(nothing => model)
      ).toPromise();
  }
}

export class ComplexModelService<TViewModel extends BaseModel, TEditModel extends EditModel> extends ModelServiceBase<TViewModel>  {

  public getAllRef(): Observable<TEditModel[]> {
    return this.http.get<TEditModel[]>(`${this.apiRoot}/Ref`);
  }

  public save(model: TEditModel): Promise<TEditModel> {
    return this.http.post<TEditModel>(this.apiRoot, model)
      .pipe(take(1))
      .toPromise();
  }

  public update(model: TEditModel): Promise<TEditModel> {
    return this.http.put<TEditModel>(`${this.apiRoot}/${model.id}`, model)
      .pipe(
        take(1),
        map(nothing => model)
      )
      .toPromise();
  }
}

export class ModelWithSlugService<TViewModel extends BaseModel, TEditModel extends EditModel> extends ComplexModelService<TViewModel, TEditModel>{

  public getBySlug(slug: string): Observable<TViewModel> {
    return this.http.get<TViewModel>(`${this.apiRoot}/slug/${slug}`);
  }

}

export class AssignmentsService extends ModelWithSlugService<Assignment, AssignmentEdit> {

  public getAll(): Observable<Assignment[]> {
    throw new Error('Invalid operation. GetAll is not supported for assignments. Use the getByCourse(courseId) method instead.');
  }

  public getByCourse(courseId: string): Observable<Assignment[]> {
    return this.http.get<Assignment[]>(`${this.apiRoot}/ForCourse/${courseId}`);
  }

}

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  protected get apiRoot(): string {
    return `/api/Users`;
  }

  public getProfessors(): Observable<User[]> {
    return this.getUsersByRole('Professors');
  }

  public getAssistants(): Observable<User[]> {
    return this.getUsersByRole('Assistants');
  }

  public getStudents(): Observable<User[]> {
    return this.getUsersByRole('Students');
  }

  private getUsersByRole(role: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiRoot}/${role}`);

  }
}

@Injectable({
  providedIn: 'root'
})
export class CourseRelationsService {

  constructor(private http: HttpClient) { }

  protected get apiRoot(): string {
    return `/api/Courses`;
  }

  public addGroup(courseGroup: CourseGroup): Promise<CourseGroup> {
    return this.addModel('AddGroup', courseGroup);
  }

  public deleteGroup(courseGroup: CourseGroup): Promise<CourseGroup> {
    return this.deleteModel('DeleteGroup', courseGroup.courseId, courseGroup.groupId);
  }

  public addAssistant(courseAssistant: CourseAssistant): Promise<CourseAssistant> {
    return this.addModel('AddAssistant', courseAssistant);
  }

  public deleteAssistant(courseAssistant: CourseAssistant): Promise<CourseAssistant> {
    return this.deleteModel('DeleteAssistant', courseAssistant.courseId, courseAssistant.assistant.id);
  }

  public addStudent(courseStudent: CourseStudent): Promise<CourseStudent> {
    return this.addModel('AddStudent', courseStudent);
  }

  public deleteStudent(courseStudent: CourseStudent): Promise<CourseStudent> {
    return this.deleteModel('DeleteStudent', courseStudent.courseId, courseStudent.student.id);
  }

  private addModel<T extends CourseRelation>(modelEndpoint: string, model: T): Promise<T> {
    return this.http.post<T>(`${this.apiRoot}/${model.courseId}/${modelEndpoint}`, model)
      .pipe(take(1))
      .toPromise();
  }

  private deleteModel<T extends CourseRelation>(modelEndpoint: string, courseId: string, modelId: string): Promise<T> {
    return this.http.delete<T>(`${this.apiRoot}/${courseId}/${modelEndpoint}/${modelId}`)
      .pipe(take(1))
      .toPromise();
  }
}

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

  public delete(modelId: string): Promise<TModel> {
    return super.delete(modelId)
      .then(resultModel => {
        this.deleteFromCache(modelId);
        return resultModel;
      });
  }


  private deleteFromCache(modelId: string) {
    let newValue = this.modelCache.value.slice();
    var idx = newValue.findIndex(m => m.id == modelId);
    if (idx > -1) {
      newValue.splice(idx, 1);
      this.modelCache.next(newValue);
    }

    let oneSub = this.modelOneCache[modelId];
    if (oneSub != null) {
      oneSub.complete();
      delete this.modelOneCache[modelId];
    }
  }
}
