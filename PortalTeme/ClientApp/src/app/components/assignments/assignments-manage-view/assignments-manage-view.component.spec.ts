import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentsManageViewComponent } from './assignments-manage-view.component';

describe('AssignmentsManageViewComponent', () => {
  let component: AssignmentsManageViewComponent;
  let fixture: ComponentFixture<AssignmentsManageViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignmentsManageViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentsManageViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
