import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TMCSComponent } from './tmcs.component';

describe('TMCSComponent', () => {
  let component: TMCSComponent;
  let fixture: ComponentFixture<TMCSComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TMCSComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TMCSComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
