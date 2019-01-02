import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject, Observable } from 'rxjs';

import { ObservableDataSource } from '../../datasources/observable.datasource';
import { CourseDefinition, Semester } from '../../models/course-definition.model';
import { ModelServiceFactory } from '../../services/model.service';
import { Year } from '../../models/year.model';

@Component({
  selector: 'app-course-definitions',
  templateUrl: './course-definitions.component.html',
  styleUrls: ['./course-definitions.component.scss']
})
export class CourseDefinitionsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  displayedColumns: string[] = ['name', 'year', 'semester', 'actions'];
  dataSource: MatTableDataSource<CourseDefinition>;

  data: BehaviorSubject<CourseDefinition[]>;
  hasData: boolean;

  years: BehaviorSubject<Year[]>;

  Semester = Semester;

  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);
    this.hasData = true;

    var apiSub = this.modelSvcFactory.courses.getAll().subscribe(response => {
      this.data.next(response);
      this.hasData = response.length > 0;

      apiSub.unsubscribe();
    });

    var apiSub2 = this.modelSvcFactory.years.getAll().subscribe(yearsRes => {
      this.years.next(yearsRes);

      apiSub2.unsubscribe();
    });

    this.dataSource = new ObservableDataSource<CourseDefinition>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  getYearName(yearId: string): string {
    const year = this.years.value.find(y => y.id == yearId);
    if (year == null)
      return '<invalid year>';
    return year.name;
  }

  addCourse() {
    var newData = this.data.value.slice();
    newData.push({ id: '', name: '', semester: Semester.First, year: '' });
    this.data.next(newData);
  }

  removeCourse(element: CourseDefinition) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
  }

  saveCourse(element: CourseDefinition) {
    this.modelSvcFactory.courses.save(element)
      .then(courseDef => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = courseDef;
        this.data.next(newData);
      });
  }

  deleteCourse(element: CourseDefinition) {
    this.modelSvcFactory.courses.delete(element)
      .then(courseDef => {
        this.removeCourse(element);
      });
  }

}
