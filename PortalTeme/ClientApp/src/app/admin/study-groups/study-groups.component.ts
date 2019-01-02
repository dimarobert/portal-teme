import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { StudyGroup } from '../../models/study-group.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ObservableDataSource } from '../../datasources/observable.datasource';
import { StudyDomain } from '../../models/study-domain.model';
import { Year } from '../../models/year.model';

@Component({
  selector: 'app-study-groups',
  templateUrl: './study-groups.component.html',
  styleUrls: ['./study-groups.component.scss']
})
export class StudyGroupsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  displayedColumns: string[] = ['name', 'domain', 'year', 'actions'];
  dataSource: MatTableDataSource<StudyGroup>;

  data: BehaviorSubject<StudyGroup[]>;
  hasData: boolean;

  years: BehaviorSubject<Year[]>;
  domains: BehaviorSubject<StudyDomain[]>;


  @ViewChild(MatSort) sort: MatSort;

  ngOnInit() {
    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);
    this.domains = new BehaviorSubject([]);
    this.hasData = true;

    var apiSub = this.modelSvcFactory.studyGroups.getAll().subscribe(response => {
      this.data.next(response);
      this.hasData = response.length > 0;

      apiSub.unsubscribe();
    });

    var apiSubY = this.modelSvcFactory.years.getAll().subscribe(yearsRes => {
      this.years.next(yearsRes);

      apiSubY.unsubscribe();
    });
    var apiSubD = this.modelSvcFactory.studyDomains.getAll().subscribe(domainsRes => {
      this.domains.next(domainsRes);

      apiSubD.unsubscribe();
    });

    this.dataSource = new ObservableDataSource<StudyGroup>(this.data);
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'name', disableClear: false, start: 'asc' });
  }

  getYearName(yearId: string): string {
    const year = this.years.value.find(y => y.id == yearId);
    if (year == null)
      return '<invalid year>';
    return year.name;
  }

  getDomainName(domainId: string): string {
    const domain = this.domains.value.find(y => y.id == domainId);
    if (domain == null)
      return '<invalid domain>';
    return domain.name;
  }

  add() {
    var newData = this.data.value.slice();
    newData.push({ id: '', name: '', domain: '', year: '' });
    this.data.next(newData);
    this.hasData = true;
  }

  remove(element: StudyGroup) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
    this.hasData = newData.length > 0;
  }

  save(element: StudyGroup) {
    this.modelSvcFactory.studyGroups.save(element)
      .then(sGroup => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = sGroup;
        this.data.next(newData);
        this.hasData = newData.length > 0;
      });
  }

  delete(element: StudyGroup) {
    this.modelSvcFactory.studyGroups.delete(element)
      .then(sGroup => {
        this.remove(element);
      });
  }

}
