import { Component, OnInit, ViewChild } from '@angular/core';
import { CoursesService } from '../../services/courses.service';
import { ObservableDataSource } from '../../datasources/observable.datasource';
import { Year } from '../../models/year.model';
import { MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-academic-years',
  templateUrl: './academic-years.component.html',
  styleUrls: ['./academic-years.component.scss']
})
export class AcademicYearsComponent implements OnInit {

  constructor(private courseService: CoursesService) { }

  displayedColumns: string[] = ['name', 'actions'];
  dataSource: MatTableDataSource<Year>;

  data: BehaviorSubject<Year[]>;
  hasData: boolean;

  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.data = new BehaviorSubject([]);
    this.hasData = true;

    var apiSub = this.courseService.getYears().subscribe(response => {
      this.data.next(response);
      this.hasData = response.length > 0;

      apiSub.unsubscribe();
    });

    this.dataSource = new ObservableDataSource<Year>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  addYear() {
    var newData = this.data.value.slice();
    newData.push({ id: '', name: '' });
    this.data.next(newData);
  }

  removeYear(element: Year) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
  }

  saveYear(element: Year) {
    this.courseService.saveYear(element)
      .then(year => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = year;
        this.data.next(newData);
      });
  }

  deleteYear(element: Year) {
    this.courseService.deleteYear(element)
      .then(year => {
        this.removeYear(element);
      });
  }

}
