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
          <div class="header-actions">
            <a routerLink="/admin" class="btn-secondary">Volver al Panel</a>
            <a routerLink="/admin/candidates/new" class="btn-primary">Nuevo Candidato</a>
          </div>
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
                <a [routerLink]="['/admin/candidates/edit', candidate.id]" class="btn-edit">Editar</a>
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
    .btn-secondary { background: #f8fafc; color: #64748b; padding: 10px 20px; border: 1px solid #e2e8f0; border-radius: 6px; text-decoration: none; margin-right: 10px; }
    .btn-edit { background: #f59e0b; color: white; padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; text-decoration: none; margin-right: 5px; font-size: 0.8125rem; }
    .btn-danger { background: #ef4444; color: white; padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; font-size: 0.8125rem; }
    .header-actions { display: flex; align-items: center; }
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

  deleteCandidate(id: string) {
    if (confirm('¿Eliminar candidato?')) {
      this.candidateService.deleteCandidate(id).subscribe(() => this.loadCandidates());
    }
  }
}
