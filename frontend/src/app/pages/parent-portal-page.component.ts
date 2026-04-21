import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';

@Component({
  selector: 'app-parent-portal-page',
  imports: [CurrencyPipe, DatePipe, FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Parent Portal</p>
      <h2>Student progress, fees, homework, and classroom day at a glance</h2>
    </section>

    <section class="workspace-grid">
      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Student Switcher</p>
            <h3>Open the portal for a linked learner</h3>
          </div>
        </div>
        <div class="attendance-filter-grid">
          <label class="attendance-remark-field attendance-remark-field--compact">
            <span>Student</span>
            <select [ngModel]="selectedStudentId()" (ngModelChange)="selectedStudentId.set($event)">
              @for (student of studentOptions(); track student.id) {
                <option [ngValue]="student.id">{{ student.studentName }} - {{ student.className }} / {{ student.sectionName }}</option>
              }
            </select>
          </label>
          <button
            type="button"
            class="button button--secondary"
            [disabled]="!selectedStudentId() || store.isLoadingParentPortal()"
            (click)="loadPortal()">
            {{ store.isLoadingParentPortal() ? 'Loading...' : 'Load portal' }}
          </button>
        </div>
        @if (store.parentPortalError(); as errorMessage) {
          <div class="notice notice--error">
            <strong>Parent portal update failed.</strong>
            <p>{{ errorMessage }}</p>
          </div>
        }
      </article>

      @if (store.parentPortalDashboard(); as portal) {
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Snapshot</p>
              <h3>Current student context</h3>
            </div>
          </div>
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>{{ portal.studentName }}</strong>
              <p>{{ portal.admissionNumber }} - {{ portal.className }} / {{ portal.sectionName }}</p>
              <span>{{ portal.guardianName }} | {{ portal.guardianPhoneNumber }}</span>
            </article>
          </div>
        </article>
      }
    </section>

    @if (store.parentPortalDashboard(); as portal) {
      <section class="metrics">
        <article class="metric-card">
          <span>Attendance</span>
          <strong>{{ portal.attendancePercentage }}%</strong>
          <p>Weighted attendance across recent recorded days.</p>
        </article>
        <article class="metric-card">
          <span>Outstanding Fees</span>
          <strong>{{ portal.outstandingFees | currency: 'INR':'symbol':'1.0-0' }}</strong>
          <p>Current dues awaiting payment.</p>
        </article>
        <article class="metric-card">
          <span>Exam Term</span>
          <strong>{{ portal.currentExamTerm }}</strong>
          <p>Latest published assessment window.</p>
        </article>
        <article class="metric-card">
          <span>Latest Result</span>
          <strong>{{ portal.latestExamPercentage }}%</strong>
          <p>Most recent exam average.</p>
        </article>
      </section>

      <div class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Student</p>
              <h3>Linked learner profile</h3>
            </div>
          </div>
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>{{ portal.studentName }}</strong>
              <p>{{ portal.admissionNumber }} - {{ portal.className }} / {{ portal.sectionName }}</p>
              <span>{{ portal.guardianName }} | {{ portal.guardianPhoneNumber }}</span>
            </article>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Announcements</p>
              <h3>What parents should know</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (announcement of portal.announcements; track announcement.title) {
              <article class="guardian-item">
                <strong>{{ announcement.title }}</strong>
                <p>{{ announcement.message }}</p>
                <span>{{ announcement.publishDate | date: 'dd MMM yyyy' }}</span>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Homework</p>
              <h3>Upcoming assignments</h3>
            </div>
          </div>
          <div class="application-list">
            @for (item of portal.upcomingHomework; track item.subjectName + item.title) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ item.title }}</strong>
                    <p>{{ item.subjectName }}</p>
                  </div>
                  <span class="status-chip" [class]="'status-chip status-chip--' + item.status.toLowerCase()">{{ item.status }}</span>
                </div>
                <div class="application-meta">
                  <span>Due {{ item.dueOn | date: 'dd MMM yyyy' }}</span>
                </div>
                <p class="application-guardian">{{ item.instructions }}</p>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Fees</p>
              <h3>Outstanding items</h3>
            </div>
          </div>
          <div class="application-list">
            @for (fee of portal.outstandingFeeItems; track fee.feeName + fee.dueOn) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ fee.feeName }}</strong>
                    <p>Due {{ fee.dueOn | date: 'dd MMM yyyy' }}</p>
                  </div>
                  <span class="status-chip status-chip--waitlisted">{{ fee.status }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ fee.balanceAmount | currency: 'INR':'symbol':'1.0-0' }}</span>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Exam Results</p>
              <h3>Latest subject performance</h3>
            </div>
          </div>
          <div class="application-list">
            @for (result of portal.examResults; track result.examTermName + result.subjectName) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ result.subjectName }}</strong>
                    <p>{{ result.examTermName }}</p>
                  </div>
                  <span class="status-chip status-chip--active">{{ result.grade }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ result.marksObtained }} / {{ result.maxMarks }}</span>
                  <span>{{ result.resultStatus }}</span>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Today&apos;s Timetable</p>
              <h3>Classes lined up for the day</h3>
            </div>
          </div>
          <div class="application-list">
            @for (period of portal.todayTimetable; track period.dayOfWeek + period.periodNumber) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>P{{ period.periodNumber }} - {{ period.subjectName }}</strong>
                    <p>{{ period.startTime }} to {{ period.endTime }}</p>
                  </div>
                  <span class="status-chip status-chip--active">{{ period.teacherName }}</span>
                </div>
              </article>
            } @empty {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>No class periods today</strong>
                    <p>The student has no timetable periods for the current day.</p>
                  </div>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Recent Attendance</p>
              <h3>Latest attendance updates</h3>
            </div>
          </div>
          <div class="application-list">
            @for (attendance of portal.recentAttendance; track attendance.attendanceDate) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ attendance.attendanceDate | date: 'dd MMM yyyy' }}</strong>
                    <p>{{ attendance.remarks || 'No remark recorded.' }}</p>
                  </div>
                  <span class="status-chip" [class]="'status-chip status-chip--' + attendance.status.toLowerCase()">{{ attendance.status }}</span>
                </div>
              </article>
            }
          </div>
        </article>
      </div>
    }

    @if (!store.parentPortalDashboard() && !store.isLoadingParentPortal()) {
      <section class="notice" [class.notice--error]="!!store.parentPortalError()">
        <strong>{{ store.parentPortalError() ? 'Parent portal is unavailable.' : 'Parent portal is ready to load.' }}</strong>
        <p>{{ store.parentPortalError() || 'Choose a student above to load attendance, homework, exam, and fee details.' }}</p>
      </section>
    }
  `
})
export class ParentPortalPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly selectedStudentId = signal<number | null>(null);
  protected readonly studentOptions = computed(() => this.store.students());
  private hasLoadedInitialPortal = false;

  constructor() {
    effect(() => {
      const currentPortal = this.store.parentPortalDashboard();
      const students = this.studentOptions();

      if (currentPortal) {
        this.selectedStudentId.set(currentPortal.studentId);
        return;
      }

      if (!this.selectedStudentId() && students.length > 0) {
        this.selectedStudentId.set(students[0].id);
      }

      if (!currentPortal && !this.store.isLoadingParentPortal() && students.length > 0 && !this.hasLoadedInitialPortal) {
        this.hasLoadedInitialPortal = true;
        this.store.loadParentPortal(this.selectedStudentId() ?? students[0].id);
      }
    });
  }

  protected loadPortal(): void {
    this.store.loadParentPortal(this.selectedStudentId());
  }
}
