import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEditAttendeesPageComponent } from './course-edit-attendees-page.component';

describe('CourseEditAttendeesPageComponent', () => {
  let component: CourseEditAttendeesPageComponent;
  let fixture: ComponentFixture<CourseEditAttendeesPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseEditAttendeesPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseEditAttendeesPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
