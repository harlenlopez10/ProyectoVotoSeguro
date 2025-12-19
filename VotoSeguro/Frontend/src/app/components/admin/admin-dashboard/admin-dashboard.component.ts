import { Component, OnInit } from '@angular/core';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { ReportService, VoteStatistics } from '../../../services/report.service';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './admin-dashboard.component.html',
    styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
    totalVoters = 0;
    votesCast = 0;
    participation = 0;
    stats?: VoteStatistics;

    // Bar Chart
    public barChartOptions: ChartConfiguration['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: true } }
    };
    public barChartType: ChartType = 'bar';
    public barChartData: ChartData<'bar'> = {
        labels: [],
        datasets: [{ data: [], label: 'Votos', backgroundColor: '#6366f1' }]
    };

    // Pie Chart
    public pieChartOptions: ChartConfiguration['options'] = {
        responsive: true,
        maintainAspectRatio: false,
    };
    public pieChartType: ChartType = 'pie';
    public pieChartData: ChartData<'pie'> = {
        labels: [],
        datasets: [{ data: [], backgroundColor: ['#6366f1', '#ec4899', '#8b5cf6', '#f59e0b', '#10b981'] }]
    };

    // Simplified Chart - Total Votes per Candidate
    public lineChartOptions: ChartConfiguration['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y', // Horizontal bars
        plugins: {
            legend: { display: false }
        },
        scales: {
            x: {
                beginAtZero: true,
                title: { display: true, text: 'Cantidad de Votos', color: '#64748b' }
            },
            y: {
                grid: { display: false }
            }
        }
    };
    public lineChartType: ChartType = 'bar';
    public lineChartData: ChartData<'bar'> = {
        labels: [],
        datasets: []
    };

    constructor(
        private reportService: ReportService,
        private authService: AuthService
    ) { }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.reportService.getStatistics().subscribe({
            next: (data) => {
                this.stats = data;
                this.totalVoters = data.totalVoters;
                this.votesCast = data.totalVotes;
                this.participation = data.participationPercentage;
                this.updateCharts(data);
            },
            error: (err) => console.error('Error loading statistics', err)
        });
    }

    updateCharts(data: VoteStatistics) {
        const labels = data.candidateResults.map(c => c.name);
        const votes = data.candidateResults.map(c => c.votes);
        const colors = ['#6366f1', '#ec4899', '#8b5cf6', '#f59e0b', '#10b981'];

        this.barChartData = {
            labels: labels,
            datasets: [{
                data: votes,
                label: 'Votos',
                backgroundColor: colors
            }]
        };

        this.pieChartData = {
            labels: labels,
            datasets: [{
                data: votes,
                backgroundColor: colors
            }]
        };

        // Result Summary Chart (Simple Horizontal Bar)
        this.lineChartData = {
            labels: labels,
            datasets: [{
                data: votes,
                label: 'Votos Totales',
                backgroundColor: colors,
                borderRadius: 4
            }]
        };
    }

    logout() {
        this.authService.logout();
    }
}
