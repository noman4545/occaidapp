import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TMCSAdminComponent } from './tmcs-admin.component';

describe('TMCSComponent', () => {
  let component: TMCSAdminComponent;
  let fixture: ComponentFixture<TMCSAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TMCSAdminComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TMCSAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
