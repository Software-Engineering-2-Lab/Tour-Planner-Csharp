import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourPhotosComponent } from './tour-photos.component';

describe('TourPhotosComponent', () => {
  let component: TourPhotosComponent;
  let fixture: ComponentFixture<TourPhotosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourPhotosComponent ],
    }).compileComponents();

    fixture = TestBed.createComponent(TourPhotosComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
