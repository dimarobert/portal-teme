import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminMenuService, AdminMenuState } from '../services/admin-menu.service';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.scss']
})
export class AdminPageComponent implements OnInit {

  constructor(private adminMenuSvc: AdminMenuService) { }

  ngOnInit() {
    this.adminMenuSvc.changeMenuState(AdminMenuState.AdminMenu);
  }

}
