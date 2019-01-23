import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabbedMenuComponent } from './tabbed-menu.component';

describe('TabbedMenuComponent', () => {
  let component: TabbedMenuComponent;
  let fixture: ComponentFixture<TabbedMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TabbedMenuComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabbedMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
