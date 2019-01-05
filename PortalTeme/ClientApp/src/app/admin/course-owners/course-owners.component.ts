import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';

import { DatasourceColumnDefinition, ColumnType, ColumnDefinition, NamedModelItemAccessor } from '../../models/column-definition.model';
import { ModelServiceFactory } from '../../services/model.service';
import { Year } from '../../models/year.model';
import { NamedModelItemDatasource } from '../../datasources/named-model.item-datasource';
import { Course } from '../../models/course.model';

@Component({
  selector: 'app-course-owners-definitions',
  templateUrl: './course-owners.component.html',
  styleUrls: ['./course-owners.component.scss']
})
export class CourseOwnersDefinitionsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: ColumnDefinition[];
  data: BehaviorSubject<Course[]>;
  itemAccessor: NamedModelItemAccessor<Year>;

  years: BehaviorSubject<Year[]>;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);

    this.itemAccessor = new NamedModelItemAccessor<Year>();

    this.columnDefs = [{
      id: 'name',
      title: 'Course',
      type: ColumnType.Textbox
    },
    {
      id: 'owner',
      title: 'Professor',
      type: ColumnType.Textbox
    },
    {
      id: 'numOfAssistants',
      title: '# Assistants',
      type: ColumnType.Textbox
    }
    , <DatasourceColumnDefinition<Year>>{
      id: 'year',
      title: 'Year',
      type: ColumnType.Select,
      datasource: new NamedModelItemDatasource<Year>(this.years, 'year')
    }];

    this.getData();
  }

  private getData() {
    const years$ = this.modelSvcFactory.years.getAll();
    const courses$ = this.modelSvcFactory.coursesOwners.getAll();

    forkJoin(
      years$.pipe(take(1)),
      courses$.pipe(take(1))
    ).subscribe(results => {
      this.years.next(results[0]);
      this.data.next(results[1]);
    });
  }

  save(element: Course): Promise<Course> {
    return this.modelSvcFactory.coursesOwners.save(element);
  }

  delete(element: Course): Promise<Course> {
    return this.modelSvcFactory.coursesOwners.delete(element);
  }

}
