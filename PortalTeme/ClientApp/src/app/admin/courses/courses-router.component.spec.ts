import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursesRouterComponent } from './courses-router.component';

describe('CourseOwnersDefinitionsComponent', () => {
  let component: CoursesRouterComponent;
  let fixture: ComponentFixture<CoursesRouterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CoursesRouterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoursesRouterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
