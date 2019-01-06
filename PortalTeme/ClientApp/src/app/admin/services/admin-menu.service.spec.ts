import { TestBed } from '@angular/core/testing';

import { AdminMenuService } from './admin-menu.service';

describe('AdminMenuService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AdminMenuService = TestBed.get(AdminMenuService);
    expect(service).toBeTruthy();
  });
});
