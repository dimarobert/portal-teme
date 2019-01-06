import { Component, OnInit } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../../services/admin-menu.service';

@Component({
    selector: 'app-course-owner-basic',
    templateUrl: 'basic.component.html',
    styleUrls: ['basic.component.scss']
})
export class CourseOwnerBasicComponent implements OnInit {

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.adminMenuSvc.changeMenuState(AdminMenuState.CreateCourseMenu);
  }
}

