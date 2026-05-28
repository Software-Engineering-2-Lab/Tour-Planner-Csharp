import { Component, OnInit, inject } from '@angular/core';
import { AbstractControl, ValidationErrors, FormControl, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

function passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const pw = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pw && confirm && pw !== confirm ? { mismatch: true } : null;
}

@Component({
    selector: 'app-user-profile',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './user-profile.component.html',
    styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent implements OnInit {
    private authService = inject(AuthService);

    username = '';
    email = '';
    totalTours = 0;
    totalLogs = 0;

    showUsernameForm = false;
    showEmailForm = false;
    showPasswordForm = false;

    profileForm = new FormGroup({
        username: new FormControl('', [Validators.required, Validators.minLength(3)]),
        email: new FormControl('', [Validators.required, Validators.email]),
        password: new FormControl('', [Validators.required, Validators.minLength(6)]),
        confirmPassword: new FormControl('', [Validators.required]),
    }, { validators: passwordMatchValidator });

    get usernameControl() { return this.profileForm.get('username') as FormControl; }
    get emailControl() { return this.profileForm.get('email') as FormControl; }
    get passwordControl() { return this.profileForm.get('password') as FormControl; }
    get confirmPasswordControl() { return this.profileForm.get('confirmPassword') as FormControl; }

    ngOnInit(): void {
        this.authService.getProfile().subscribe({
            next: (user) => {
                this.username = user.username;
                this.email = user.email;
                this.totalTours = user.totalTours ?? 0;
                localStorage.setItem('username', user.username);
                localStorage.setItem('email', user.email);
            },
            error: () => {
                this.username = localStorage.getItem('username') ?? 'N/A';
                this.email = localStorage.getItem('email') ?? 'N/A';
            }
        });
    }

    onSave(field: 'username' | 'email' | 'password'): void {
        const control = this.profileForm.get(field);
        if (control?.invalid) return;
        if (field === 'password' && this.profileForm.errors?.['mismatch']) return;

        const payload: any = {};
        if (field === 'username') payload.username = this.usernameControl.value;
        if (field === 'email') payload.email = this.emailControl.value;
        if (field === 'password') payload.password = this.passwordControl.value;

        this.authService.updateProfile(payload).subscribe({
            next: (user) => {
                this.username = user.username;
                this.email = user.email;
                this.totalTours = user.totalTours ?? 0;
                localStorage.setItem('username', user.username);
                localStorage.setItem('email', user.email);

                if (field === 'username') this.showUsernameForm = false;
                if (field === 'email') this.showEmailForm = false;
                if (field === 'password') {
                    this.passwordControl.reset();
                    this.confirmPasswordControl.reset();
                    this.showPasswordForm = false;
                }
            },
            error: (err) => console.error('Update failed', err)
        });
    }

    onCancel(): void {
        window.history.back();
    }
}