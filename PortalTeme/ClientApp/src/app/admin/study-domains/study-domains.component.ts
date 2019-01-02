import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { StudyDomain } from '../../models/study-domain.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ObservableDataSource } from '../../datasources/observable.datasource';

@Component({
  selector: 'app-study-domains',
  templateUrl: './study-domains.component.html',
  styleUrls: ['./study-domains.component.scss']
})
export class StudyDomainsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  displayedColumns: string[] = ['name', 'actions'];
  dataSource: MatTableDataSource<StudyDomain>;

  data: BehaviorSubject<StudyDomain[]>;
  hasData: boolean;

  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.data = new BehaviorSubject([]);
    this.hasData = true;

    var apiSub = this.modelSvcFactory.studyDomains.getAll().subscribe(response => {
      this.data.next(response);
      this.hasData = response.length > 0;

      apiSub.unsubscribe();
    });

    this.dataSource = new ObservableDataSource<StudyDomain>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  add() {
    var newData = this.data.value.slice();
    newData.push({ id: '', name: '' });
    this.data.next(newData);
    this.hasData = true;
  }

  remove(element: StudyDomain) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
    this.hasData = newData.length > 0;
  }

  save(element: StudyDomain) {
    this.modelSvcFactory.studyDomains.save(element)
      .then(sDomain => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = sDomain;
        this.data.next(newData);
        this.hasData = newData.length > 0;
      });
  }

  delete(element: StudyDomain) {
    this.modelSvcFactory.studyDomains.delete(element)
      .then(sdomain => {
        this.remove(element);
      });
  }

}
