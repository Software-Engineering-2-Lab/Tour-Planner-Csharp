import { Component, inject, signal, output, ChangeDetectionStrategy } from '@angular/core';
import { RouterModule, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { TourModalComponent } from '../../features/dashboard/tour-modal/tour-modal.component';
import { ExportImportService } from '../../core/services/export-import.service';
import { SearchService } from '../../core/services/search.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, RouterLink, TourModalComponent, FormsModule],
  templateUrl: './navbar.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  private searchService = inject(SearchService);
  private exportImport = inject(ExportImportService);
  isTourModalOpen = signal(false);

  public localSearchTerm = '';
  public onSearchChange(newTerm: string) {
      this.searchService.updateSearch(newTerm);
  }

  toggleSidebar = output<void>();
  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  openAddTour(): void {
    this.isTourModalOpen.set(true);
  }

  closeTourModal(): void {
    this.isTourModalOpen.set(false);
  }

  onImport(event: Event): void {
    const files = (event.target as HTMLInputElement).files;
    if (!files) return;
    const userId = Number(localStorage.getItem('userId'));
    this.exportImport.importTours(files, userId).subscribe({
      next: () => {
        console.log('Import successful');
        (event.target as HTMLInputElement).value = '';
      },
      error: (err) => console.error('Import failed', err),
    });
  }
}
