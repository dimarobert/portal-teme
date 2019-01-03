import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, AbstractControl } from '@angular/forms';
import { MatTableDataSource, MatSort } from '@angular/material';
import { BehaviorSubject } from 'rxjs';

import { StudyGroup } from '../../models/study-group.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ObservableDataSource } from '../../datasources/observable.datasource';
import { StudyDomain } from '../../models/study-domain.model';
import { Year } from '../../models/year.model';
import { isHttpErrorResponse } from '../../type-guards/errors.type-guard';

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

  errors: { [key: string]: string[] };

  @ViewChild(MatSort) sort: MatSort;

  addForms: Map<object, FormGroup> = new Map<object, FormGroup>();

  ngOnInit() {
    this.errors = {};
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

  hasAnyError(): boolean {
    return Object.keys(this.errors).length > 0;
  }

  hasError(element: object, field: string): boolean {
    return this.getFormControl(element, field).invalid;
  }

  getError(element: object, field: string): string {
    let control = this.getFormControl(element, field);
    if (control.valid)
      return '';

    if (control.hasError('server'))
      return control.getError('server');

    if (control.hasError('required'))
      return `The ${field} field is required.`;

    return '';
  }

  getForm(row: object): AbstractControl {
    return this.addForms.get(row);
  }

  getFormControl(row: object, field: string): AbstractControl {
    return this.getForm(row).get(field);
  }

  add() {
    const newRow = {};
    this.addForms.set(newRow, new FormGroup({
      name: new FormControl(''),
      domain: new FormControl(''),
      year: new FormControl('')
    }));

    var newData = this.data.value.slice();
    newData.push(<any>newRow);
    this.data.next(newData);
    this.hasData = true;

  }

  remove(element: any) {
    var newData = this.data.value.slice();
    var index = newData.indexOf(element);
    newData.splice(index, 1);
    this.data.next(newData);
    this.hasData = newData.length > 0;

    this.addForms.delete(element);
  }

  save(element: StudyGroup) {
    this.errors = {};
    let form = this.getForm(element);
    const value: StudyGroup = {
      id: '',
      name: form.get('name').value,
      year: form.get('year').value,
      domain: form.get('domain').value,
    };

    this.modelSvcFactory.studyGroups.save(value)
      .then(sGroup => {
        var newData = this.data.value.slice();
        var index = newData.indexOf(element);
        newData[index] = sGroup;
        this.data.next(newData);
        this.hasData = newData.length > 0;

        this.addForms.delete(element);
      })
      .catch(error => {
        if (isHttpErrorResponse(error)) {
          this.errors = error.error;
          for (var err in this.errors) {
            const control = form.get(err);
            if (!control)
              continue;

            control.setErrors({
              server: 'Server validation failed' //this.errors[err][0]
            });
            control.markAsTouched();
          }
        }
      });
  }

  delete(element: StudyGroup) {
    this.modelSvcFactory.studyGroups.delete(element)
      .then(sGroup => {
        this.remove(element);
      });
  }

}
