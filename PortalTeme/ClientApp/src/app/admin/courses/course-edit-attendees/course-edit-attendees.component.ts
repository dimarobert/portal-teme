import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';

import { nameof } from '../../../type-guards/nameof.guard';
import { DatasourceColumnDefinition, ColumnType } from './../../../models/column-definition.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { NamedModelItemDatasource } from '../../../datasources/named-model.item-datasource';
import { ModelAccessor, CourseGroupModelAccessor } from '../../../models/model.accessor';

import { StudyGroup } from './../../../models/study-group.model';
import { Course, User, StudyGroupRef } from './../../../models/course.model';
import { DataTableColumns } from '../../../models/column-definition.model';
import { RelatedItemAccessor } from '../../../models/item.accesor';


@Component({
  selector: 'app-course-edit-attendees',
  templateUrl: './course-edit-attendees.component.html',
  styleUrls: ['./course-edit-attendees.component.scss']
})
export class CourseEditAttendeesComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory, private route: ActivatedRoute) { }

  studentColumnDefs: DataTableColumns;
  groupColumnDefs: DataTableColumns;
  studentList: BehaviorSubject<User[]>;
  groupList: BehaviorSubject<StudyGroupRef[]>;

  studyGroups: BehaviorSubject<StudyGroup[]>;

  private currentCourse: Course;
  private courseId: string;

  modelAccessor: ModelAccessor;

  ngOnInit() {
    this.saveGroup = this.saveGroup.bind(this);
    this.deleteGroup = this.deleteGroup.bind(this);

    this.groupList = new BehaviorSubject([]);
    this.studyGroups = new BehaviorSubject([]);
    this.courseId = this.route.parent.snapshot.paramMap.get('id');

    this.modelAccessor = new CourseGroupModelAccessor();

    this.groupColumnDefs = new DataTableColumns([
      <DatasourceColumnDefinition<StudyGroup>>{
        id: nameof<StudyGroupRef>('groupId'),
        title: 'Study Group',
        type: ColumnType.Select,
        datasource: new NamedModelItemDatasource<StudyGroup>(this.studyGroups, 'group'),
        // itemAccessor: new RelatedItemAccessor<StudyGroupRef>(item => item.name)
      }
    ]
    );

    this.getData();
  }

  private getData() {
    const studyGroups$ = this.modelSvcFactory.studyGroups.getAll();
    const currentCourse$ = this.modelSvcFactory.courses.get(this.courseId);

    forkJoin(
      studyGroups$.pipe(take(1)),
      currentCourse$.pipe(take(1))
    ).subscribe(results => {
      this.studyGroups.next(results[0]);
      this.currentCourse = results[1];

      this.groupList.next(this.currentCourse.groups);
    }
    );
  }

  saveGroup(element: StudyGroupRef): Promise<StudyGroupRef> {
    element.courseId = this.currentCourse.id;
    return this.modelSvcFactory.courseRelations.addGroup(element);
  }

  deleteGroup(element: StudyGroupRef): Promise<StudyGroupRef> {
    element.courseId = this.currentCourse.id;
    return this.modelSvcFactory.courseRelations.deleteGroup(element);
  }

}
