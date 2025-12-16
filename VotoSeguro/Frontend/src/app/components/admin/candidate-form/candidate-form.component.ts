import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CandidateService } from '../../../services/candidate.service';

@Component({
    selector: 'app-candidate-form',
    template: `
    <div class="admin-container">
      <h2>Registrar Candidato</h2>
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
        
        <button type="submit" [disabled]="form.invalid">Guardar</button>
      </form>
    </div>
  `,
    styles: [`
    .admin-container { padding: 20px; max-width: 600px; margin: 0 auto; }
    form { display: flex; flex-direction: column; gap: 15px; }
    input, textarea { width: 100%; padding: 8px; margin-top: 5px; }
    button { background: #6366f1; color: white; padding: 10px; border: none; cursor: pointer; }
  `]
})
export class CandidateFormComponent {
    form: FormGroup;

    constructor(
        private fb: FormBuilder,
        private candidateService: CandidateService,
        private router: Router
    ) {
        this.form = this.fb.group({
            name: ['', Validators.required],
            party: ['', Validators.required],
            description: [''],
            imageUrl: ['']
        });
    }

    onSubmit() {
        if (this.form.valid) {
            // In a real scenario with file upload, we'd use FormData. 
            // APIs usually expect FormData if uploading files, or JSON if just URLs.
            // My service uses FormData. Let's convert or adjust service.
            const formData = new FormData();
            Object.keys(this.form.value).forEach(key => {
                formData.append(key, this.form.value[key]);
            });

            this.candidateService.createCandidate(formData).subscribe(() => {
                this.router.navigate(['/admin/candidates']);
            });
        }
    }
}
