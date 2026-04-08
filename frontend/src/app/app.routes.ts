import { Routes } from '@angular/router';
import { AdmissionsPageComponent } from './pages/admissions-page.component';
import { AcademicsPageComponent } from './pages/academics-page.component';
import { AttendancePageComponent } from './pages/attendance-page.component';
import { DashboardPageComponent } from './pages/dashboard-page.component';
import { ExaminationsPageComponent } from './pages/examinations-page.component';
import { FeesPageComponent } from './pages/fees-page.component';
import { HomeworkPageComponent } from './pages/homework-page.component';
import { MasterDataPageComponent } from './pages/master-data-page.component';
import { ParentPortalPageComponent } from './pages/parent-portal-page.component';
import { TransportPageComponent } from './pages/transport-page.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: DashboardPageComponent },
  { path: 'master-data', component: MasterDataPageComponent },
  { path: 'admissions', component: AdmissionsPageComponent },
  { path: 'academics', component: AcademicsPageComponent },
  { path: 'examinations', component: ExaminationsPageComponent },
  { path: 'homework', component: HomeworkPageComponent },
  { path: 'fees', component: FeesPageComponent },
  { path: 'attendance', component: AttendancePageComponent },
  { path: 'transport', component: TransportPageComponent },
  { path: 'parent-portal', component: ParentPortalPageComponent },
  { path: '**', redirectTo: '' }
];
