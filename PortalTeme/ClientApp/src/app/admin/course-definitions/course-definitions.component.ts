import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';

import { nameof } from '../../type-guards/nameof.guard';

import { Year } from '../../models/year.model';
import { CourseDefinition, Semester } from '../../models/course-definition.model';
import { ModelServiceFactory } from '../../services/model.service';
import { DatasourceColumnDefinition, ColumnType, EditableColumnDefinition, DataTableColumns } from '../../models/column-definition.model';
import { NamedModelItemDatasource } from '../../datasources/named-model.item-datasource';
import { BaseModelAccessor, ModelAccessor } from '../../models/model.accessor';
import { MenuService } from '../../services/menu.service';

@Component({
  selector: 'app-course-definitions',
  templateUrl: './course-definitions.component.html',
  styleUrls: ['./course-definitions.component.scss']
})
export class CourseDefinitionsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory, private menuService: MenuService) { }

  columnDefs: DataTableColumns;
  data: BehaviorSubject<CourseDefinition[]>;
  modelAccessor: ModelAccessor;

  years: BehaviorSubject<Year[]>;

  Semester = Semester;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.update = this.update.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);

    this.modelAccessor = new BaseModelAccessor();

    this.columnDefs = new DataTableColumns([
      <EditableColumnDefinition>{
        id: nameof<CourseDefinition>('name'),
        title: 'Name',
        type: ColumnType.Textbox
      },
      {
        id: nameof<CourseDefinition>('acronym'),
        title: 'Acronym',
        type: ColumnType.Textbox
      }, <DatasourceColumnDefinition<Year>>{
        id: nameof<CourseDefinition>('year'),
        title: 'Year',
        type: ColumnType.Select,
        datasource: new NamedModelItemDatasource<Year>(this.years, 'year')
      }]);

    this.getData();
  }

  private getData() {
    let years$ = this.modelSvcFactory.years.getAll();
    let courses$ = this.modelSvcFactory.courseDefinitions.getAll();

    forkJoin(
      years$.pipe(take(1)),
      courses$.pipe(take(1))
    ).subscribe(results => {
      this.years.next(results[0]);
      this.data.next(results[1]);
    });
  }

  save(element: CourseDefinition): Promise<CourseDefinition> {
    return this.modelSvcFactory.courseDefinitions.save(element).then((cd) => {
      this.menuService.refreshCourses();
      return cd;
    });
  }

  update(element: CourseDefinition): Promise<CourseDefinition> {
    return this.modelSvcFactory.courseDefinitions.update(element).then((cd) => {
      this.menuService.refreshCourses();
      return cd;
    });
  }

  delete(element: CourseDefinition): Promise<CourseDefinition> {
    return this.modelSvcFactory.courseDefinitions.delete(element.id).then((cd) => {
      this.menuService.refreshCourses();
      return cd;
    });
  }

}
