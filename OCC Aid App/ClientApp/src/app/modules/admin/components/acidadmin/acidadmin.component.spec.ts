import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ACIDAdminComponent } from './acidadmin.component';

describe('ACIDAdminComponent', () => {
  let component: ACIDAdminComponent;
  let fixture: ComponentFixture<ACIDAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ACIDAdminComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ACIDAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
