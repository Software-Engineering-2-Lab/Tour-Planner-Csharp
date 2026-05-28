import {Injectable, inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({providedIn:'root'})
export class ExportImportService {
    private http = inject(HttpClient);
    private readonly API = 'http://localhost:8080/api/tours';

    exportTour(tour:any):void {
        const json = JSON.stringify(tour,null,2);
        const blob = new Blob([json], {type : 'application/json'});
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `tour-${tour.name}.json`;
        a.click();
        URL.revokeObjectURL(url);
    }

  importTours(files: FileList, userId: number): void {
    Array.from(files).forEach(file => {
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          const data = JSON.parse(e.target?.result as string);
          const tours = Array.isArray(data) ? data : [data];
          const toImport = tours.map(tour => ({
            ...tour,
            id: null,        
            userId: userId  
          }));
          this.http.post(`${this.API}/import`, toImport).subscribe({
            next: () => console.log('Import successful'),
            error: () => console.error('Import failed')
          });
        } catch {
          console.error('Invalid JSON file');
        }
      };
      reader.readAsText(file);
    });
  }
    
}
