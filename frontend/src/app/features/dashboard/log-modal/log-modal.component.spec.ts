import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogModalComponent } from './log-modal.component';

describe('LogModalComponent', () => {
  let component: LogModalComponent;
  let fixture: ComponentFixture<LogModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LogModalComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(LogModalComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
