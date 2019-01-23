import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentTasksEditFormComponent } from './assignment-tasks-edit-form.component';

describe('AssignmentTasksEditFormComponent', () => {
  let component: AssignmentTasksEditFormComponent;
  let fixture: ComponentFixture<AssignmentTasksEditFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentTasksEditFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentTasksEditFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
