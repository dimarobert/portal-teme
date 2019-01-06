import { Component, OnInit, OnDestroy } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../../services/admin-menu.service';

@Component({
  selector: 'app-course-edit-router',
  templateUrl: 'course-edit-router.component.html',
  styleUrls: ['course-edit-router.component.scss']
})
export class CourseEditRouterComponent implements OnInit, OnDestroy {

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.adminMenuSvc.changeMenuState(AdminMenuState.EditCourseMenu);
  }

  ngOnDestroy(): void {
    this.adminMenuSvc.changeMenuState(AdminMenuState.AdminMenu);
  }
}
