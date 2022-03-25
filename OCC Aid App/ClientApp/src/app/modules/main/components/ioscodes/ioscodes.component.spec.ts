import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IOSCodesComponent } from './ioscodes.component';

describe('IOSCodesComponent', () => {
  let component: IOSCodesComponent;
  let fixture: ComponentFixture<IOSCodesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IOSCodesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IOSCodesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
