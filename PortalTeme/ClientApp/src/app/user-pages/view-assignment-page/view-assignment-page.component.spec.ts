import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAssignmentPageComponent } from './view-assignment-page.component';

describe('ViewAssignmentPageComponent', () => {
  let component: ViewAssignmentPageComponent;
  let fixture: ComponentFixture<ViewAssignmentPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewAssignmentPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAssignmentPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
