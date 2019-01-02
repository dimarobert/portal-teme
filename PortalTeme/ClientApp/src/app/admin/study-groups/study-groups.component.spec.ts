import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudyGroupsComponent } from './study-groups.component';

describe('StudyGroupsComponent', () => {
  let component: StudyGroupsComponent;
  let fixture: ComponentFixture<StudyGroupsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudyGroupsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudyGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
