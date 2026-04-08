import { DatePipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { AttendanceEntryDraft, CreateAttendanceLeaveRequest } from '../app.models';

@Component({
  selector: 'app-attendance-page',
  imports: [DatePipe, FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Attendance Module</p>
      <h2>Daily marking, leave desk, and section register</h2>
    </section>

    <section class="metrics">
      <article class="metric-card">
        <span>Marked Today</span>
        <strong>{{ store.attendanceDashboard()?.totalStudentsMarked ?? 0 }}</strong>
        <p>Students marked in the selected roster.</p>
      </article>
      <article class="metric-card">
        <span>Present</span>
        <strong>{{ store.attendanceDashboard()?.presentCount ?? 0 }}</strong>
        <p>Students present in the latest run.</p>
      </article>
      <article class="metric-card">
        <span>Absent</span>
        <strong>{{ store.attendanceDashboard()?.absentCount ?? 0 }}</strong>
        <p>Students absent and needing follow-up.</p>
      </article>
      <article class="metric-card">
        <span>Monthly Attendance</span>
        <strong>{{ store.attendanceMonthlyReport()?.overallAttendancePercentage ?? 0 }}%</strong>
        <p>Current month overall trend.</p>
      </article>
    </section>

    <div class="workspace-grid">
      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Entry Board</p>
            <h3>Roster marking by class and section</h3>
          </div>
        </div>
        @if (store.attendanceEntryBoard(); as board) {
          <div class="application-list">
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>{{ board.className }} / {{ board.sectionName }}</strong>
                  <p>{{ board.studentsMarked }} of {{ board.studentsOnRoll }} student(s) marked</p>
                </div>
                <span class="status-chip status-chip--active">{{ board.attendanceDate | date: 'dd MMM yyyy' }}</span>
              </div>
            </article>
            <div class="attendance-filter-grid">
              <label class="attendance-remark-field attendance-remark-field--compact">
                <span>Attendance Date</span>
                <input type="date" [ngModel]="selectedDate()" (ngModelChange)="selectedDate.set($event)" />
              </label>
              <label class="attendance-remark-field attendance-remark-field--compact">
                <span>Class</span>
                <select [ngModel]="selectedClassId()" (ngModelChange)="onClassChange($event)">
                  @for (schoolClass of classOptions(); track schoolClass.id) {
                    <option [ngValue]="schoolClass.id">{{ schoolClass.name }}</option>
                  }
                </select>
              </label>
              <label class="attendance-remark-field attendance-remark-field--compact">
                <span>Section</span>
                <select [ngModel]="selectedSectionId()" (ngModelChange)="selectedSectionId.set($event)">
                  @for (section of sectionOptions(); track section.id) {
                    <option [ngValue]="section.id">{{ section.name }}</option>
                  }
                </select>
              </label>
              <button type="button" class="button button--secondary" [disabled]="!canLoadSelection() || store.isLoading()" (click)="loadSelectedRoster()">
                Load roster
              </button>
            </div>
            <div class="attendance-toolbar">
              <div class="attendance-toolbar__summary">
                <span>{{ draftPresentCount() }} present</span>
                <span>{{ draftAbsentCount() }} absent</span>
                <span>{{ draftLateCount() }} late</span>
              </div>
              <div class="attendance-toolbar__actions">
                <button type="button" class="button button--secondary" (click)="markAll('Present')">Mark all present</button>
                <button
                  type="button"
                  class="button button--primary"
                  [disabled]="hasUnmarkedStudents() || store.isSavingAttendance()"
                  (click)="saveAttendance(board.attendanceDate)">
                  {{ store.isSavingAttendance() ? 'Saving...' : 'Save attendance' }}
                </button>
              </div>
            </div>
            @if (store.attendanceSaveMessage(); as message) {
              <div class="notice" [class.notice--error]="message.includes('could not') || message.includes('failed')">
                <strong>Attendance update</strong>
                <p>{{ message }}</p>
              </div>
            }
            @for (student of board.students; track student.studentId) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ student.studentName }}</strong>
                    <p>{{ student.admissionNumber }}{{ student.hasApprovedLeave ? ' - ' + student.leaveType : '' }}</p>
                  </div>
                  <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(draftFor(student.studentId).status)">{{ draftFor(student.studentId).status }}</span>
                </div>
                <div class="attendance-status-picker">
                  @for (option of statusOptions; track option) {
                    <button
                      type="button"
                      class="attendance-status-picker__option"
                      [class.attendance-status-picker__option--active]="draftFor(student.studentId).status === option"
                      (click)="setStatus(student.studentId, option)">
                      {{ option }}
                    </button>
                  }
                </div>
                <label class="attendance-remark-field">
                  <span>Remarks</span>
                  <input
                    type="text"
                    [ngModel]="draftFor(student.studentId).remarks"
                    (ngModelChange)="setRemarks(student.studentId, $event)"
                    [placeholder]="student.hasApprovedLeave ? 'Leave note or reason' : 'Optional note'" />
                </label>
                <p class="application-guardian">{{ student.remarks || 'No leave or remark recorded.' }}</p>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">New Leave Request</p>
            <h3>Raise a leave entry for a student</h3>
          </div>
        </div>
        <div class="form-grid">
          <label class="form-field">
            <span>Student</span>
            <select [(ngModel)]="leaveDraft.studentId" name="leaveStudentId">
              <option [ngValue]="0">Select student</option>
              @for (student of store.attendanceEntryBoard()?.students ?? []; track student.studentId) {
                <option [ngValue]="student.studentId">{{ student.studentName }} · {{ student.admissionNumber }}</option>
              }
            </select>
          </label>
          <label class="form-field">
            <span>Leave Date</span>
            <input type="date" [(ngModel)]="leaveDraft.leaveDate" name="leaveDate" />
          </label>
          <label class="form-field">
            <span>Leave Type</span>
            <select [(ngModel)]="leaveDraft.leaveType" name="leaveType">
              @for (type of leaveTypes; track type) {
                <option [value]="type">{{ type }}</option>
              }
            </select>
          </label>
          <label class="form-field">
            <span>Reason</span>
            <textarea [(ngModel)]="leaveDraft.reason" name="leaveReason" rows="2"></textarea>
          </label>
          <div class="form-actions">
            <button type="button" class="button button--primary" [disabled]="!canSubmitLeave() || store.isCreatingLeave()" (click)="submitLeaveRequest()">
              {{ store.isCreatingLeave() ? 'Submitting...' : 'Submit Leave Request' }}
            </button>
          </div>
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Leave Desk</p>
            <h3>Approve or reject leave requests</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (request of store.attendanceLeaveRequests(); track request.id) {
            <article class="guardian-item">
              <strong>{{ request.studentName }}</strong>
              <p>{{ request.leaveType }} - {{ request.reason }}</p>
              <span>{{ request.status }}</span>
              <div class="attendance-status-picker">
                <button type="button" class="attendance-status-picker__option" [disabled]="store.isUpdatingLeave()" (click)="updateLeave(request.id, 'Approved')">Approve</button>
                <button type="button" class="attendance-status-picker__option" [disabled]="store.isUpdatingLeave()" (click)="updateLeave(request.id, 'Rejected')">Reject</button>
              </div>
            </article>
          } @empty {
            <article class="guardian-item">
              <strong>No leave requests</strong>
              <p>The selected roster has no leave requests for this day.</p>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Recent Records</p>
            <h3>Latest attendance rows</h3>
          </div>
        </div>
        <div class="application-list">
          @for (record of recentRecords(); track record.id) {
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>{{ record.studentName }}</strong>
                  <p>{{ record.attendanceDate | date: 'dd MMM yyyy' }} · {{ record.admissionNumber }}</p>
                </div>
                <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(record.status)">{{ record.status }}</span>
              </div>
              <p class="application-guardian">{{ record.remarks || 'No remarks recorded.' }}</p>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Class Rollup</p>
            <h3>Section-wise daily summary</h3>
          </div>
        </div>
        @if (store.attendanceDashboard(); as dashboard) {
          <div class="guardian-list">
            @for (summary of dashboard.classSummaries; track summary.className + summary.sectionName) {
              <article class="guardian-item">
                <strong>{{ summary.className }} / {{ summary.sectionName }}</strong>
                <p>{{ summary.attendancePercentage }}% attendance with {{ summary.studentsMarked }} marked</p>
                <span>Present {{ summary.presentCount }}, Late {{ summary.lateCount }}, Absent {{ summary.absentCount }}</span>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Monthly Watchlist</p>
            <h3>Students needing follow-up</h3>
          </div>
        </div>
        @if (store.attendanceMonthlyReport(); as report) {
          <div class="guardian-list">
            @for (student of report.studentsNeedingAttention; track student.studentId) {
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
            <p class="eyebrow">Class Register</p>
            <h3>Monthly day-wise register</h3>
          </div>
        </div>
        @if (store.classAttendanceRegister(); as register) {
          <div class="application-list">
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>{{ register.className }} / {{ register.sectionName }}</strong>
                  <p>{{ register.monthLabel }} - {{ register.workingDayLabels.length }} working day(s)</p>
                </div>
              </div>
            </article>
            @for (row of register.rows; track row.studentId) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ row.studentName }}</strong>
                    <p>{{ row.admissionNumber }}</p>
                  </div>
                  <span class="status-chip status-chip--active">P {{ row.presentDays }} / A {{ row.absentDays }} / L {{ row.lateDays }}</span>
                </div>
                <div class="application-meta">
                  @for (day of register.workingDayLabels; track day) {
                    <span>{{ day }}: {{ row.dailyStatus[day] || '-' }}</span>
                  }
                </div>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Upcoming Holidays</p>
            <h3>Calendar context</h3>
          </div>
        </div>
        @if (store.attendanceEntryBoard(); as board) {
          <div class="guardian-list">
            @for (holiday of board.upcomingHolidays; track holiday.id) {
              <article class="guardian-item">
                <strong>{{ holiday.title }}</strong>
                <p>{{ holiday.category }}</p>
                <span>{{ holiday.holidayDate | date: 'dd MMM yyyy' }}</span>
              </article>
            }
          </div>
        }
      </article>
    </div>
  `
})
export class AttendancePageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly statusOptions = ['Present', 'Absent', 'Late'];
  protected readonly leaveTypes = ['Sick Leave', 'Personal Leave', 'Emergency Leave', 'Family Event'];
  protected readonly draftEntries = signal<Record<number, AttendanceEntryDraft>>({});
  protected readonly selectedDate = signal<string>('');
  protected readonly selectedClassId = signal<number | null>(null);
  protected readonly selectedSectionId = signal<number | null>(null);
  protected leaveDraft: CreateAttendanceLeaveRequest = {
    studentId: 0,
    leaveDate: '',
    leaveType: 'Sick Leave',
    reason: ''
  };
  protected readonly statusTone = (status: string) => status.toLowerCase().replace(/\s+/g, '-');
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );
  protected readonly recentRecords = computed(() => this.store.attendanceDashboard()?.todayRecords ?? []);
  protected readonly hasUnmarkedStudents = computed(() =>
    Object.values(this.draftEntries()).some((entry) => entry.status === 'Unmarked')
  );
  protected readonly canLoadSelection = computed(() =>
    Boolean(this.selectedDate() && this.selectedClassId() && this.selectedSectionId())
  );
  protected readonly draftPresentCount = computed(() =>
    Object.values(this.draftEntries()).filter((entry) => entry.status === 'Present').length
  );
  protected readonly draftAbsentCount = computed(() =>
    Object.values(this.draftEntries()).filter((entry) => entry.status === 'Absent').length
  );
  protected readonly draftLateCount = computed(() =>
    Object.values(this.draftEntries()).filter((entry) => entry.status === 'Late').length
  );

  constructor() {
    effect(() => {
      const board = this.store.attendanceEntryBoard();

      if (!board) {
        return;
      }

      this.selectedDate.set(board.attendanceDate);
      this.selectedClassId.set(board.classId);
      this.selectedSectionId.set(board.sectionId);
      this.leaveDraft = {
        studentId: board.students[0]?.studentId ?? 0,
        leaveDate: board.attendanceDate,
        leaveType: 'Sick Leave',
        reason: ''
      };

      const draft = board.students.reduce<Record<number, AttendanceEntryDraft>>((entries, student) => {
        entries[student.studentId] = {
          studentId: student.studentId,
          status: student.status,
          remarks: student.remarks
        };

        return entries;
      }, {});

      this.draftEntries.set(draft);
    });
  }

  protected draftFor(studentId: number): AttendanceEntryDraft {
    return this.draftEntries()[studentId] ?? {
      studentId,
      status: 'Unmarked',
      remarks: null
    };
  }

  protected setStatus(studentId: number, status: string): void {
    this.draftEntries.update((entries) => ({
      ...entries,
      [studentId]: {
        ...this.draftFor(studentId),
        status
      }
    }));
  }

  protected setRemarks(studentId: number, remarks: string): void {
    this.draftEntries.update((entries) => ({
      ...entries,
      [studentId]: {
        ...this.draftFor(studentId),
        remarks: remarks.trim() || null
      }
    }));
  }

  protected markAll(status: string): void {
    const updated = Object.values(this.draftEntries()).reduce<Record<number, AttendanceEntryDraft>>((entries, student) => {
      entries[student.studentId] = {
        ...student,
        status
      };

      return entries;
    }, {});

    this.draftEntries.set(updated);
  }

  protected onClassChange(classId: number): void {
    this.selectedClassId.set(classId);
    const firstSection = this.sectionOptions()[0];
    this.selectedSectionId.set(firstSection?.id ?? null);
  }

  protected saveAttendance(attendanceDate: string): void {
    this.store.saveAttendance(attendanceDate, Object.values(this.draftEntries()));
  }

  protected loadSelectedRoster(): void {
    if (!this.canLoadSelection()) {
      return;
    }

    this.store.loadAttendanceForSelection(this.selectedDate(), this.selectedClassId(), this.selectedSectionId());
  }

  protected submitLeaveRequest(): void {
    if (!this.canSubmitLeave()) {
      return;
    }

    this.store.createLeaveRequest(this.leaveDraft);
    this.leaveDraft = {
      ...this.leaveDraft,
      reason: ''
    };
  }

  protected updateLeave(leaveRequestId: number, status: string): void {
    this.store.updateLeaveStatus(leaveRequestId, status);
  }

  protected canSubmitLeave(): boolean {
    return Boolean(this.leaveDraft.studentId && this.leaveDraft.leaveDate && this.leaveDraft.leaveType && this.leaveDraft.reason.trim());
  }
}
