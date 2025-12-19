import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Candidate } from '../models/candidate.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CandidateService {
    private apiUrl = `${environment.apiUrl}/candidates`;

    constructor(private http: HttpClient) { }

    private getHeaders() {
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }

    getCandidates(): Observable<Candidate[]> {
        // Public endpoint, map backend fields to frontend model
        return this.http.get<any[]>(this.apiUrl).pipe(
            map(candidates => candidates.map(c => ({
                id: c.id,
                name: c.name,
                party: c.party,
                description: c.description || '',
                imageUrl: c.photoUrl || c.logoUrl || '',
                votes: c.votes ?? c.voteCount ?? 0
            } as Candidate)))
        );
    }

    createCandidate(candidate: any): Observable<any> {
        return this.http.post(this.apiUrl, candidate, { headers: this.getHeaders() });
    }

    updateCandidate(id: string, candidate: any): Observable<any> {
        return this.http.put(`${this.apiUrl}/${id}`, candidate, { headers: this.getHeaders() });
    }

    deleteCandidate(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
    }
}
