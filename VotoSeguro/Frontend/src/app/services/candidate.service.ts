import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Candidate } from '../models/candidate.model';

@Injectable({
    providedIn: 'root'
})
export class CandidateService {
    private apiUrl = 'http://localhost:5000/api/candidates';

    constructor(private http: HttpClient) { }

    private getHeaders() {
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }

    getCandidates(): Observable<Candidate[]> {
        return this.http.get<Candidate[]>(this.apiUrl, { headers: this.getHeaders() });
    }

    createCandidate(candidate: FormData): Observable<any> {
        return this.http.post(this.apiUrl, candidate, { headers: this.getHeaders() });
    }

    updateCandidate(id: number, candidate: FormData): Observable<any> {
        return this.http.put(`${this.apiUrl}/${id}`, candidate, { headers: this.getHeaders() });
    }

    deleteCandidate(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
    }
}
