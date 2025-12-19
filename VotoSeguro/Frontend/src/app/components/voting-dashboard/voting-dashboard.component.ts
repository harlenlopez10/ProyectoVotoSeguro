import { Component, OnInit } from '@angular/core';
import { Candidate } from '../../models/candidate.model';
import { CandidateService } from '../../services/candidate.service';
import { VoteService } from '../../services/vote.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-voting-dashboard',
    templateUrl: './voting-dashboard.component.html',
    styleUrls: ['./voting-dashboard.component.css']
})
export class VotingDashboardComponent implements OnInit {
    candidates: Candidate[] = [];
    hasVoted = false;
    user: any;

    constructor(
        private candidateService: CandidateService,
        private voteService: VoteService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        this.authService.currentUser$.subscribe(user => {
            this.user = user;
            if (user?.hasVoted) {
                this.hasVoted = true;
            }
        });

        this.checkVoteStatus(); // Double check with server
        this.loadCandidates();
    }

    checkVoteStatus() {
        // If we want to be sure
        this.voteService.getMyVote().subscribe({
            next: (vote) => {
                if (vote) this.hasVoted = true;
            },
            error: () => { }
        });
    }

    loadCandidates() {
        this.candidateService.getCandidates().subscribe(candidates => {
            this.candidates = candidates;
        });
    }

    handleVote(candidateId: string) {
        if (this.hasVoted) return;

        if (confirm('¿Estás seguro de tu voto? No podrás cambiarlo.')) {
            this.voteService.vote(candidateId).subscribe({
                next: () => {
                    this.hasVoted = true;
                    this.loadCandidates(); // Refresh counts if needed
                },
                error: (err) => alert('Error al votar')
            });
        }
    }
}
