import { Component, OnInit } from '@angular/core';
import { CollapseOption, MenuService } from '../../services/menu.service';

@Component({
  selector: 'app-course-router',
  templateUrl: './course-router.component.html',
  styleUrls: ['./course-router.component.scss']
})
export class CourseRouterComponent implements OnInit {

  constructor(private menuService: MenuService) { }

  ngOnInit() {
    this.menuService.setCourseCollapse(CollapseOption.Open);
  }

}
