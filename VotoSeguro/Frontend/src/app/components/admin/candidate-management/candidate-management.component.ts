import { Component, OnInit } from '@angular/core';
import { CandidateService } from '../../../services/candidate.service';
import { Candidate } from '../../../models/candidate.model';

@Component({
    selector: 'app-candidate-management',
    template: `
    <div class="admin-container">
      <div class="main-content">
        <header>
          <h1>Gestión de Candidatos</h1>
          <a routerLink="/admin/candidates/new" class="btn-primary">Nuevo Candidato</a>
        </header>

        <table class="candidate-table">
          <thead>
            <tr>
              <th>Foto</th>
              <th>Nombre</th>
              <th>Partido</th>
              <th>Votos</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let candidate of candidates">
              <td><img [src]="candidate.imageUrl" alt="Foto" width="50"></td>
              <td>{{ candidate.name }}</td>
              <td>{{ candidate.party }}</td>
              <td>{{ candidate.votes }}</td>
              <td>
                <button (click)="deleteCandidate(candidate.id)" class="btn-danger">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
    styles: [`
    .admin-container { padding: 20px; }
    header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .candidate-table { width: 100%; border-collapse: collapse; margin-top: 20px; }
    th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
    .btn-primary { background: #6366f1; color: white; padding: 10px 20px; border-radius: 6px; text-decoration: none; }
    .btn-danger { background: #ef4444; color: white; padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; }
  `]
})
export class CandidateManagementComponent implements OnInit {
    candidates: Candidate[] = [];

    constructor(private candidateService: CandidateService) { }

    ngOnInit() {
        this.loadCandidates();
    }

    loadCandidates() {
        this.candidateService.getCandidates().subscribe(data => this.candidates = data);
    }

    deleteCandidate(id: number) {
        if (confirm('¿Eliminar candidato?')) {
            this.candidateService.deleteCandidate(id).subscribe(() => this.loadCandidates());
        }
    }
}
