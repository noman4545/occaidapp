import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ACIDComponent } from './acid.component';

describe('ACIDComponent', () => {
  let component: ACIDComponent;
  let fixture: ComponentFixture<ACIDComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ACIDComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ACIDComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
