import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment';

export interface User {
    id: string;
    email: string;
    role: 'admin' | 'voter';
    hasVoted?: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = `${environment.apiUrl}/auth`;
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient, private router: Router) {
        this.loadUserFromToken();
    }

    // Expose current user for other services/guards
    getCurrentUser() {
        return this.currentUserSubject.value;
    }

    private loadUserFromToken() {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded: any = jwtDecode(token);
                // Map claims to User object. Adjust claim names based on your JWT structure
                const user: User = {
                    id: decoded.sub || decoded.uid,
                    email: decoded.email,
                    role: decoded.role || 'voter',
                    hasVoted: decoded.hasVoted === 'True' || decoded.hasVoted === true
                };
                this.currentUserSubject.next(user);
            } catch (e) {
                this.logout();
            }
        }
    }

    login(credentials: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
            tap((response: any) => {
                localStorage.setItem('token', response.token);
                this.loadUserFromToken();
            })
        );
    }

    register(userData: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, userData);
    }

    logout() {
        localStorage.removeItem('token');
        this.currentUserSubject.next(null);
        this.router.navigate(['/login']);
    }

    isAuthenticated(): boolean {
        return !!this.currentUserSubject.value;
    }

    isAdmin(): boolean {
        return this.currentUserSubject.value?.role === 'admin';
    }
}
