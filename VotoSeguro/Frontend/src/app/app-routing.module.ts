import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VotingDashboardComponent } from './components/voting-dashboard/voting-dashboard.component';
import { LoginComponent } from './components/public/login/login.component';
import { RegisterComponent } from './components/public/register/register.component';
import { AuthGuard } from './guards/auth.guard';


import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard.component';
import { CandidateManagementComponent } from './components/admin/candidate-management/candidate-management.component';
import { CandidateFormComponent } from './components/admin/candidate-form/candidate-form.component';
import { VoterAuditComponent } from './components/admin/voter-audit/voter-audit.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'vote', component: VotingDashboardComponent },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [AuthGuard] },
  { path: 'admin/candidates', component: CandidateManagementComponent, canActivate: [AuthGuard] },
  { path: 'admin/candidates/new', component: CandidateFormComponent, canActivate: [AuthGuard] },
  { path: 'admin/candidates/edit/:id', component: CandidateFormComponent, canActivate: [AuthGuard] },
  { path: 'admin/voters', component: VoterAuditComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
