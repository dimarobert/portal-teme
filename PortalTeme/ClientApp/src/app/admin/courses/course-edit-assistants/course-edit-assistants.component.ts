import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, forkJoin } from 'rxjs';

import { nameof } from '../../../type-guards/nameof.guard';
import { ModelServiceFactory } from '../../../services/model.service';
import { CustomItemAccessor } from '../../../models/item.accesor';
import { BaseModelAccessor } from '../../../models/model.accessor';
import { DataTableColumns, DatasourceColumnDefinition, ColumnType } from '../../../models/column-definition.model';

import { CourseAssistant, User, Course } from '../../../models/course.model';
import { CustomModelItemDatasource } from '../../../datasources/named-model.item-datasource';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-course-edit-assistants',
  templateUrl: './course-edit-assistants.component.html',
  styleUrls: ['./course-edit-assistants.component.scss']
})
export class CourseEditAssistantsComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory, private route: ActivatedRoute) { }

  columnDefs: DataTableColumns;

  data: BehaviorSubject<User[]>;
  modelAccessor: BaseModelAccessor;
  assistants: BehaviorSubject<User[]>;

  private courseId: string;
  private currentCourse: Course;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.courseId = this.route.parent.snapshot.paramMap.get('id');

    this.assistants = new BehaviorSubject([]);

    this.data = new BehaviorSubject([]);
    this.modelAccessor = new BaseModelAccessor();

    this.columnDefs = new DataTableColumns([
      <DatasourceColumnDefinition<User>>{
        id: nameof<User>("id"),
        title: 'Assistants',
        type: ColumnType.Select,
        datasource: new CustomModelItemDatasource<User>({
          getItems: () => this.assistants,
          getTitle: item => `${item.firstName} ${item.lastName}`,
          getValue: item => item.id,
          findByValue: (item, value) => item.id == value,
          modelName: 'assistant'
        }),
        itemAccessor: new CustomItemAccessor<User>(null, item => item.id, (item, value) => {
          item.id = value;
        })
      }
    ]);

    this.getData();
  }

  getData() {
    const assistants$ = this.modelSvcFactory.users.getAssistants();
    const currentCourse$ = this.modelSvcFactory.courses.get(this.courseId);

    forkJoin(
      assistants$.pipe(take(1)),
      currentCourse$.pipe(take(1))
    ).subscribe(results => {
      this.assistants.next(results[0]);
      this.currentCourse = results[1];

      this.data.next(this.currentCourse.assistants);
    });
  }

  save(element: User): Promise<User> {
    const value = <CourseAssistant>{
      courseId: this.courseId,
      assistant: element
    };
    return this.modelSvcFactory.courseRelations.addAssistant(value)
      .then(ca => ca.assistant);
  }

  delete(element: User): Promise<User> {
    const value = <CourseAssistant>{
      courseId: this.courseId,
      assistant: element
    };
    return this.modelSvcFactory.courseRelations.deleteAssistant(value)
      .then(ca => ca.assistant);
  }

}
