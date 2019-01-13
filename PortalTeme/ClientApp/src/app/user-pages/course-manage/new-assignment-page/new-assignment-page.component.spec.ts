import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewAssignmentPageComponent } from './new-assignment-page.component';

describe('NewAssignmentPageComponent', () => {
  let component: NewAssignmentPageComponent;
  let fixture: ComponentFixture<NewAssignmentPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewAssignmentPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewAssignmentPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
