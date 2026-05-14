import { Component, signal } from '@angular/core';
import { RouterModule, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { TourModalComponent } from '../../../features/dashboard/components/modals/tour-modal/tour-modal.component';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports : [RouterModule, RouterLink, TourModalComponent],
    templateUrl: './navbar.component.html',
    styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

    isTourModalOpen = signal(false); 

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

}