import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudyDomainsComponent } from './study-domains.component';

describe('StudyDomainsComponent', () => {
  let component: StudyDomainsComponent;
  let fixture: ComponentFixture<StudyDomainsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudyDomainsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudyDomainsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
