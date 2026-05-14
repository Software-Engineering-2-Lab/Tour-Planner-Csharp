import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { RouteResult } from './map-facade.service';

export interface RouteRequest {
  fromName: string;
  toName: string;
}

// ---------------------------------------------------------------------------
// Hardcoded mock routes – simulates what the backend/ORS would return.
// When the real backend is ready, only THIS service changes; the component
// and MapFacadeService stay untouched (facade pattern pays off here).
// ---------------------------------------------------------------------------
const MOCK_ROUTES: Record<string, RouteResult> = {
  'vienna-graz': {
    distanceKm: 195,
    durationMin: 120,
    coordinates: [
      [48.2082, 16.3738], // Vienna
      [47.8, 15.9],
      [47.5, 15.4],
      [47.0707, 15.4395], // Graz
    ],
  },
  'vienna-salzburg': {
    distanceKm: 297,
    durationMin: 180,
    coordinates: [
      [48.2082, 16.3738], // Vienna
      [48.1, 15.6],
      [47.9, 14.5],
      [47.8, 13.8],
      [47.7997, 13.0446], // Salzburg
    ],
  },
  'vienna-linz': {
    distanceKm: 187,
    durationMin: 110,
    coordinates: [
      [48.2082, 16.3738], // Vienna
      [48.2, 15.6],
      [48.25, 14.8],
      [48.3058, 14.2865], // Linz
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
  /**
   * Simulates an async route request.
   * Replace the body with a real HttpClient call once the backend exists.
   */
  getRoute(request: RouteRequest): Observable<RouteResult> {
    const key = `${request.fromName.toLowerCase()}-${request.toName.toLowerCase()}`;
    const result = MOCK_ROUTES[key] ?? FALLBACK_ROUTE;

    // Simulate ~400 ms network latency so the UI can show a loading state
    return of(result).pipe(delay(400));
  }

  /** Returns all available mock destinations for a dropdown / demo */
  getAvailableRoutes(): { label: string; from: string; to: string }[] {
    return [
      { label: 'Vienna → Graz', from: 'vienna', to: 'graz' },
      { label: 'Vienna → Salzburg', from: 'vienna', to: 'salzburg' },
      { label: 'Vienna → Linz', from: 'vienna', to: 'linz' },
    ];
  }
}
