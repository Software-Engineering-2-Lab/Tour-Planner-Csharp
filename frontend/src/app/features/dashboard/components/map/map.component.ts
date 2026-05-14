import {
  Component,
  OnInit,
  OnDestroy,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MapFacadeService } from '../../../../core/services/map-facade.service';
import { MockRouteService } from '../../../../core/services/mock-route.service';

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss',
})
export class MapComponent implements OnInit, OnDestroy {
  private mapFacade = inject(MapFacadeService);
  private routeService = inject(MockRouteService);

  readonly isLoading = signal(false);
  readonly routeInfo = signal<{ distanceKm: number; durationMin: number } | null>(null);
  readonly availableRoutes = this.routeService.getAvailableRoutes();

  ngOnInit(): void {
    this.mapFacade.initMap('map-container');
  }

  ngOnDestroy(): void {
    this.mapFacade.destroyMap();
  }

  onRouteSelect(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const [from, to] = value.split('-');
    this.loadRoute(from, to);
  }

  private loadRoute(from: string, to: string): void {
    this.isLoading.set(true);
    this.routeInfo.set(null);
    this.mapFacade.clearMarkers();
    this.mapFacade.clearRoute();

    this.routeService.getRoute({ fromName: from, toName: to }).subscribe({
      next: (result) => {
        this.mapFacade.drawRoute(result.coordinates);

        const start = result.coordinates[0];
        const end = result.coordinates[result.coordinates.length - 1];
        this.mapFacade.setMarker(start[0], start[1], `Start: ${from}`);
        this.mapFacade.setMarker(end[0], end[1], `End: ${to}`);

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