import { Injectable } from '@angular/core';
import * as L from 'leaflet';

export interface RouteResult {
  coordinates: [number, number][];
  distanceKm: number;
  durationMin: number;
}

const DOT_ICON = L.divIcon({
  className: '',
  html: '<div style="width:10px;height:10px;border-radius:50%;background:#3b82f6;border:2px solid white;box-shadow:0 1px 4px rgba(0,0,0,0.4)"></div>',
  iconSize: [10, 10],
  iconAnchor: [5, 5],
  popupAnchor: [0, -8],
});

@Injectable({
  providedIn: 'root',
})
export class MapFacadeService {
  private map: L.Map | null = null;
  private routeLayer: L.Polyline | null = null;
  private markers: L.Marker[] = [];

  initMap(containerId: string): void {
    if (this.map) return;

    this.map = L.map(containerId, {
      zoomControl: true,
      attributionControl: true,
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(this.map);

    this.map.setView([48.2082, 16.3738], 12);
  }

  setCenter(lat: number, lng: number, zoom = 13): void {
    this.map?.setView([lat, lng], zoom);
  }

  setMarker(lat: number, lng: number, label?: string): void {
    if (!this.map) return;
    const marker = L.marker([lat, lng], { icon: DOT_ICON });
    if (label) marker.bindPopup(label);
    marker.addTo(this.map);
    this.markers.push(marker);
  }

  clearMarkers(): void {
    this.markers.forEach((m) => m.remove());
    this.markers = [];
  }

  drawRoute(coordinates: [number, number][]): void {
    if (!this.map) return;
    this.routeLayer?.remove();
    this.routeLayer = L.polyline(coordinates, {
      color: '#3b82f6',
      weight: 4,
    }).addTo(this.map);
    this.map.fitBounds(this.routeLayer.getBounds(), { padding: [32, 32] });
  }

  clearRoute(): void {
    this.routeLayer?.remove();
    this.routeLayer = null;
  }

  destroyMap(): void {
    this.map?.remove();
    this.map = null;
    this.markers = [];
    this.routeLayer = null;
  }
}