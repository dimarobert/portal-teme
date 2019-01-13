import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentEditFormComponent } from './assignment-edit-form.component';

describe('AssignmentEditFormComponent', () => {
  let component: AssignmentEditFormComponent;
  let fixture: ComponentFixture<AssignmentEditFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentEditFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentEditFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
