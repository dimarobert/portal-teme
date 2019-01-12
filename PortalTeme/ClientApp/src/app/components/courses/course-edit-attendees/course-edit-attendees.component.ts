import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';

import { nameof } from '../../../type-guards/nameof.guard';
import { DatasourceColumnDefinition, ColumnType } from './../../../models/column-definition.model';
import { ModelServiceFactory } from '../../../services/model.service';
import { NamedModelItemDatasource, CustomModelItemDatasource } from '../../../datasources/named-model.item-datasource';
import { ModelAccessor, CourseGroupModelAccessor, BaseModelAccessor } from '../../../models/model.accessor';
import { CustomItemAccessor } from '../../..//models/item.accesor';

import { StudyGroup } from './../../../models/study-group.model';
import { Course, User, CourseGroup, CourseStudent } from './../../../models/course.model';
import { DataTableColumns } from '../../../models/column-definition.model';


@Component({
  selector: 'app-course-edit-attendees',
  templateUrl: './course-edit-attendees.component.html',
  styleUrls: ['./course-edit-attendees.component.scss']
})
export class CourseEditAttendeesComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory, private route: ActivatedRoute) { }

  @Input() courseId: string;

  studentColumnDefs: DataTableColumns;
  groupColumnDefs: DataTableColumns;

  studentList: BehaviorSubject<User[]>;
  groupList: BehaviorSubject<CourseGroup[]>;

  studyGroups: BehaviorSubject<StudyGroup[]>;
  students: BehaviorSubject<User[]>;

  private currentCourse: Course;

  groupsModelAccessor: ModelAccessor;
  studentsModelAccessor: ModelAccessor;

  ngOnInit() {
    this.saveGroup = this.saveGroup.bind(this);
    this.deleteGroup = this.deleteGroup.bind(this);

    this.saveStudent = this.saveStudent.bind(this);
    this.deleteStudent = this.deleteStudent.bind(this);

    this.groupList = new BehaviorSubject([]);
    this.studentList = new BehaviorSubject([]);

    this.studyGroups = new BehaviorSubject([]);
    this.students = new BehaviorSubject([]);

    this.groupsModelAccessor = new CourseGroupModelAccessor();
    this.studentsModelAccessor = new BaseModelAccessor();

    this.groupColumnDefs = new DataTableColumns([
      <DatasourceColumnDefinition<StudyGroup>>{
        id: nameof<CourseGroup>('groupId'),
        title: 'Study Group',
        type: ColumnType.Select,
        datasource: new NamedModelItemDatasource<StudyGroup>(this.studyGroups, 'group'),
      }
    ]);

    this.studentColumnDefs = new DataTableColumns([
      <DatasourceColumnDefinition<User>>{
        id: nameof<User>('id'),
        title: 'Student',
        type: ColumnType.Select,
        datasource: new CustomModelItemDatasource<User>({
          getItems: () => this.students,
          getTitle: item => `${item.firstName} ${item.lastName}`,
          getValue: item => item.id,
          findByValue: (item, value) => item.id == value,
          modelName: 'student'
        }),
        itemAccessor: new CustomItemAccessor<User>(null, item => item.id, (item, value) => {
          item.id = value;
        })
      }
    ]);

    this.getData();
  }

  private getData() {
    const studyGroups$ = this.modelSvcFactory.studyGroups.getAll();
    const students$ = this.modelSvcFactory.users.getStudents();
    const currentCourse$ = this.modelSvcFactory.courses.get(this.courseId);

    forkJoin(
      studyGroups$.pipe(take(1)),
      students$.pipe(take(1)),
      currentCourse$.pipe(take(1))
    ).subscribe(results => {
      this.studyGroups.next(results[0]);
      this.students.next(results[1]);
      this.currentCourse = results[2];

      this.groupList.next(this.currentCourse.groups);
      this.studentList.next(this.currentCourse.students);
    });
  }

  saveGroup(element: CourseGroup): Promise<CourseGroup> {
    element.courseId = this.currentCourse.id;
    return this.modelSvcFactory.courseRelations.addGroup(element);
  }

  deleteGroup(element: CourseGroup): Promise<CourseGroup> {
    element.courseId = this.currentCourse.id;
    return this.modelSvcFactory.courseRelations.deleteGroup(element);
  }


  saveStudent(element: User): Promise<User> {
    const value = <CourseStudent>{
      courseId: this.courseId,
      student: element
    };
    return this.modelSvcFactory.courseRelations.addStudent(value)
      .then(cs => cs.student);
  }

  deleteStudent(element: User): Promise<User> {
    const value = <CourseStudent>{
      courseId: this.courseId,
      student: element
    };
    return this.modelSvcFactory.courseRelations.deleteStudent(value)
      .then(cs => cs.student);
  }
}
