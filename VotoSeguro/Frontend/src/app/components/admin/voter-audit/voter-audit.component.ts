import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-voter-audit',
  template: `
    <div class="admin-container">
      <div class="main-content">
        <header>
          <h1>Auditoría de Votantes</h1>
          <a routerLink="/admin" class="btn-secondary">Volver al Panel</a>
        </header>

        <div class="filter-card">
          <input type="text" [(ngModel)]="searchTerm" placeholder="Buscar por nombre o email..." class="search-input">
        </div>

        <table class="audit-table">
          <thead>
            <tr>
              <th>Nombre Completo</th>
              <th>Email</th>
              <th>Estado</th>
              <th>Candidato</th>
              <th>Fecha/Hora</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let voter of filteredVoters()">
              <td>{{ voter.fullName }}</td>
              <td>{{ voter.email }}</td>
              <td>
                <span [class]="voter.hasVoted ? 'badge-success' : 'badge-pending'">
                  {{ voter.hasVoted ? 'Votó' : 'Pendiente' }}
                </span>
              </td>
              <td>{{ voter.votedForName || '---' }}</td>
              <td>{{ voter.voteTimestamp | date:'short' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .admin-container { padding: 20px; }
    header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .btn-secondary { background: #f8fafc; color: #64748b; padding: 10px 20px; border: 1px solid #e2e8f0; border-radius: 6px; text-decoration: none; }
    .search-input { width: 100%; padding: 10px; margin-bottom: 20px; border: 1px solid #ddd; border-radius: 8px; }
    .audit-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1); }
    td { padding: 15px; text-align: left; border-bottom: 1px solid #edf2f7; color: #1e293b; }
    th { background: #f8fafc; font-weight: 600; color: #334155; }
    .badge-success { background: #dcfce7; color: #166534; padding: 4px 12px; border-radius: 9999px; font-size: 0.875rem; }
    .badge-pending { background: #fef3c7; color: #92400e; padding: 4px 12px; border-radius: 9999px; font-size: 0.875rem; }
  `]
})
export class VoterAuditComponent implements OnInit {
  voters: any[] = [];
  searchTerm: string = '';

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.loadVoters();
  }

  private getHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  loadVoters() {
    this.http.get<any[]>(`${environment.apiUrl}/users`, { headers: this.getHeaders() })
      .subscribe(data => this.voters = data);
  }

  filteredVoters() {
    return this.voters.filter(v =>
      v.fullName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      v.email.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
