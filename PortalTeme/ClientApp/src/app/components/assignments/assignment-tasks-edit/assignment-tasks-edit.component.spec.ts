import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentTasksEditComponent } from './assignment-tasks-edit.component';

describe('AssignmentTasksEditComponent', () => {
  let component: AssignmentTasksEditComponent;
  let fixture: ComponentFixture<AssignmentTasksEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentTasksEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentTasksEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
