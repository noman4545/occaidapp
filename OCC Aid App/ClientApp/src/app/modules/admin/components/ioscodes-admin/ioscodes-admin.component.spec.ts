import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IOSCodesAdminComponent } from './ioscodes-admin.component';

describe('IOSCodesAdminComponent', () => {
  let component: IOSCodesAdminComponent;
  let fixture: ComponentFixture<IOSCodesAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IOSCodesAdminComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IOSCodesAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
