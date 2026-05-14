import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../../../../core/services/tour.service';
import { Tour, CreateTourDto } from '../../../../../core/models/tour.model';
import { AuthService } from '../../../../../core/services/auth.service';

@Component({
    selector: 'app-tour-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './tour-modal.component.html',
    styleUrl: './tour-modal.component.scss'
})
export class TourModalComponent implements OnInit {
    @Input() editTour?: Tour;
    @Output() close = new EventEmitter<void>();

    private authService = inject(AuthService);

    name: string = '';
    description: string = '';
    fromLocation: string = '';
    toLocation: string = '';
    transportType: 'BIKE' | 'HIKE' | 'RUNNING' | 'VACATION' = 'BIKE';

    distance: number = 0;
    estimatedTime: number = 0;

    setTransport(type: 'BIKE' | 'HIKE' | 'RUNNING' | 'VACATION'): void {
        this.transportType = type;
    }

    onSave(): void {
        const currentUserId = this.authService.getUserId();

        if (currentUserId === 0) {
            alert("Session expired. Please log in again.");
            return;
        }
        if (!this.name || !this.fromLocation || !this.toLocation) {
            alert("Please fill in the required fields!");
            return;
        }
        if (this.editTour) {
            const updatedData: Tour = {
                ...this.editTour,
                name: this.name,
                description: this.description,
                fromLocation: this.fromLocation,
                toLocation: this.toLocation,
                transportType: this.transportType,
                distance: this.distance,
                estimatedTime: this.estimatedTime
            };
            this.tourService.updateTour(updatedData);
        } else {
            const newTour = {
                name: this.name,
                description: this.description,
                fromLocation: this.fromLocation,
                toLocation: this.toLocation,
                transportType: this.transportType,
                distance: Number(this.distance),
                estimatedTime: Number(this.estimatedTime),
                routeImagePath: 'default.png',
                popularity: 0,
                childFriendliness: 0,
                userId: Number(localStorage.getItem('userId'))
            }
            this.tourService.addTour(newTour);
        }
        this.close.emit();
    }

    constructor(private tourService: TourService) {}

    ngOnInit(): void {
        if (this.editTour) {
            this.name = this.editTour.name;
            this.fromLocation = this.editTour.fromLocation;
            this.toLocation = this.editTour.toLocation;
            this.transportType = this.editTour.transportType;
            this.description = this.editTour.description;
            this.distance = this.editTour.distance;
            this.estimatedTime = this.editTour.estimatedTime;
        }
    }

    onCancel(): void {
        this.close.emit();
    }
}