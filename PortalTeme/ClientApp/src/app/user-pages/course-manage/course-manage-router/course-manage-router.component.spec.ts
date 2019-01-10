import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseManageRouterComponent } from './course-manage-router.component';

describe('CourseManageRouterComponent', () => {
  let component: CourseManageRouterComponent;
  let fixture: ComponentFixture<CourseManageRouterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseManageRouterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseManageRouterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
