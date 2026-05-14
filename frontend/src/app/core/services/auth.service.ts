import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { TourService } from './tour.service';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:8080/api/auth';
    private tourService = inject(TourService);
    private router = inject(Router);

    private _isLoggedIn = signal<boolean>(!!localStorage.getItem('token'));
    isLoggedIn = this._isLoggedIn.asReadonly();

    constructor(private http: HttpClient) {}

    login(credentials: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/login`, credentials);
    }

    register(userData: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, userData);
    }

    getToken(): string | null {
        return localStorage.getItem('jwt_token');
    }

    saveToken(token: string) {
    localStorage.setItem('token', token);
    this._isLoggedIn.set(true); // Label: Notify the app immediately
}

    logout() {
        localStorage.clear();
        this.tourService.clearData(); 
        this.router.navigate(['/login']); 
        this._isLoggedIn.set(false); 
    }

    getUserId(): number {
        const id = localStorage.getItem('userId');
        return id ? parseInt(id, 10) : 0;
    }
}