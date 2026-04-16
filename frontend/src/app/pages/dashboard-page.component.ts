import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AppDataStore } from '../app.data';
import { matchesSearch } from '../search.utils';

@Component({
  selector: 'app-dashboard-page',
  imports: [CurrencyPipe, RouterLink],
  template: `
    <section class="hero">
      <div class="hero__content">
        <p class="eyebrow">ERP Overview</p>
        <h1>School operations at a glance</h1>
        <p class="hero__summary">Move from overview into focused admissions, fees, and attendance workspaces.</p>
      </div>

      <div class="hero__panel">
        <p class="panel__title">Operational focus</p>
        <ul class="panel__list">
          @for (item of focusAreas; track item.step) {
            <li>
              <span>{{ item.step }}</span>
              <p>{{ item.summary }}</p>
            </li>
          }
        </ul>
      </div>
    </section>

    @if (store.loadError()) {
      <section class="notice notice--error">
        <strong>Backend connection needed.</strong>
        <p>{{ store.loadError() }}</p>
      </section>
    }

    <section class="metrics">
      @for (metric of metrics(); track metric.label) {
        <article class="metric-card">
          <span>{{ metric.label }}</span>
          <strong>{{ metric.value }}</strong>
          <p>{{ metric.description }}</p>
        </article>
      }
    </section>

    @if (store.isLoading()) {
      <section class="notice">
        <strong>Loading ERP workspace...</strong>
        <p>Admissions, fee, and attendance data are being fetched from the API.</p>
      </section>
    } @else {
      <section class="workspace">
        <div class="section-heading">
          <p class="eyebrow">Overview</p>
          <h2>Current school pulse</h2>
        </div>

        <label class="list-search">
          <span>Quick Search</span>
          <input type="search" [value]="searchQuery()" (input)="searchQuery.set($any($event.target).value)" placeholder="Filter overview cards, students, receipts, and applications" />
        </label>

        <div class="workspace-grid">
          <article class="data-card">
            <div class="data-card__header">
              <div>
                <p class="eyebrow">Modules</p>
                <h3>Open a workspace directly</h3>
              </div>
            </div>
            <div class="form-actions">
              @for (module of moduleLinks; track module.path) {
                <a class="button button--secondary" [routerLink]="module.path">{{ module.label }}</a>
              }
            </div>
          </article>

          <article class="data-card">
            <div class="data-card__header">
              <div>
                <p class="eyebrow">Admissions</p>
                <h3>Recent pipeline movement</h3>
              </div>
            </div>
            @if (store.admissionsDashboard(); as admissions) {
              <div class="application-list">
                @for (application of filteredApplications(); track application.id) {
                  <article class="application-item">
                    <div class="application-item__main">
                      <div>
                        <strong>{{ application.studentName }}</strong>
                        <p>{{ application.applicationNumber }} - {{ application.campusName }}</p>
                      </div>
                      <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(application.status)">{{ application.status }}</span>
                    </div>
                  </article>
                }
              </div>
            }
          </article>

          <article class="data-card">
            <div class="data-card__header">
              <div>
                <p class="eyebrow">Fees</p>
                <h3>Recent receipts</h3>
              </div>
            </div>
            @if (store.feesDashboard(); as fees) {
              <div class="guardian-list">
                @for (receipt of filteredReceipts(); track receipt.id) {
                  <article class="guardian-item">
                    <strong>{{ receipt.studentName }}</strong>
                    <p>{{ receipt.feeName }} - {{ receipt.paymentMethod }}</p>
                    <span>{{ receipt.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
                  </article>
                }
              </div>
            }
          </article>

          <article class="data-card">
            <div class="data-card__header">
              <div>
                <p class="eyebrow">Attendance</p>
                <h3>Students needing attention</h3>
              </div>
            </div>
            @if (store.attendanceMonthlyReport(); as report) {
              <div class="guardian-list">
                @for (student of filteredWatchlistStudents(); track student.studentId) {
                  <article class="guardian-item">
                    <strong>{{ student.studentName }}</strong>
                    <p>{{ student.className }} / {{ student.sectionName }}</p>
                    <span>{{ student.attendancePercentage }}% attendance</span>
                  </article>
                }
              </div>
            }
          </article>

          <article class="data-card">
            <div class="data-card__header">
              <div>
                <p class="eyebrow">Readiness</p>
                <h3>Profile and document closure</h3>
              </div>
            </div>
            <div class="guardian-list">
              @for (profile of filteredProfiles(); track profile.id) {
                <article class="guardian-item">
                  <strong>{{ profile.studentName }}</strong>
                  <p>{{ profile.profileCompletionPercentage }}% complete</p>
                  <span>{{ profile.pendingDocumentCount }} pending document(s)</span>
                </article>
              }
            </div>
          </article>
        </div>
      </section>
    }
  `
})
export class DashboardPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly searchQuery = signal('');
  protected readonly statusTone = (status: string) => status.toLowerCase().replace(/\s+/g, '-');
  protected readonly moduleLinks = [
    { label: 'Master Data', path: '/master-data' },
    { label: 'Admissions', path: '/admissions' },
    { label: 'Academics', path: '/academics' },
    { label: 'Examinations', path: '/examinations' },
    { label: 'Homework', path: '/homework' },
    { label: 'Fees', path: '/fees' },
    { label: 'Attendance', path: '/attendance' },
    { label: 'Transport', path: '/transport' },
    { label: 'Parent Portal', path: '/parent-portal' }
  ];
  protected readonly focusAreas = [
    { step: 'Admissions pulse', summary: 'Track new, approved, and waiting applications.' },
    { step: 'Collections watch', summary: 'See what was collected and what is still pending.' },
    { step: 'Attendance risk', summary: 'Spot students and classes that need intervention.' }
  ];
  protected readonly filteredApplications = computed(() =>
    (this.store.admissionsDashboard()?.recentApplications ?? []).filter((application) =>
      matchesSearch(this.searchQuery(), application.studentName, application.applicationNumber, application.campusName, application.status))
  );
  protected readonly filteredReceipts = computed(() =>
    (this.store.feesDashboard()?.recentReceipts ?? []).filter((receipt) =>
      matchesSearch(this.searchQuery(), receipt.studentName, receipt.feeName, receipt.paymentMethod, receipt.receiptNumber))
  );
  protected readonly filteredWatchlistStudents = computed(() =>
    (this.store.attendanceMonthlyReport()?.studentsNeedingAttention ?? []).filter((student) =>
      matchesSearch(this.searchQuery(), student.studentName, student.className, student.sectionName, student.attendancePercentage))
  );
  protected readonly filteredProfiles = computed(() =>
    this.store.studentProfiles().filter((profile) =>
      matchesSearch(this.searchQuery(), profile.studentName, profile.className, profile.sectionName, profile.primaryContactNumber))
  );

  protected readonly metrics = computed(() => {
    const admissions = this.store.admissionsDashboard();
    const fees = this.store.feesDashboard();
    const attendance = this.store.attendanceDashboard();
    const report = this.store.attendanceMonthlyReport();

    return [
      { label: 'Applications', value: admissions?.totalApplications.toString() ?? '--', description: 'Students in the admissions funnel.' },
      { label: 'Outstanding Fees', value: fees ? `Rs ${fees.outstandingAmount}` : '--', description: 'Net amount still awaiting collection.' },
      { label: 'Attendance Today', value: attendance?.totalStudentsMarked.toString() ?? '--', description: 'Rows captured in the latest attendance session.' },
      { label: 'Monthly Attendance', value: report ? `${report.overallAttendancePercentage}%` : '--', description: 'Overall trend for the latest month.' },
      { label: 'Active Routes', value: this.store.transportDashboard()?.totalRoutes.toString() ?? '--', description: 'Transport routes currently configured.' },
      { label: 'Pending Docs', value: this.store.pendingDocumentsCount().toString(), description: 'Student documents waiting to be cleared.' }
    ];
  });
}
