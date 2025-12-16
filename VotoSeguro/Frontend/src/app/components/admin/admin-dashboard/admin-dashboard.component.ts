import { Component, OnInit } from '@angular/core';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { CandidateService } from '../../../services/candidate.service';
import { Candidate } from '../../../models/candidate.model';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './admin-dashboard.component.html',
    styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
    totalVoters = 120; // Mock
    votesCast = 45;
    participation = 0;

    candidates: Candidate[] = [];

    // Bar Chart
    public barChartOptions: ChartConfiguration['options'] = {
        responsive: true,
    };
    public barChartType: ChartType = 'bar';
    public barChartData: ChartData<'bar'> = {
        labels: [],
        datasets: [
            { data: [], label: 'Votos' }
        ]
    };

    // Pie Chart
    public pieChartOptions: ChartConfiguration['options'] = {
        responsive: true,
    };
    public pieChartType: ChartType = 'pie';
    public pieChartData: ChartData<'pie'> = {
        labels: [],
        datasets: [{ data: [] }]
    };

    constructor(private candidateService: CandidateService) { }

    ngOnInit(): void {
        this.calculateStats();
        this.loadData();
    }

    calculateStats() {
        this.participation = Math.round((this.votesCast / this.totalVoters) * 100);
    }

    loadData() {
        // Mock data for now, or fetch from service
        // In real app: this.candidateService.getCandidates().subscribe(...)
        this.candidates = [
            { id: 1, name: 'Elena Rodríguez', party: 'Verde', description: '', imageUrl: '', votes: 20 },
            { id: 2, name: 'Carlos Mendoza', party: 'Progreso', description: '', imageUrl: '', votes: 15 },
            { id: 3, name: 'Sofía Almirante', party: 'Ciudadano', description: '', imageUrl: '', votes: 10 }
        ];

        this.updateCharts();
    }

    updateCharts() {
        const labels = this.candidates.map(c => c.name);
        const data = this.candidates.map(c => c.votes);

        this.barChartData = {
            labels: labels,
            datasets: [{ data: data, label: 'Votos', backgroundColor: ['#6366f1', '#ec4899', '#8b5cf6'] }]
        };

        this.pieChartData = {
            labels: labels,
            datasets: [{ data: data, backgroundColor: ['#6366f1', '#ec4899', '#8b5cf6'] }]
        };
    }
}
