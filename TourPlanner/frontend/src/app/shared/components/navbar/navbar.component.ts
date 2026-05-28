import { Component, inject, signal, output } from '@angular/core';
import { RouterModule, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { TourModalComponent } from '../../../features/dashboard/components/modals/tour-modal/tour-modal.component';
import {ExportImportService} from '../../../core/services/export-import.service'

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports : [RouterModule, RouterLink, TourModalComponent],
    templateUrl: './navbar.component.html',
    styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
    private exportImport = inject(ExportImportService);
    isTourModalOpen = signal(false); 

    toggleSidebar = output<void>()
    constructor(
        private authService: AuthService,
        private router: Router
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

    onImport(event : Event):void {
        const files = (event.target as HTMLInputElement).files;
        if (!files) return;
        const userId = Number(localStorage.getItem('userId'));
        console.log('userId:', userId);
        this.exportImport.importTours(files, userId);
    } 


}