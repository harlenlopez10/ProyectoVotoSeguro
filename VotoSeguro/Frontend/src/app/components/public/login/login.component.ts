import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    loginForm: FormGroup;
    error = '';
    loading = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.loginForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required]
        });
    }

    onSubmit() {
        if (this.loginForm.invalid) return;

        this.loading = true;
        this.authService.login(this.loginForm.value).subscribe({
            next: () => {
                if (this.authService.isAdmin()) {
                    this.router.navigate(['/admin']);
                } else {
                    this.router.navigate(['/vote']);
                }
            },
            error: (err) => {
                this.error = 'Credenciales inválidas o error de conexión';
                this.loading = false;
                console.error(err);
            }
        });
    }
}
