import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateTourDto, Tour } from '../models/tour.model';
import { TourLog } from '../models/tour-log.model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TourService {
    private http = inject(HttpClient);
    private readonly API_URL = environment.apiUrl; 

    tours = signal<Tour[]>([]);
    logs = signal<TourLog[]>([]);
    selectedTour = signal<Tour | null>(null);

    selectedTourLogs = computed(() => this.logs());

    constructor() {
        this.loadTours();
    }

    loadTours(): void {
        const userId = localStorage.getItem('userId');
    
        if (!userId) {
            this.tours.set([]);
            return;
        }

        this.http.get<Tour[]>(`${this.API_URL}/tours/user/${userId}`).subscribe({
            next: (data) => {
                this.tours.set(data);
                if (data.length > 0 && !this.selectedTour()) {
                    this.selectTour(data[0]);
                }
            },
            error: (err) => {
                console.error('Failed to load tours. The global endpoint is now disabled.', err);
                this.tours.set([]);
            }
        });
    }

    selectTour(tour: Tour | null): void {
        this.selectedTour.set(tour);
        if (tour) {
            this.loadLogsForTour(tour.id);
        } else {
            this.logs.set([]);
        }
    }

    addTour(tourData: CreateTourDto): void {
        const userId = localStorage.getItem('userId');
        const payload = { 
            ...tourData, 
            userId: userId ? Number(userId) : null 
        };

        this.http.post<Tour>(`${this.API_URL}/tours`, payload).subscribe({
            next: (savedTour) => {
                this.tours.update(current => [...current, savedTour]);
                this.selectTour(savedTour);
            },
            error: (err) => console.error('Error creating tour:', err)
        });
    }

    updateTour(updatedTour: Tour): void {
        this.http.put<Tour>(`${this.API_URL}/tours/${updatedTour.id}`, updatedTour).subscribe(res => {
            this.tours.update(current => current.map(t => t.id === res.id ? res : t));
            if (this.selectedTour()?.id === res.id) {
                this.selectedTour.set(res);
            }
        });
    }

    deleteTour(tourId: number): void {
        this.http.delete(`${this.API_URL}/tours/${tourId}`).subscribe(() => {
            this.tours.update(current => current.filter(t => t.id !== tourId));
            if (this.selectedTour()?.id === tourId) {
                this.selectedTour.set(null);
                this.logs.set([]);
            }
        });
    }

    loadLogsForTour(tourId: number): void {
        this.http.get<TourLog[]>(`${this.API_URL}/tours/${tourId}/logs`).subscribe(data => {
            this.logs.set(data);
        });
    }

    addLog(log: TourLog): Observable<TourLog> {
        return this.http.post<TourLog>(`${this.API_URL}/tours/${log.tourId}/logs`, log);
    }

    updateLog(updatedLog: TourLog): Observable<TourLog> {
        return this.http.put<TourLog>(`${this.API_URL}/tours/${updatedLog.tourId}/logs/${updatedLog.id}`, updatedLog);
    }

    deleteLog(logId: number, tourId: number): void {
        this.http.delete(`${this.API_URL}/tours/${tourId}/logs/${logId}`).subscribe(() => {
            this.logs.update(current => current.filter(l => l.id !== logId));
        });
    }

    getToursByUserId(userId: number): Observable<Tour[]> {
        return this.http.get<Tour[]>(`${this.API_URL}/user/${userId}`);
    }

    clearData(): void {
        this.tours.set([]);
        this.selectedTour.set(null);
        this.logs.set([]);
    }
}