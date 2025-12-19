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

    // Temporal Trend Chart - Time on X, Votes on Y
    public lineChartOptions: ChartConfiguration['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'x',
        plugins: {
            legend: { display: true, position: 'top' },
            tooltip: { mode: 'index', intersect: false }
        },
        elements: {
            line: { tension: 0.4, borderWidth: 3, fill: 'origin' }
        },
        scales: {
            x: {
                grid: { display: false },
                title: { display: true, text: 'Línea de Tiempo (HH:mm)', color: '#64748b' }
            },
            y: {
                beginAtZero: true,
                title: { display: true, text: 'Votos Acumulados', color: '#64748b' }
            }
        }
    };
    public lineChartType: ChartType = 'line';
    public lineChartData: ChartData<'line'> = {
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
        const colorsPalette = ['#6366f1', '#ec4899', '#8b5cf6', '#f59e0b', '#10b981'];

        // Create a persistent mapping of Candidate Name -> Color
        const colorMap: { [key: string]: string } = {};
        labels.forEach((name, index) => {
            colorMap[name] = colorsPalette[index % colorsPalette.length];
        });

        // Use the mapped colors for Bar and Pie charts
        const chartColors = labels.map(name => colorMap[name]);

        this.barChartData = {
            labels: labels,
            datasets: [{
                data: votes,
                label: 'Votos',
                backgroundColor: chartColors
            }]
        };

        this.pieChartData = {
            labels: labels,
            datasets: [{
                data: votes,
                backgroundColor: chartColors
            }]
        };

        // Temporal Trend Chart (Time Series)
        if (data.trends && data.trends.length > 0) {
            const sortedTrends = [...data.trends].sort((a, b) =>
                new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
            );

            const timeLabels = Array.from(new Set(sortedTrends.map(t =>
                new Date(t.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
            )));

            const candidatesInTrend = Array.from(new Set(data.trends.map(t => t.candidateName)));

            this.lineChartData = {
                labels: timeLabels,
                datasets: candidatesInTrend.map((name) => {
                    const color = colorMap[name] || '#cbd5e1';
                    let currentVal = 0;

                    const dataPoints = timeLabels.map(time => {
                        const match = sortedTrends.find(t =>
                            t.candidateName === name &&
                            new Date(t.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) === time
                        );
                        if (match) currentVal = match.cumulativeVotes;
                        return currentVal;
                    });

                    return {
                        data: dataPoints,
                        label: name,
                        borderColor: color,
                        backgroundColor: color + '15',
                        fill: true,
                        tension: 0.4,
                        pointRadius: 3
                    };
                })
            };
        }
    }

    logout() {
        this.authService.logout();
    }
}
