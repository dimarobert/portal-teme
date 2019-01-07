import { StudyGroup } from './../../../models/study-group.model';
import { DatasourceColumnDefinition, ColumnType } from './../../../models/column-definition.model';
import { Course, User, StudyGroupRef } from './../../../models/course.model';
import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';
import { nameof } from '../../../type-guards/nameof.guard';
import { ModelServiceFactory } from '../../../services/model.service';
import { DataTableColumns } from '../../../models/column-definition.model';
import { NamedModelItemDatasource } from '../../../datasources/named-model.item-datasource';
import { ActivatedRoute } from '@angular/router';
import { ModelAccessor, BaseModelAccessor } from '../../../models/model.accessor';


@Component({
  selector: 'app-course-edit-attendees',
  templateUrl: './course-edit-attendees.component.html',
  styleUrls: ['./course-edit-attendees.component.scss']
})
export class CourseEditAttendeesComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory,private route: ActivatedRoute) { }

  studentColumnDefs: DataTableColumns;
  groupColumnDefs: DataTableColumns;
  studentList: BehaviorSubject<User[]>;
  groupList: BehaviorSubject<StudyGroupRef[]>;

  studyGroups: BehaviorSubject<StudyGroup[]>;
  courseGroups: BehaviorSubject<StudyGroupRef[]>;
  private currentCourse: BehaviorSubject<Course>;
  private courseId: string;

  modelAccessor: ModelAccessor;

  ngOnInit() {

    this.groupList = new BehaviorSubject([]);
    this.courseId = this.route.snapshot.paramMap.get('id');

    this.modelAccessor = new BaseModelAccessor();

    this.groupColumnDefs = new DataTableColumns([
      <DatasourceColumnDefinition<StudyGroup>>{
        id: nameof<StudyGroupRef>('name'),
        title: 'Study Group',
        type: ColumnType.Select,
        datasource: new NamedModelItemDatasource<StudyGroup>(this.studyGroups, 'group')
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
        this.currentCourse.next(results[1]);
        this.courseGroups.next(results[1].groups);
      }
    );
  }

  saveGroups(element: StudyGroupRef): Promise<StudyGroupRef> {
    return null //this.modelSvcFactory.courseDefinitions.save(element);
  }

  deleteGroups(element: StudyGroupRef): Promise<StudyGroupRef> {
    return null //this.modelSvcFactory.courseDefinitions.delete(element.id);
  }

}
