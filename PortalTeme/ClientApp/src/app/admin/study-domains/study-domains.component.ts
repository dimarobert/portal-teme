import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { nameof } from '../../type-guards/nameof.guard';

import { StudyDomain } from '../../models/study-domain.model';
import { ModelServiceFactory } from '../../services/model.service';
import { ColumnType, DataTableColumns, EditableColumnDefinition } from '../../models/column-definition.model';
import { BaseModelAccessor, ModelAccessor } from '../../models/model.accessor';

@Component({
  selector: 'app-study-domains',
  templateUrl: './study-domains.component.html',
  styleUrls: ['./study-domains.component.scss']
})
export class StudyDomainsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: DataTableColumns;
  data: BehaviorSubject<StudyDomain[]>;
  modelAccessor: ModelAccessor;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);
    this.modelAccessor = new BaseModelAccessor();

    this.columnDefs = new DataTableColumns([
      <EditableColumnDefinition>{
        id: nameof<StudyDomain>('name'),
        title: 'Name',
        type: ColumnType.Textbox
      }
    ]);

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
    return this.modelSvcFactory.studyDomains.delete(element.id);
  }

}
