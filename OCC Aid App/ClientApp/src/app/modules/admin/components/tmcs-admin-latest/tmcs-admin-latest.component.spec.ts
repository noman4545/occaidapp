import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TMCSAdminComponentLatest } from './tmcs-admin-latest.component';

describe('TMCSComponent', () => {
  let component: TMCSAdminComponentLatest;
  let fixture: ComponentFixture<TMCSAdminComponentLatest>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TMCSAdminComponentLatest]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TMCSAdminComponentLatest);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
