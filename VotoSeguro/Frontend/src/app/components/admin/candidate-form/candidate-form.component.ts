import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CandidateService } from '../../../services/candidate.service';

@Component({
  selector: 'app-candidate-form',
  template: `
    <div class="admin-container">
      <h2>{{ isEditMode ? 'Editar Candidato' : 'Registrar Candidato' }}</h2>
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <div>
          <label>Nombre</label>
          <input formControlName="name" placeholder="Nombre completo">
        </div>
        <div>
          <label>Partido</label>
          <input formControlName="party" placeholder="Partido político">
        </div>
        <div>
          <label>Descripción</label>
          <textarea formControlName="description"></textarea>
        </div>
        <div>
            <label>URL Foto (Temporal)</label>
            <input formControlName="imageUrl">
        </div>
        <!-- File upload handling would go here for real Storage -->
        
        <div class="form-actions">
          <button type="button" (click)="onCancel()" class="btn-cancel">Cancelar</button>
          <button type="submit" [disabled]="form.invalid" class="btn-save">
            {{ isEditMode ? 'Actualizar' : 'Guardar' }} Candidato
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .admin-container { padding: 20px; max-width: 600px; margin: 0 auto; }
    form { display: flex; flex-direction: column; gap: 15px; }
    input, textarea { width: 100%; padding: 12px; margin-top: 5px; border: 1px solid #e2e8f0; border-radius: 6px; }
    .form-actions { display: flex; gap: 15px; margin-top: 10px; }
    button { padding: 12px 20px; border: none; border-radius: 6px; cursor: pointer; font-weight: 600; flex: 1; }
    .btn-save { background: #6366f1; color: white; }
    .btn-save:disabled { background: #94a3b8; cursor: not-allowed; }
    .btn-cancel { background: #f1f5f9; color: #475569; }
  `]
})
export class CandidateFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  candidateId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private candidateService: CandidateService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      party: ['', Validators.required],
      description: [''],
      imageUrl: ['']
    });
  }

  ngOnInit() {
    this.candidateId = this.route.snapshot.paramMap.get('id');
    if (this.candidateId) {
      this.isEditMode = true;
      this.loadCandidate();
    }
  }

  loadCandidate() {
    this.candidateService.getCandidates().subscribe(candidates => {
      const candidate = candidates.find(c => c.id === this.candidateId);
      if (candidate) {
        this.form.patchValue({
          name: candidate.name,
          party: candidate.party,
          description: candidate.description,
          imageUrl: candidate.imageUrl
        });
      }
    });
  }

  onCancel() {
    this.router.navigate(['/admin/candidates']);
  }

  onSubmit() {
    if (this.form.valid) {
      if (this.isEditMode && this.candidateId) {
        this.candidateService.updateCandidate(this.candidateId, this.form.value).subscribe(() => {
          this.router.navigate(['/admin/candidates']);
        });
      } else {
        this.candidateService.createCandidate(this.form.value).subscribe(() => {
          this.router.navigate(['/admin/candidates']);
        });
      }
    }
  }
}
