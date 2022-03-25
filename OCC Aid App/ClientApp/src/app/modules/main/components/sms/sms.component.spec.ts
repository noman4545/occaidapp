import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SMSComponent } from './sms.component';

describe('SMSComponent', () => {
  let component: SMSComponent;
  let fixture: ComponentFixture<SMSComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SMSComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SMSComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
