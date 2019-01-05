import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'admin-nav-menu',
  templateUrl: './admin-nav-menu.component.html',
  styleUrls: ['./admin-nav-menu.component.scss']
})
export class AdminNavMenuComponent implements OnInit {

  links: any[];
  constructor() {
    this.links = [
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
   }

  ngOnInit() {
  }

}
