import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['../login/login.component.css'] // Reuse login styles
})
export class RegisterComponent {
    registerForm: FormGroup;
    error = '';
    loading = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.registerForm = this.fb.group({
            nombre: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    onSubmit() {
        if (this.registerForm.invalid) return;

        this.loading = true;
        this.authService.register(this.registerForm.value).subscribe({
            next: () => {
                // Auto login or redirect to login
                this.router.navigate(['/login']);
            },
            error: (err) => {
                this.error = 'Error al registrar. Intenta nuevamente.';
                this.loading = false;
                console.error(err);
            }
        });
    }
}
