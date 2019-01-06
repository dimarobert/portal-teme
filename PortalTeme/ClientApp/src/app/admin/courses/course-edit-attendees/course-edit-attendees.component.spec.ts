import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEditAttendeesComponent } from './course-edit-attendees.component';

describe('CourseEditAttendeesComponent', () => {
  let component: CourseEditAttendeesComponent;
  let fixture: ComponentFixture<CourseEditAttendeesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseEditAttendeesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseEditAttendeesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
