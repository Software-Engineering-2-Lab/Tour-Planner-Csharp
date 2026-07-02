import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'highlight',
  standalone: true
})
export class HighlightPipe implements PipeTransform {
  transform(text: string | undefined | null, searchTerm: string | undefined | null): string {
    
    if (!text) {
      return '';
    }

    if (!searchTerm) {
      return text;
    }

    const cleanSearchTerm = searchTerm.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');
    const regex = new RegExp(`(${cleanSearchTerm})`, 'gi');
    return text.replace(regex, `<mark class="search-highlight">$1</mark>`);
  }
}