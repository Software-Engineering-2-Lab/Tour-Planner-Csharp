import { Component, signal, computed, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tour } from '../../../../core/models/tour.model';
import { TourService } from '../../../../core/services/tour.service';
import { TourModalComponent } from '../modals/tour-modal/tour-modal.component';

@Component({
    selector: 'app-tour-list',
    standalone: true,
    imports: [CommonModule, TourModalComponent],
    templateUrl: './tour-list.component.html',
    styleUrl: './tour-list.component.scss'
})
export class TourListComponent {
    private tourService = inject(TourService);
    tourSelected = output<void>()
    isTourModalOpen = signal(false);
    tourSearchTerm = signal('');
    
    tours = this.tourService.tours;

    filteredTours = computed(() => {
        const term = this.tourSearchTerm().toLowerCase();
        const allTours = this.tourService.tours();
        
        if (!term) return allTours;

        return allTours.filter(t => 
            t.name.toLowerCase().includes(term) || 
            t.fromLocation.toLowerCase().includes(term) ||
            t.toLocation.toLowerCase().includes(term)
        );
    });

    selectedTour = this.tourService.selectedTour;

    onSearchChange(event: Event): void {
        const value = (event.target as HTMLInputElement).value;
        this.tourSearchTerm.set(value);
    }

    selectTour(tour: Tour): void {
        this.tourService.selectTour(tour);
    }

    openAddTour(): void {
        this.isTourModalOpen.set(true);
    }

    closeTourModal(): void {
        this.isTourModalOpen.set(false);
    }
}