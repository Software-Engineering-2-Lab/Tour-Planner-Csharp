import { Component, EventEmitter, Output, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../../../../core/services/tour.service';
import { TourLog } from '../../../../../core/models/tour-log.model';


@Component({
    selector: 'app-log-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './log-modal.component.html',
    styleUrl: './log-modal.component.scss'
})
export class LogModalComponent {
	@Input() editLog?: TourLog;
    @Input() tourId!: number;
    @Output() close = new EventEmitter<void>();

    dateTime: string = new Date().toISOString().slice(0, 16);
    difficulty: number = 5;
    totalDistance: number = 0;
    totalTime: number = 0;
    rating: number = 3;
    comment: string = '';

    constructor(private tourService: TourService) {}

	ngOnInit(): void {
    	if (this.editLog) {
        	this.dateTime = this.editLog.dateTime;
        	this.difficulty = this.editLog.difficulty;
        	this.totalDistance = this.editLog.totalDistance;
        	this.totalTime = this.editLog.totalTime;
        	this.rating = this.editLog.rating;
        	this.comment = this.editLog.comment;
    	}
	}

	onSave(): void {
    	const logData: TourLog = {
        	id: this.editLog ? this.editLog.id : null,
        	tourId: this.tourId,
        	dateTime: new Date(this.dateTime).toISOString().split('.')[0],
        	comment: this.comment,
        	difficulty: this.difficulty,
        	totalDistance: Number(this.totalDistance),
        	totalTime: Number(this.totalTime),
        	rating: Number(this.rating)
    	};

    	if (this.editLog) {
        	this.tourService.updateLog(logData).subscribe(() => {
            	this.close.emit();
        	});
    	} else {
        	this.tourService.addLog(logData).subscribe(() => {
            	this.close.emit();
        	});
    	}
	}

    setDifficulty(val: number): void {
        this.difficulty = val;
    }

	getDifficultyLabel(val: number): string {
    	if (val >= 8) return 'HARD (' + val + ')';
    	if (val >= 5) return 'MEDIUM (' + val + ')';
    	return 'EASY (' + val + ')';
	}

    setRating(val: number): void {
        this.rating = val;
    }

    onCancel(): void {
        this.close.emit();
    }
}