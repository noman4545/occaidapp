import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TMCSLatestComponent } from './tmcs-latest.component';

describe('TMCSComponent', () => {
  let component: TMCSLatestComponent;
  let fixture: ComponentFixture<TMCSLatestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TMCSLatestComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TMCSLatestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
