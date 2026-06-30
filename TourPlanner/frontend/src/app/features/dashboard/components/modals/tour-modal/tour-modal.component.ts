import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../../../../core/services/tour.service';
import { Tour, CreateTourDto } from '../../../../../core/models/tour.model';
import { AuthService } from '../../../../../core/services/auth.service';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

import { RealRouteService } from '../../../../../core/services/route.service';

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
    private routeService = inject(RealRouteService);

    name: string = '';
    description: string = '';
    fromLocation: string = '';
    toLocation: string = '';
    transportType: 'BIKE' | 'HIKE' | 'WALK' | 'DRIVE' = 'DRIVE';

    distance: number = 0;
    estimatedTime: number = 0;

    resolvedFromLocation: string = '';
    resolvedToLocation: string = '';
    
    private inputSubject = new Subject<void>();

    constructor(private tourService: TourService) {
        this.inputSubject.pipe(
            debounceTime(800),
            takeUntilDestroyed()
        ).subscribe(() => {
            this.calculateRouteIfReady();
        });
    }

    setTransport(type: 'BIKE' | 'HIKE' | 'WALK' | 'DRIVE'): void {
        this.transportType = type;
        this.inputSubject.next(); 
    }

    onInputChange(): void {
        this.inputSubject.next(); 
    }

    private calculateRouteIfReady(): void {
        if (this.fromLocation.trim() && this.toLocation.trim() && this.transportType) {

            console.log('Fetching route for:', this.fromLocation, '->', this.toLocation, 'via', this.transportType);

            this.routeService.getRoutePreview(this.fromLocation, this.toLocation, this.transportType)
                .subscribe({
                    next: (response) => {
                        this.resolvedFromLocation = response.resolvedFrom;
                        this.resolvedToLocation = response.resolvedTo;
                        this.distance = response.distanceKm;
                        this.estimatedTime = response.durationMin;
                    },
                    error: (err) => {
                        console.error('Eroare la obținerea preview-ului rutei', err);
                        this.resolvedFromLocation = 'Location not found / Error';
                        this.resolvedToLocation = 'Location not found / Error';
                    }
                });

        } else {
            this.resolvedFromLocation = '';
            this.resolvedToLocation = '';
            this.distance = 0;
            this.estimatedTime = 0;
        }
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

    formatTime(minutes: number): string {
        const h = Math.floor(minutes / 60);
        const m = Math.round(minutes % 60);
        if (h === 0) return `${m} min`;
        if (m === 0) return `${h}h`;
        return `${h}h ${m} min`;
    }
}