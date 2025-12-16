import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class VoteService {
    private apiUrl = 'http://localhost:5000/api/votes';

    constructor(private http: HttpClient) { }

    private getHeaders() {
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }

    vote(candidateId: number): Observable<any> {
        return this.http.post(this.apiUrl, { candidateId }, { headers: this.getHeaders() });
    }

    getMyVote(): Observable<any> {
        return this.http.get(`${this.apiUrl}/my-vote`, { headers: this.getHeaders() });
    }
}
