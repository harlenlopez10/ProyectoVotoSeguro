import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Candidate } from '../../models/candidate.model';

@Component({
    selector: 'app-candidate-card',
    templateUrl: './candidate-card.component.html',
    styleUrls: ['./candidate-card.component.css']
})
export class CandidateCardComponent {
    @Input() candidate!: Candidate;
    @Output() vote = new EventEmitter<number>();

    onVote() {
        this.vote.emit(this.candidate.id);
    }
}
