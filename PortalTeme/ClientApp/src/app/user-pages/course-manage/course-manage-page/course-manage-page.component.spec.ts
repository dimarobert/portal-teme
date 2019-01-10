import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseManagePageComponent } from './course-manage-page.component';

describe('CourseManagePageComponent', () => {
  let component: CourseManagePageComponent;
  let fixture: ComponentFixture<CourseManagePageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseManagePageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseManagePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
