import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSubmissionPageComponent } from './create-submission-page.component';

describe('CreateSubmissionPageComponent', () => {
  let component: CreateSubmissionPageComponent;
  let fixture: ComponentFixture<CreateSubmissionPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateSubmissionPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateSubmissionPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
