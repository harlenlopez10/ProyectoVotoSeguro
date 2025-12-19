import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface CandidateResult {
    id: string;
    name: string;
    party: string;
    photoUrl: string;
    logoUrl: string;
    votes: number;
    percentage: number;
}

export interface VoteTrend {
    timestamp: string;
    candidateName: string;
    cumulativeVotes: number;
}

export interface VoteStatistics {
    totalVoters: number;
    totalVotes: number;
    participationPercentage: number;
    candidateResults: CandidateResult[];
    trends: VoteTrend[];
}

@Injectable({
    providedIn: 'root'
})
export class ReportService {
    private apiUrl = `${environment.apiUrl}/reports`;

    constructor(private http: HttpClient) { }

    private getHeaders() {
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }

    getStatistics(): Observable<VoteStatistics> {
        return this.http.get<VoteStatistics>(`${this.apiUrl}/statistics`, { headers: this.getHeaders() });
    }
}
