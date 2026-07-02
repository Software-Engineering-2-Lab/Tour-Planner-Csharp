import { Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root' // one global instance
})
export class SearchService {
    public searchTerm = signal<string>('');


    public updateSearch(term: string) {
        this.searchTerm.set(term);
    }
}