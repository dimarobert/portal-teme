import { Component, OnInit } from '@angular/core';
import { AdminMenuService, AdminMenuState } from '../services/admin-menu.service';

@Component({
  selector: 'admin-nav-menu',
  templateUrl: './admin-nav-menu.component.html',
  styleUrls: ['./admin-nav-menu.component.scss']
})
export class AdminNavMenuComponent implements OnInit {

  adminLinks: any[];
  createCourseLinks: any[];

  AdminMenuState = AdminMenuState;

  get menuState() {
    return this.adminMenuSvc.menuState;
  }

  constructor(private adminMenuSvc: AdminMenuService) {

    this.adminLinks = [
      {
        label: 'Years',
        link: '/admin/years',
        index: 0
      },
      {
        label: 'Study Domains',
        link: '/admin/study-domains',
        index: 0
      },
      {
        label: 'Study Groups',
        link: '/admin/study-groups',
        index: 0
      },
      {
        label: 'Course Definitions',
        link: '/admin/courses',
        index: 0
      },
      {
        label: 'Course Users (Profersors)',
        link: '/admin/courses-owners',
        index: 0
      }
    ];

    this.createCourseLinks = [
      {
        label: 'Back',
        link: '/admin/courses-owners',
        index: 0
      },
      {
        label: 'Basic Information',
        link: '/admin/courses-owners/Basic',
        index: 0
      },
      {
        label: 'Assitants',
        link: '/admin/courses-owners/assitants',
        index: 0
      },
      {
        label: 'Attentees',
        link: '/admin/courses-owners/attentees',
        index: 0
      }
    ];
  }

  ngOnInit() {
  }

}
