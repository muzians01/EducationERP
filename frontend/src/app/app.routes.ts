import { Routes } from '@angular/router';
import { AdmissionsPageComponent } from './pages/admissions-page.component';
import { AcademicsPageComponent } from './pages/academics-page.component';
import { AttendancePageComponent } from './pages/attendance-page.component';
import { DashboardPageComponent } from './pages/dashboard-page.component';
import { ExaminationsPageComponent } from './pages/examinations-page.component';
import { FeesPageComponent } from './pages/fees-page.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: DashboardPageComponent },
  { path: 'admissions', component: AdmissionsPageComponent },
  { path: 'academics', component: AcademicsPageComponent },
  { path: 'examinations', component: ExaminationsPageComponent },
  { path: 'fees', component: FeesPageComponent },
  { path: 'attendance', component: AttendancePageComponent },
  { path: '**', redirectTo: '' }
];
