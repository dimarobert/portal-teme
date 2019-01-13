import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentEditPageComponent } from './assignment-edit-page.component';

describe('AssignmentEditPageComponent', () => {
  let component: AssignmentEditPageComponent;
  let fixture: ComponentFixture<AssignmentEditPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentEditPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentEditPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
