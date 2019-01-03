import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';

import { CourseDefinition, Semester } from '../../models/course-definition.model';
import { ModelServiceFactory } from '../../services/model.service';
import { Year } from '../../models/year.model';
import { DatasourceColumnDefinition, ColumnType, ColumnDefinition, NamedModelItemAccessor } from '../../models/column-definition.model';
import { NamedModelItemDatasource } from '../../datasources/named-model.item-datasource';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-course-definitions',
  templateUrl: './course-definitions.component.html',
  styleUrls: ['./course-definitions.component.scss']
})
export class CourseDefinitionsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: ColumnDefinition[];
  data: BehaviorSubject<CourseDefinition[]>;
  itemAccessor: NamedModelItemAccessor<Year>;

  years: BehaviorSubject<Year[]>;

  Semester = Semester;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);

    this.itemAccessor = new NamedModelItemAccessor<Year>();

    this.columnDefs = [{
      id: 'name',
      title: 'Name',
      type: ColumnType.Textbox
    }, <DatasourceColumnDefinition<Year>>{
      id: 'year',
      title: 'Year',
      type: ColumnType.Select,
      datasource: new NamedModelItemDatasource<Year>(this.years, 'year')
    }];

    this.getData();
  }

  private getData() {
    let years$ = this.modelSvcFactory.years.getAll();
    let courses$ = this.modelSvcFactory.courses.getAll();

    forkJoin(
      years$.pipe(take(1)),
      courses$.pipe(take(1))
    ).subscribe(results => {
      this.years.next(results[0]);
      this.data.next(results[1]);
    });
  }

  save(element: CourseDefinition): Promise<CourseDefinition> {
    return this.modelSvcFactory.courses.save(element);
  }

  delete(element: CourseDefinition): Promise<CourseDefinition> {
    return this.modelSvcFactory.courses.delete(element);
  }

}
