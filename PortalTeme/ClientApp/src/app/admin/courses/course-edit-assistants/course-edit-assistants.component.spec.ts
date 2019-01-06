import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEditAssistantsComponent } from './course-edit-assistants.component';

describe('CourseEditAssistantsComponent', () => {
  let component: CourseEditAssistantsComponent;
  let fixture: ComponentFixture<CourseEditAssistantsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseEditAssistantsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseEditAssistantsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
