# VotoSeguro Frontend

Frontend application for the VotoSeguro voting system, built with Angular 16.

## Structure

### Components
- **Public**:
  - `LoginComponent`: User authentication.
  - `RegisterComponent`: Voter registration.
- **Admin**:
  - `AdminDashboardComponent`: Real-time statistics and charts (Bar/Pie) of voting results.
  - `CandidateManagementComponent`: List of candidates with delete option.
  - `CandidateFormComponent`: Create/Edit candidates.
- **Voter**:
  - `VotingDashboardComponent`: Main interface for voters to view candidates and cast a secure vote.
  - `CandidateCardComponent`: Reusable card displaying candidate info.

### Services
- `AuthService`: Manages JWT tokens, login, register, and role helpers (`isAdmin`).
- `CandidateService`: CRUD operations for candidates.
- `VoteService`: Handles voting transaction and status checking.

## Setup

1. Make sure the backend API is running on `http://localhost:5000`.
2. Install dependencies (if not already):
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm start
   ```
4. Navigate to `http://localhost:4200`.

## Features Implemented
- **Authentication**: JWT based. Admin and Voter roles.
- **Responsive Design**: Modern UI with "Inter" font and glassmorphism effects.
- **Charts**: Integration with `chart.js` and `ng2-charts` for visualizing results.
- **Voting Logic**: Prevention of duplicate votes locally and via API.
