import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { CreateAdmissionApplication } from '../app.models';

@Component({
  selector: 'app-admissions-page',
  imports: [DatePipe, CurrencyPipe, ReactiveFormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Admissions Module</p>
      <h2>Applications, guardians, and onboarding readiness</h2>
    </section>

    <section class="metrics">
      <article class="metric-card">
        <span>Total Applications</span>
        <strong>{{ store.admissionsDashboard()?.totalApplications ?? 0 }}</strong>
        <p>All candidates currently in the intake pipeline.</p>
      </article>
      <article class="metric-card">
        <span>New</span>
        <strong>{{ store.admissionsDashboard()?.newApplications ?? 0 }}</strong>
        <p>Fresh submissions waiting for counsellor review.</p>
      </article>
      <article class="metric-card">
        <span>Approved</span>
        <strong>{{ store.admissionsDashboard()?.approvedApplications ?? 0 }}</strong>
        <p>Applications ready for student onboarding.</p>
      </article>
      <article class="metric-card">
        <span>Registration Fees</span>
        <strong>{{ (store.admissionsDashboard()?.totalRegistrationFees ?? 0) | currency: 'INR':'symbol':'1.0-0' }}</strong>
        <p>Fee value received across the pipeline.</p>
      </article>
    </section>

    <section class="data-card">
      <div class="data-card__header">
        <div>
          <p class="eyebrow">New Application</p>
          <h3>Submit admission application</h3>
        </div>
      </div>
      <form [formGroup]="applicationForm" (ngSubmit)="onSubmitApplication()" class="form-grid">
        <label class="form-field">
          <span>First Name</span>
          <input type="text" formControlName="studentFirstName" />
        </label>
        <label class="form-field">
          <span>Last Name</span>
          <input type="text" formControlName="studentLastName" />
        </label>
        <label class="form-field">
          <span>Date of Birth</span>
          <input type="date" formControlName="dateOfBirth" />
        </label>
        <label class="form-field">
          <span>Gender</span>
          <select formControlName="gender">
            <option value="">Select gender</option>
            @for (gender of genderOptions; track gender) {
              <option [value]="gender">{{ gender }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Campus</span>
          <select formControlName="campusId">
            <option value="">Select campus</option>
            @for (campus of store.academicStructure()?.campuses ?? []; track campus.id) {
              <option [value]="campus.id">{{ campus.name }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Academic Year</span>
          <select formControlName="academicYearId">
            <option value="">Select year</option>
            @for (year of store.academicStructure()?.academicYears ?? []; track year.id) {
              <option [value]="year.id">{{ year.name }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Class</span>
          <select formControlName="schoolClassId">
            <option value="">Select class</option>
            @for (schoolClass of store.academicStructure()?.classes ?? []; track schoolClass.id) {
              <option [value]="schoolClass.id">{{ schoolClass.name }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Section</span>
          <select formControlName="sectionId">
            <option value="">Select section</option>
            @for (section of availableSections(); track section.id) {
              <option [value]="section.id">{{ section.name }} - {{ section.roomNumber }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Guardian</span>
          <select formControlName="guardianId">
            <option value="">Select guardian</option>
            @for (guardian of store.admissionGuardians(); track guardian.id) {
              <option [value]="guardian.id">{{ guardian.fullName }} - {{ guardian.relationship }}</option>
            }
          </select>
        </label>
        <label class="form-field">
          <span>Registration Fee</span>
          <input type="number" formControlName="registrationFee" min="0" />
        </label>
        <div class="form-actions">
          <button type="submit" class="button button--primary" [disabled]="applicationForm.invalid || isSubmitting()">
            {{ isSubmitting() ? 'Submitting...' : 'Submit Application' }}
          </button>
        </div>
      </form>
      @if (feedbackMessage(); as message) {
        <div class="notice" [class.notice--error]="feedbackTone() === 'error'">
          <strong>Admissions update</strong>
          <p>{{ message }}</p>
        </div>
      }
    </section>

    <div class="workspace-grid">
      <article class="data-card data-card--span">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Applications</p>
            <h3>Complete admissions queue</h3>
          </div>
        </div>
        <div class="application-list">
          @for (application of store.admissionApplications(); track application.id) {
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
                <span>{{ application.guardianName }}</span>
                <span>{{ application.appliedOn | date: 'dd MMM yyyy' }}</span>
                <span>{{ application.registrationFee | currency: 'INR':'symbol':'1.0-0' }}</span>
              </div>
              <div class="form-actions form-actions--inline">
                @if (application.status !== 'Approved') {
                  <button type="button" class="button button--secondary" (click)="updateStatus(application.id, 'Approved')">Approve</button>
                }
                @if (application.status !== 'Waitlisted') {
                  <button type="button" class="button button--secondary" (click)="updateStatus(application.id, 'Waitlisted')">Waitlist</button>
                }
                @if (application.status !== 'Rejected') {
                  <button type="button" class="button button--danger" (click)="updateStatus(application.id, 'Rejected')">Reject</button>
                }
              </div>
            </article>
          } @empty {
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>No applications yet</strong>
                  <p>New submissions will appear here once created.</p>
                </div>
              </div>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Guardians</p>
            <h3>Family contacts</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (guardian of store.admissionGuardians(); track guardian.id) {
            <article class="guardian-item">
              <strong>{{ guardian.fullName }}</strong>
              <p>{{ guardian.relationship }} - {{ guardian.occupation }}</p>
              <p>{{ guardian.phoneNumber }} - {{ guardian.email }}</p>
              <span>{{ guardian.campusName }}</span>
            </article>
          }
        </div>
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
  protected readonly isSubmitting = signal(false);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly genderOptions = ['Male', 'Female', 'Other'];
  protected readonly availableSections = computed(() => {
    const classId = Number(this.applicationForm.controls.schoolClassId.value || 0);
    return (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === classId);
  });

  private readonly fb = inject(FormBuilder);
  protected readonly applicationForm = this.fb.nonNullable.group({
    studentFirstName: ['', Validators.required],
    studentLastName: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    gender: ['', Validators.required],
    campusId: ['', Validators.required],
    academicYearId: ['', Validators.required],
    schoolClassId: ['', Validators.required],
    sectionId: ['', Validators.required],
    guardianId: ['', Validators.required],
    registrationFee: [1500, [Validators.required, Validators.min(0)]]
  });

  constructor() {
    this.applicationForm.controls.schoolClassId.valueChanges.subscribe(() => {
      this.applicationForm.controls.sectionId.setValue('');
    });
  }

  protected onSubmitApplication(): void {
    if (this.applicationForm.invalid) {
      return;
    }

    this.isSubmitting.set(true);
    this.feedbackMessage.set(null);

    const formValue = this.applicationForm.getRawValue();
    const dto: CreateAdmissionApplication = {
      campusId: Number(formValue.campusId),
      academicYearId: Number(formValue.academicYearId),
      schoolClassId: Number(formValue.schoolClassId),
      sectionId: Number(formValue.sectionId),
      guardianId: Number(formValue.guardianId),
      studentFirstName: formValue.studentFirstName,
      studentLastName: formValue.studentLastName,
      dateOfBirth: formValue.dateOfBirth,
      gender: formValue.gender,
      registrationFee: Number(formValue.registrationFee)
    };

    this.store.createAdmissionApplication(dto).subscribe({
      next: () => {
        this.applicationForm.reset({
          studentFirstName: '',
          studentLastName: '',
          dateOfBirth: '',
          gender: '',
          campusId: '',
          academicYearId: '',
          schoolClassId: '',
          sectionId: '',
          guardianId: '',
          registrationFee: 1500
        });
        this.isSubmitting.set(false);
        this.feedbackTone.set('success');
        this.feedbackMessage.set('Admission application submitted successfully.');
        this.store.loadAdmissionsDashboard();
      },
      error: () => {
        this.isSubmitting.set(false);
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Admission application could not be submitted.');
      }
    });
  }

  protected updateStatus(applicationId: number, status: string): void {
    this.feedbackMessage.set(null);
    this.store.updateAdmissionApplicationStatus(applicationId, status).subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(`Application marked as ${status.toLowerCase()}.`);
        this.store.loadAdmissionsDashboard();
      },
      error: () => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Application status could not be updated.');
      }
    });
  }
}
