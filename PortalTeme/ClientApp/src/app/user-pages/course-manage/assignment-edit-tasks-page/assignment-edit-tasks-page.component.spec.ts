import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentEditTasksPageComponent } from './assignment-edit-tasks-page.component';

describe('AssignmentEditTasksPageComponent', () => {
  let component: AssignmentEditTasksPageComponent;
  let fixture: ComponentFixture<AssignmentEditTasksPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentEditTasksPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentEditTasksPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
