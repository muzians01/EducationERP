import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';

import { AppDataStore } from '../app.data';

@Component({
  selector: 'app-admissions-page',
  imports: [DatePipe, CurrencyPipe],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Admissions Module</p>
      <h2>Applications, guardians, and onboarding readiness</h2>
    </section>

    <div class="workspace-grid">
      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Applications</p>
            <h3>Current admissions queue</h3>
          </div>
        </div>
        @if (store.admissionsDashboard(); as admissions) {
          <div class="application-list">
            @for (application of admissions.recentApplications; track application.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ application.studentName }}</strong>
                    <p>{{ application.applicationNumber }} - {{ application.campusName }}</p>
                  </div>
                  <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(application.status)">{{ application.status }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ application.className }} / {{ application.sectionName }}</span>
                  <span>{{ application.appliedOn | date: 'dd MMM yyyy' }}</span>
                  <span>{{ application.registrationFee | currency: 'INR':'symbol':'1.0-0' }}</span>
                </div>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Guardians</p>
            <h3>Family contacts</h3>
          </div>
        </div>
        @if (store.admissionsDashboard(); as admissions) {
          <div class="guardian-list">
            @for (guardian of admissions.guardians; track guardian.id) {
              <article class="guardian-item">
                <strong>{{ guardian.fullName }}</strong>
                <p>{{ guardian.relationship }} - {{ guardian.occupation }}</p>
                <p>{{ guardian.phoneNumber }} - {{ guardian.email }}</p>
                <span>{{ guardian.campusName }}</span>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Profiles</p>
            <h3>Student onboarding status</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (profile of store.studentProfiles(); track profile.id) {
            <article class="guardian-item">
              <strong>{{ profile.studentName }}</strong>
              <p>{{ profile.className }} / {{ profile.sectionName }}</p>
              <p>{{ profile.primaryContactNumber }}</p>
              <span>{{ profile.pendingDocumentCount }} pending document(s)</span>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Documents</p>
            <h3>Verification queue</h3>
          </div>
        </div>
        <div class="application-list">
          @for (document of store.studentDocuments(); track document.id) {
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>{{ document.documentType }}</strong>
                  <p>{{ document.studentName }} - {{ document.admissionNumber }}</p>
                </div>
                <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(document.status)">{{ document.status }}</span>
              </div>
            </article>
          }
        </div>
      </article>
    </div>
  `
})
export class AdmissionsPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly statusTone = (status: string) => status.toLowerCase().replace(/\s+/g, '-');
}
