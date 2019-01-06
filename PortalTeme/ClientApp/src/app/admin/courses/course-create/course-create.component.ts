import { Component, OnInit } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../../services/admin-menu.service';

@Component({
  selector: 'app-course-create',
  templateUrl: './course-create.component.html',
  styleUrls: ['./course-create.component.scss']
})
export class CourseCreateComponent implements OnInit {

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.adminMenuSvc.changeMenuState(AdminMenuState.CreateCourseMenu);
  }

}
