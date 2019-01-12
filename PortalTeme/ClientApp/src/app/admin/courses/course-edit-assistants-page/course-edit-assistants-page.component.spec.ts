import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEditAssistantsPageComponent } from './course-edit-assistants-page.component';

describe('CourseEditAssistantsPageComponent', () => {
  let component: CourseEditAssistantsPageComponent;
  let fixture: ComponentFixture<CourseEditAssistantsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseEditAssistantsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseEditAssistantsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
