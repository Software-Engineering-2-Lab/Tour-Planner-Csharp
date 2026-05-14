import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourModalComponent } from './tour-modal.component';

describe('TourModalComponent', () => {
	let component: TourModalComponent;
	let fixture: ComponentFixture<TourModalComponent>;

	beforeEach(async () => {
    	await TestBed.configureTestingModule({
      		imports: [TourModalComponent],
    	}).compileComponents();

    	fixture = TestBed.createComponent(TourModalComponent);
    	component = fixture.componentInstance;
    	await fixture.whenStable();
  	});

  	it('should create', () => {
    	expect(component).toBeTruthy();
  	});
});
