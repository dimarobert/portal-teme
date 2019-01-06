import { Component } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../../services/admin-menu.service';

@Component({
  selector: 'app-course-edit',
  templateUrl: 'course-edit.component.html',
  styleUrls: ['course-edit.component.scss']
})
export class CourseEditComponent {

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.adminMenuSvc.changeMenuState(AdminMenuState.EditCourseMenu);
  }
  
}
