import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseRouterComponent } from './course-router.component';

describe('CourseRouterComponent', () => {
  let component: CourseRouterComponent;
  let fixture: ComponentFixture<CourseRouterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseRouterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseRouterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
