import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TableEditableCellComponent } from './table-editable-cell.component';

describe('TableEditableCellComponent', () => {
  let component: TableEditableCellComponent;
  let fixture: ComponentFixture<TableEditableCellComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TableEditableCellComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TableEditableCellComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
