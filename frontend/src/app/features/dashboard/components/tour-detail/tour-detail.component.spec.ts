import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourDetailComponent } from './tour-detail.component';

describe('TourDetailComponent', () => {
  let component: TourDetailComponent;
  let fixture: ComponentFixture<TourDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourDetailComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TourDetailComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
