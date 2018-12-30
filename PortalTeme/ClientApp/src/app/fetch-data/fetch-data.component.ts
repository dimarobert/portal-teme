import { Component, Inject, ViewChild, AfterViewInit, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSort, MatTableDataSource } from '@angular/material';
import { ObservableDataSource } from '../datasources/observable.datasource';


export interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent implements OnInit {

  displayedColumns: string[] = ['dateFormatted', 'temperatureC', 'temperatureF', 'summary'];
  dataSource: MatTableDataSource<WeatherForecast>;

  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.dataSource.sort = this.sort;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.dataSource = new ObservableDataSource(http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts'));
  }

}
