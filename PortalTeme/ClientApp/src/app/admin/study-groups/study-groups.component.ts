import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';

import { StudyGroup } from '../../models/study-group.model';
import { ModelServiceFactory } from '../../services/model.service';
import { StudyDomain } from '../../models/study-domain.model';
import { Year } from '../../models/year.model';
import { ColumnDefinition, ColumnType, DatasourceColumnDefinition, NamedModelItemAccessor } from '../../models/column-definition.model';
import { NamedModelItemDatasource } from '../../datasources/named-model.item-datasource';
import { flatMap, take, map } from 'rxjs/operators';

@Component({
  selector: 'app-study-groups',
  templateUrl: './study-groups.component.html',
  styleUrls: ['./study-groups.component.scss']
})
export class StudyGroupsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: ColumnDefinition[] = [];
  data: BehaviorSubject<StudyGroup[]>;
  itemAccessor: NamedModelItemAccessor<StudyGroup>;

  years: BehaviorSubject<Year[]>;
  domains: BehaviorSubject<StudyDomain[]>;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.years = new BehaviorSubject([]);
    this.domains = new BehaviorSubject([]);

    this.itemAccessor = new NamedModelItemAccessor<StudyGroup>();

    this.columnDefs = [{
      id: 'name',
      title: 'Name',
      type: ColumnType.Textbox
    }, <DatasourceColumnDefinition<Year>>{
      id: 'year',
      title: 'Year',
      type: ColumnType.Select,
      datasource: new NamedModelItemDatasource<Year>(this.years, 'year')
    }, <DatasourceColumnDefinition<StudyDomain>>{
      id: 'domain',
      title: 'Domain',
      type: ColumnType.Select,
      datasource: new NamedModelItemDatasource<StudyDomain>(this.domains, 'domain')
    }];

    this.getData();
  }

  private getData() {
    let years$ = this.modelSvcFactory.years.getAll();
    let domains$ = this.modelSvcFactory.studyDomains.getAll();
    let groups$ = this.modelSvcFactory.studyGroups.getAll();

    forkJoin(
      years$.pipe(take(1)),
      domains$.pipe(take(1)),
      groups$.pipe(take(1))
    ).subscribe(results => {
      this.years.next(results[0]);
      this.domains.next(results[1]);
      this.data.next(results[2]);
    });
  }

  save(element: StudyGroup): Promise<StudyGroup> {
    return this.modelSvcFactory.studyGroups.save(element);
  }

  delete(element: StudyGroup): Promise<StudyGroup> {
    return this.modelSvcFactory.studyGroups.delete(element);
  }

}
