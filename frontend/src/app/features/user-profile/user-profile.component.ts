import { Component, OnInit, inject } from '@angular/core';
import {AbstractControl, ValidationErrors, FormControl, ReactiveFormsModule, FormGroup, Validators} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../../core/services/auth.service';

function passwordMatchValidator(group: AbstractControl) : ValidationErrors | null {
  const pw = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return pw && confirm && pw !== confirm ? {mismatch : true} : null; 
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

  showUsernameForm = false;
  showEmailForm = false;
  showPasswordForm = false;

  profileForm = new FormGroup({
  username: new FormControl('', [Validators.required, Validators.minLength(3)]),
  email: new FormControl('', [Validators.required, Validators.email]),
  password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  confirmPassword: new FormControl('', [Validators.required]),
}, { validators: passwordMatchValidator });

totalTours = 0;
totalLogs = 0;

ngOnInit(): void {
  this.username = localStorage.getItem('username') ?? 'N/A';
  this.email = localStorage.getItem('email') ?? 'N/A';
}

// add these getters to your component class
get usernameControl() { return this.profileForm.get('username') as FormControl; }
get emailControl() { return this.profileForm.get('email') as FormControl; }
get passwordControl() { return this.profileForm.get('password') as FormControl; }
get confirmPasswordControl() { return this.profileForm.get('confirmPassword') as FormControl; }

onSave(field: 'username' | 'email' | 'password'): void {
  const control = this.profileForm.get(field);
  if (control?.invalid) return;

  if (field === 'username') {
    this.username = this.profileForm.value.username!;
    localStorage.setItem('username', this.username);
    this.showUsernameForm = false;
  } else if (field === 'email') {
    this.email = this.profileForm.value.email!;
    localStorage.setItem('email', this.email);
    this.showEmailForm = false;
  } else if (field === 'password') {
    if (this.profileForm.errors?.['mismatch']) return;
    console.log('Password change requested');
    this.profileForm.get('password')?.reset();
    this.profileForm.get('confirmPassword')?.reset();
    this.showPasswordForm = false;
  }
}

onCancel(): void {
  window.history.back();
};

}
