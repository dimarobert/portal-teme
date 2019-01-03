import { Component, OnInit } from '@angular/core';
import { ModelServiceFactory } from '../../services/model.service';
import { Year } from '../../models/year.model';
import { BehaviorSubject } from 'rxjs';
import { ColumnType, ColumnDefinition, NamedModelItemAccessor } from '../../models/column-definition.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-academic-years',
  templateUrl: './academic-years.component.html',
  styleUrls: ['./academic-years.component.scss']
})
export class AcademicYearsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: ColumnDefinition[];
  itemAccessor: NamedModelItemAccessor<Year>;
  data: BehaviorSubject<Year[]>;


  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.data = new BehaviorSubject([]);

    this.itemAccessor = new NamedModelItemAccessor<Year>();

    this.columnDefs = [
      {
        id: 'name',
        title: 'Name',
        type: ColumnType.Textbox
      }
    ];

    this.modelSvcFactory.years.getAll()
      .pipe(take(1))
      .subscribe(response => {
        this.data.next(response);
      });
  }

  save(element: Year): Promise<Year> {
    return this.modelSvcFactory.years.save(element);
  }

  delete(element: Year): Promise<Year> {
    return this.modelSvcFactory.years.delete(element);
  }

}
