import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DropzoneFileUploadComponent } from './dropzone-file-upload.component';

describe('DropzoneFileUploadComponent', () => {
  let component: DropzoneFileUploadComponent;
  let fixture: ComponentFixture<DropzoneFileUploadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DropzoneFileUploadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DropzoneFileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
