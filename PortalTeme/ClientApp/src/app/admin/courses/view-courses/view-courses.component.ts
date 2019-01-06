import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { ModelServiceFactory } from '../../../services/model.service';
import { DataTableColumns } from '../../../models/column-definition.model';
import { ModelAccessor, BaseModelAccessor } from '../../../models/model.accessor';
import { Course, CourseDefinitionRef, User, CourseEdit } from '../../../models/course.model';
import { RelatedItemAccessor, NamedModelItemAccessor } from '../../../models/item.accesor';
import { nameof } from '../../../type-guards/nameof.guard';
import { AdminMenuService, AdminMenuState } from '../../services/admin-menu.service';

@Component({
  selector: 'app-view-courses',
  templateUrl: './view-courses.component.html',
  styleUrls: ['./view-courses.component.scss']
})
export class ViewCoursesComponent implements OnInit {

  constructor(private modelSvcFactory: ModelServiceFactory) { }

  columnDefs: DataTableColumns;

  modelAccessor: ModelAccessor;
  data: BehaviorSubject<Course[]>;

  ngOnInit() {
    this.save = this.save.bind(this);
    this.delete = this.delete.bind(this);

    this.modelAccessor = new BaseModelAccessor();
    this.data = new BehaviorSubject([]);

    this.columnDefs = new DataTableColumns([
      {
        id: nameof<Course>('courseDef'),
        title: 'Course',
        itemAccessor: new RelatedItemAccessor<CourseDefinitionRef>(cd => cd.name)
      },
      {
        id: nameof<Course>('professor'),
        title: 'Professor',
        itemAccessor: new RelatedItemAccessor<User>(p => `${p.firstName} ${p.lastName}`)
      }],
      {
        itemAccesor: new NamedModelItemAccessor()
      });

    this.getData();
  }

  private getData() {
    this.modelSvcFactory.courses.getAll()
      .pipe(take(1))
      .subscribe(results => {
        this.data.next(results);
      });
  }

  save(element: CourseEdit): Promise<CourseEdit> {
    return this.modelSvcFactory.courses.save(element);
  }

  delete(element: Course): Promise<Course> {
    return this.modelSvcFactory.courses.delete(element.id);
  }
}
