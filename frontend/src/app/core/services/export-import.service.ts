import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { TourService } from './tour.service';

@Injectable({ providedIn: 'root' })
export class ExportImportService {
    private http = inject(HttpClient);
    private tourService = inject(TourService);
    private readonly API = 'http://localhost:8080/api/tours';

    exportTour(tour: any): void {
        const json = JSON.stringify(tour, null, 2);
        const blob = new Blob([json], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `tour-${tour.name}.json`;
        a.click();
        URL.revokeObjectURL(url);
    }

    importTours(files: FileList, userId: number): Observable<any> {
        const fileArray = Array.from(files);

        const fileReads$ = fileArray.map(file =>
            new Observable<any[]>(observer => {
                const reader = new FileReader();
                reader.onload = (e) => {
                    try {
                        const data = JSON.parse(e.target?.result as string);
                        const tours = Array.isArray(data) ? data : [data];
                        const toImport = tours.map(({ tourImages, ...rest }: any) => ({
                            ...rest,
                            id: 0,
                            userId: userId
                        }));
                        observer.next(toImport);
                        observer.complete();
                    } catch {
                        observer.error('Invalid JSON file');
                    }
                };
                reader.readAsText(file);
            })
        );

        return new Observable(observer => {
            forkJoin(fileReads$).subscribe({
                next: (allTourArrays) => {
                    const merged = allTourArrays.flat();
                    this.http.post(`${this.API}/import`, merged).subscribe({
                        next: (res) => {
                            this.tourService.loadTours();
                            observer.next(res);
                            observer.complete();
                        },
                        error: (err) => observer.error(err)
                    });
                },
                error: (err) => observer.error(err)
            });
        });
    }
}