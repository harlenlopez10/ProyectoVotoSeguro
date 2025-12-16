import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VotingDashboardComponent } from './components/voting-dashboard/voting-dashboard.component';
import { LoginComponent } from './components/public/login/login.component';
import { RegisterComponent } from './components/public/register/register.component';

import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard.component';
import { CandidateManagementComponent } from './components/admin/candidate-management/candidate-management.component';
import { CandidateFormComponent } from './components/admin/candidate-form/candidate-form.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'vote', component: VotingDashboardComponent },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'admin/candidates', component: CandidateManagementComponent },
  { path: 'admin/candidates/new', component: CandidateFormComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
