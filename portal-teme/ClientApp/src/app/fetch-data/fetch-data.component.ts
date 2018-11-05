import { Component, Inject, ViewChild, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSort, MatTableDataSource } from '@angular/material';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {

  displayedColumns: string[] = ['dateFormatted', 'temperatureC', 'temperatureF', 'summary'];
  dataSource: MatTableDataSource<WeatherForecast>;

  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.dataSource.sort = this.sort;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.dataSource = new WeatherForecastDataSource(http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts'));
  }

}

export interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

export class WeatherForecastDataSource extends MatTableDataSource<WeatherForecast>{
  dataSubscription: Subscription;

  constructor(private dataObservable: Observable<WeatherForecast[]>) {
    super();
  }

  connect(): BehaviorSubject<WeatherForecast[]> {
    this.dataSubscription = this.dataObservable.subscribe({
      next: (v) => {
        this.data = v;
      }
    });

    return super.connect();
  }

  disconnect(): void {
    this.dataSubscription.unsubscribe();
  }

}
