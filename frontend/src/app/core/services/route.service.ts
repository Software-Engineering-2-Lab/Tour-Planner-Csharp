import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { RouteResult } from './map-facade.service';
import { TourService } from './tour.service';

export interface RoutePreviewResponse {
    distanceKm: number;
    durationMin: number;
    resolvedFrom: string;
    resolvedTo: string;
}

@Injectable({ providedIn: 'root' })
export class RealRouteService {
    constructor(private tourService: TourService, private http: HttpClient) {}

    getRoutePreview(from: string, to: string, transport: string): Observable<RoutePreviewResponse> {
        const url = `http://localhost:8080/api/tours/preview`; 
        const params = { from, to, transportType: transport };
        return this.http.get<RoutePreviewResponse>(url, { params });
    }

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