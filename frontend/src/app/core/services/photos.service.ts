import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface TourImageDto {
    id: number;
    fileName: string;
    url: string;
    tourId: number;
    createdAt: string;
}

@Injectable({
    providedIn: 'root'
})
export class PhotoService {
    private baseUrl = environment.apiUrl + '/tours';

    constructor(private http: HttpClient) {}

    uploadPhoto(tourId: number, file: File): Observable<TourImageDto> {
        const formData = new FormData();
        formData.append('file', file);

        const token = localStorage.getItem('token');
        let headers = new HttpHeaders();
        
        if (token) {
            headers = headers.set('Authorization', `Bearer ${token}`);
        }

        return this.http.post<TourImageDto>(`${this.baseUrl}/${tourId}/photos`, formData, { headers });
    }

    getPhotosForTour(tourId: number): Observable<TourImageDto[]> {
        const token = localStorage.getItem('token');
        let headers = new HttpHeaders();
        if (token) {
            headers = headers.set('Authorization', `Bearer ${token}`);
        }
        return this.http.get<TourImageDto[]>(`${this.baseUrl}/${tourId}/photos`, { headers });
    }
}