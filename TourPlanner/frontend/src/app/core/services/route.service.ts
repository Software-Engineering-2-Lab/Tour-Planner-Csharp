import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RouteResult } from './map-facade.service';
import { TourService } from './tour.service';

@Injectable({ providedIn: 'root' })
export class RealRouteService {
    constructor(private tourService: TourService) {}

    getRouteForSelectedTour(): Observable<RouteResult> {
        return new Observable(observer => {
            const tour = this.tourService.selectedTour();
            if (!tour?.routeImagePath) {
                observer.error('No geometry available');
                return;
            }

            try {
                // ORS stores as [[lon,lat],[lon,lat]...]
                // Leaflet needs [lat,lon]
                const raw: [number, number][] = JSON.parse(tour.routeImagePath);
                const coordinates: [number, number][] = raw.map(([lon, lat]) => [lat, lon]);

                observer.next({
                    coordinates,
                    distanceKm: tour.distance,
                    durationMin: tour.estimatedTime
                });
                observer.complete();
            } catch {
                observer.error('Failed to parse geometry');
            }
        });
    }
}