import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { StudyDomain } from '../../models/study-domain.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ColumnDefinition, ColumnType, NamedModelItemAccessor } from '../../models/column-definition.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-study-domains',
  templateUrl: './study-domains.component.html',
  styleUrls: ['./study-domains.component.scss']
})
export class StudyDomainsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: ColumnDefinition[] = [];
  data: BehaviorSubject<StudyDomain[]>;
  itemAccessor: NamedModelItemAccessor<StudyDomain>;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.itemAccessor = new NamedModelItemAccessor<StudyDomain>();

    this.columnDefs = [
      {
        id: 'name',
        title: 'Name',
        type: ColumnType.Textbox
      }
    ];

    this.modelSvcFactory.studyDomains.getAll()
      .pipe(take(1))
      .subscribe(response => {
        this.data.next(response);
      });

  }

  save(element: StudyDomain): Promise<StudyDomain> {
    return this.modelSvcFactory.studyDomains.save(element);
  }

  delete(element: StudyDomain): Promise<StudyDomain> {
    return this.modelSvcFactory.studyDomains.delete(element);
  }

}
