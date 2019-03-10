import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, forkJoin, Observable, Subscription } from 'rxjs';

import { nameof } from '../../../type-guards/nameof.guard';
import { ModelServiceFactory } from '../../../services/model.service';
import { CustomItemAccessor } from '../../../models/item.accesor';
import { BaseModelAccessor } from '../../../models/model.accessor';
import { DataTableColumns, DatasourceColumnDefinition, ColumnType } from '../../../models/column-definition.model';

import { CourseAssistant, User, Course } from '../../../models/course.model';
import { CustomModelItemDatasource } from '../../../datasources/named-model.item-datasource';
import { take } from 'rxjs/operators';
import { DataTableComponent } from '../../datatable/datatable.component';
import { MenuService } from '../../../services/menu.service';

@Component({
  selector: 'app-course-edit-assistants',
  templateUrl: './course-edit-assistants.component.html',
  styleUrls: ['./course-edit-assistants.component.scss']
})
export class CourseEditAssistantsComponent implements OnInit, OnDestroy {

  constructor(private modelSvcFactory: ModelServiceFactory, private menuService: MenuService) { }

  @Input() courseId: string;

  columnDefs: DataTableColumns;

  data: BehaviorSubject<User[]>;
  modelAccessor: BaseModelAccessor;
  assistants: BehaviorSubject<User[]>;

  @ViewChild('assistantsTable') assistantsTable: DataTableComponent;

  private currentCourse: Course;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

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

  update() {
    this.getData();
  }

  private getData() {
    const assistants$ = this.modelSvcFactory.users.getAssistants();
    const currentCourse$ = this.modelSvcFactory.courses.get(this.courseId);

    this.assistantsTable.loading = true;

    forkJoin(
      assistants$.pipe(take(1)),
      currentCourse$.pipe(take(1))
    ).subscribe(results => {
      this.assistants.next(results[0]);
      this.currentCourse = results[1];

      this.data.next(this.currentCourse.assistants);

      this.assistantsTable.loading = false;
    });
  }

  save(element: User): Promise<User> {
    const value = <CourseAssistant>{
      courseId: this.courseId,
      assistant: element
    };
    return this.modelSvcFactory.courseRelations.addAssistant(value)
      .then(ca => {
        this.menuService.refreshCourses();
        return ca.assistant;
      });
  }

  delete(element: User): Promise<User> {
    const value = <CourseAssistant>{
      courseId: this.courseId,
      assistant: element
    };
    return this.modelSvcFactory.courseRelations.deleteAssistant(value)
      .then(ca => {
        this.menuService.refreshCourses();
        return ca.assistant;
      });
  }

  ngOnDestroy(): void { }
}
