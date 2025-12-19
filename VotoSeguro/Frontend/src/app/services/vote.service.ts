import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class VoteService {
    private apiUrl = `${environment.apiUrl}/votes`;

    constructor(private http: HttpClient) { }

    private getHeaders() {
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }

    vote(candidateId: string): Observable<any> {
        return this.http.post(this.apiUrl, { candidateId }, { headers: this.getHeaders() });
    }

    getMyVote(): Observable<any> {
        return this.http.get(`${this.apiUrl}/status`, { headers: this.getHeaders() });
    }
}
