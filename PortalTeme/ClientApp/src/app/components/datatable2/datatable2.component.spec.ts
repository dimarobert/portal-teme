import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DataTableComponent2 } from './datatable2.component';

describe('DataTableComponent2', () => {
  let component: DataTableComponent2;
  let fixture: ComponentFixture<DataTableComponent2>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DataTableComponent2 ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DataTableComponent2);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
