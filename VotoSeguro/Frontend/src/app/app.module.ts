import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgChartsModule } from 'ng2-charts';
import { Chart, registerables } from 'chart.js';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CandidateCardComponent } from './components/candidate-card/candidate-card.component';
import { VotingDashboardComponent } from './components/voting-dashboard/voting-dashboard.component';
import { LoginComponent } from './components/public/login/login.component';
import { RegisterComponent } from './components/public/register/register.component';
import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard.component';
import { CandidateManagementComponent } from './components/admin/candidate-management/candidate-management.component';
import { CandidateFormComponent } from './components/admin/candidate-form/candidate-form.component';
import { VoterAuditComponent } from './components/admin/voter-audit/voter-audit.component';

Chart.register(...registerables);

@NgModule({
  declarations: [
    AppComponent,
    CandidateCardComponent,
    VotingDashboardComponent,
    LoginComponent,
    RegisterComponent,
    AdminDashboardComponent,
    CandidateManagementComponent,
    CandidateFormComponent,
    VoterAuditComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    NgChartsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
