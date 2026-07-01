import {
    Component,
    OnInit,
    OnDestroy,
    signal,
    inject,
    effect
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MapFacadeService } from '../../../core/services/map-facade.service';
import { RealRouteService } from '../../../core/services/route.service';
import { TourService } from '../../../core/services/tour.service';

@Component({
    selector: 'app-map',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './map.component.html',
    styleUrl: './map.component.scss',
})
export class MapComponent implements OnInit, OnDestroy {
    private mapFacade = inject(MapFacadeService);
    private realRouteService = inject(RealRouteService);
    private tourService = inject(TourService);

    readonly isLoading = signal(false);
    readonly routeInfo = signal<{ distanceKm: number; durationMin: number } | null>(null);

    constructor() {
        effect(() => {
            const tour = this.tourService.selectedTour();
            if (tour) {
                this.loadRouteForTour();
            } else {
                this.mapFacade.clearMarkers();
                this.mapFacade.clearRoute();
                this.routeInfo.set(null);
            }
        });
    }

    ngOnInit(): void {
        this.mapFacade.initMap('map-container');
        this.loadRouteForTour();
    }

    ngOnDestroy(): void {
        this.mapFacade.destroyMap();
    }

    private loadRouteForTour(): void {
        const tour = this.tourService.selectedTour();
        if (!tour?.routeImagePath) return;

        this.isLoading.set(true);
        this.mapFacade.clearMarkers();
        this.mapFacade.clearRoute();

        this.realRouteService.getRouteForSelectedTour().subscribe({
            next: (result) => {
                this.mapFacade.drawRoute(result.coordinates);

                const start = result.coordinates[0];
                const end = result.coordinates[result.coordinates.length - 1];
                this.mapFacade.setMarker(start[0], start[1], `Start: ${tour.fromLocation}`);
                this.mapFacade.setMarker(end[0], end[1], `End: ${tour.toLocation}`);

                this.routeInfo.set({
                    distanceKm: result.distanceKm,
                    durationMin: result.durationMin,
                });
                this.isLoading.set(false);
            },
            error: () => this.isLoading.set(false),
        });
    }
}