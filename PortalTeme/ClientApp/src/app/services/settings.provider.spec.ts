import { TestBed } from '@angular/core/testing';

import { SettingsProvider } from './settings.provider';

describe('SettingsProvider', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SettingsProvider = TestBed.get(SettingsProvider);
    expect(service).toBeTruthy();
  });
});
