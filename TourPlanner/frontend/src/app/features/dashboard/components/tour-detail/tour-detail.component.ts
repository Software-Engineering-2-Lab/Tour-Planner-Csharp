import { Component, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { TourService } from '../../../../core/services/tour.service';
import { TourLog } from '../../../../core/models/tour-log.model';
import { MapComponent } from '../map/map.component';
import { LogModalComponent } from '../modals/log-modal/log-modal.component';
import { TourModalComponent } from '../modals/tour-modal/tour-modal.component';
import { TourPhotosComponent } from '../tour-photos/tour-photos.component';
import { ExportImportService } from '../../../../core/services/export-import.service';
type TourTab = 'details' | 'photos';

@Component({
    selector: 'app-tour-detail',
    standalone: true,
    imports: [CommonModule, MapComponent, LogModalComponent, TourModalComponent, TourPhotosComponent],
    templateUrl: './tour-detail.component.html',
    styleUrl: './tour-detail.component.scss'
})
export class TourDetailComponent {
    private tourService = inject(TourService);
    private dialog = inject(MatDialog);
    private exportImport = inject(ExportImportService);

    activeTab = signal<TourTab>('details');
    searchTerm = signal('');
    
    isLogModalOpen = signal(false);
    isTourModalOpen = signal(false);
    selectedLogForEdit = signal<TourLog | undefined>(undefined);

    selectedTour = this.tourService.selectedTour;

    filteredLogs = computed(() => {
        const term = this.searchTerm().toLowerCase();
        const logs = this.tourService.selectedTourLogs();

        if (!term) return logs;

        return logs.filter(log => {
            const commentMatch = log.comment.toLowerCase().includes(term);
            const dateObj = new Date(log.dateTime);
            const visualDate = `${String(dateObj.getDate()).padStart(2, '0')}.${String(dateObj.getMonth() + 1).padStart(2, '0')}.${String(dateObj.getFullYear()).slice(-2)}`;
            
            return commentMatch || visualDate.includes(term) || log.dateTime.includes(term);
        });
    });

    loadLogs(): void {
        const tour = this.selectedTour();
        if (tour) {
            this.tourService.loadLogsForTour(tour.id);
        }
    }

    openEditTour(): void {
        this.isTourModalOpen.set(true);
    }

    onDeleteTour(): void {
        const tour = this.selectedTour();
        if (tour && confirm('Permanently delete this tour and all its logs?')) {
            this.tourService.deleteTour(tour.id);
        }
    }

    onExport(): void {
        const tour = this.selectedTour();
        if (!tour) return;
        this.exportImport.exportTour(tour);
    }

    openLogModal(): void {
        this.selectedLogForEdit.set(undefined);
        this.isLogModalOpen.set(true);
    }

    openEditLog(log: TourLog): void {
        this.selectedLogForEdit.set(log);
        this.isLogModalOpen.set(true);
    }

    closeLogModal(): void {
        this.isLogModalOpen.set(false);
        this.selectedLogForEdit.set(undefined);
        this.loadLogs();
    }

    onDeleteLog(logId: number): void {
        const tour = this.selectedTour();
        if (tour && confirm('Are you sure you want to delete this log?')) {
            this.tourService.deleteLog(logId, tour.id);
        }
    }

    setActiveTab(tab: TourTab): void {
        this.activeTab.set(tab);
    }

    onSearchChange(event: Event): void {
        this.searchTerm.set((event.target as HTMLInputElement).value);
    }

    getFriendlyLabel(value: number | undefined): string {
        if (value === undefined) return 'MEDIUM';
        if (value >= 8) return 'HARD';
        if (value >= 5) return 'MEDIUM';
        return 'EASY';
    }

    onLocalPhotoAdded(newPhoto: any): void {
        const currentTour = this.selectedTour();
        if (!currentTour) return;

        const updatedImages = currentTour.tourImages ? [...currentTour.tourImages, newPhoto] : [newPhoto];

        this.selectedTour.set({
            ...currentTour,
            tourImages: updatedImages
        });
    }


}