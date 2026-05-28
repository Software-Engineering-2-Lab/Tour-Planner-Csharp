import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { RouteResult } from './map-facade.service';

export interface RouteRequest {
  fromName: string;
  toName: string;
}


const MOCK_ROUTES: Record<string, RouteResult> = {
  'vienna-graz': {
    distanceKm: 195,
    durationMin: 120,
    coordinates: [
      [48.2082, 16.3738],
      [47.8, 15.9],
      [47.5, 15.4],
      [47.0707, 15.4395], 
    ],
  },
  'vienna-salzburg': {
    distanceKm: 297,
    durationMin: 180,
    coordinates: [
      [48.2082, 16.3738], 
      [48.1, 15.6],
      [47.9, 14.5],
      [47.8, 13.8],
      [47.7997, 13.0446], 
    ],
  },
  'vienna-linz': {
    distanceKm: 187,
    durationMin: 110,
    coordinates: [
      [48.2082, 16.3738],
      [48.2, 15.6],
      [48.25, 14.8],
      [48.3058, 14.2865], 
    ],
  },
};

const FALLBACK_ROUTE: RouteResult = {
  distanceKm: 42,
  durationMin: 30,
  coordinates: [
    [48.2082, 16.3738],
    [48.22, 16.4],
    [48.24, 16.42],
  ],
};

@Injectable({
  providedIn: 'root',
})
export class MockRouteService {
  
  getRoute(request: RouteRequest): Observable<RouteResult> {
    const key = `${request.fromName.toLowerCase()}-${request.toName.toLowerCase()}`;
    const result = MOCK_ROUTES[key] ?? FALLBACK_ROUTE;

    return of(result).pipe(delay(400));
  }

  getAvailableRoutes(): { label: string; from: string; to: string }[] {
    return [
      { label: 'Vienna → Graz', from: 'vienna', to: 'graz' },
      { label: 'Vienna → Salzburg', from: 'vienna', to: 'salzburg' },
      { label: 'Vienna → Linz', from: 'vienna', to: 'linz' },
    ];
  }
}
