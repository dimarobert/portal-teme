import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseOwnersDefinitionsComponent } from './course-owners.component';

describe('CourseOwnersDefinitionsComponent', () => {
  let component: CourseOwnersDefinitionsComponent;
  let fixture: ComponentFixture<CourseOwnersDefinitionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseOwnersDefinitionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseOwnersDefinitionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
