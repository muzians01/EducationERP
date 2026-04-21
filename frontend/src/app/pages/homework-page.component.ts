import { DatePipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { extractApiErrorMessage } from '../api-error.utils';
import {
  CreateHomeworkAssignment,
  HomeworkAssignment,
  StudentHomeworkProgress,
  UpdateHomeworkAssignment,
  UpdateHomeworkSubmission
} from '../app.models';

@Component({
  selector: 'app-homework-page',
  imports: [FormsModule, DatePipe],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Homework Module</p>
      <h2>Assignments, student submissions, and completion tracking</h2>
    </section>

    @if (store.homeworkDashboard(); as dashboard) {
      <section class="metrics">
        <article class="metric-card">
          <span>Selected Roster</span>
          <strong>{{ dashboard.selectedClassName }} / {{ dashboard.selectedSectionName }}</strong>
          <p>Class and section currently in focus for homework operations.</p>
        </article>
        <article class="metric-card">
          <span>Active Assignments</span>
          <strong>{{ dashboard.activeAssignments }}</strong>
          <p>Homework tasks currently planned for this roster.</p>
        </article>
        <article class="metric-card">
          <span>Pending Submissions</span>
          <strong>{{ dashboard.pendingSubmissions }}</strong>
          <p>Students still waiting to submit assigned work.</p>
        </article>
        <article class="metric-card">
          <span>Completion Rate</span>
          <strong>{{ completionRate(dashboard.progress) }}%</strong>
          <p>Share of tracked submissions marked as completed.</p>
        </article>
      </section>

      <div class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Homework Filters</p>
              <h3>Choose a class and section</h3>
            </div>
          </div>
          <div class="attendance-filter-grid">
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
            <button type="button" class="button button--secondary" [disabled]="!selectedClassId() || !selectedSectionId()" (click)="loadHomework()">
              Load homework desk
            </button>
          </div>
          @if (feedbackMessage(); as message) {
            <div class="notice" [class.notice--error]="feedbackTone() === 'error'">
              <strong>Homework update</strong>
              <p>{{ message }}</p>
            </div>
          }
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Assignment Desk</p>
              <h3>{{ editingAssignmentId() ? 'Update homework assignment' : 'Create a new assignment' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Class</span>
              <select [(ngModel)]="assignmentDraft.classId" name="homeworkClassId">
                <option [ngValue]="0">Select class</option>
                @for (schoolClass of classOptions(); track schoolClass.id) {
                  <option [ngValue]="schoolClass.id">{{ schoolClass.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Section</span>
              <select [(ngModel)]="assignmentDraft.sectionId" name="homeworkSectionId">
                <option [ngValue]="0">Select section</option>
                @for (section of assignmentSectionOptions(); track section.id) {
                  <option [ngValue]="section.id">{{ section.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Subject</span>
              <select [(ngModel)]="assignmentDraft.subjectId" name="homeworkSubjectId">
                <option [ngValue]="0">Select subject</option>
                @for (subject of subjectOptions(); track subject.id) {
                  <option [ngValue]="subject.id">{{ subject.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Assigned On</span>
              <input type="date" [(ngModel)]="assignmentDraft.assignedOn" name="assignedOn" />
            </label>
            <label class="form-field">
              <span>Due On</span>
              <input type="date" [(ngModel)]="assignmentDraft.dueOn" name="dueOn" />
            </label>
            <label class="form-field">
              <span>Assigned By</span>
              <input type="text" [(ngModel)]="assignmentDraft.assignedBy" name="assignedBy" />
            </label>
            <label class="form-field form-field--full">
              <span>Title</span>
              <input type="text" [(ngModel)]="assignmentDraft.title" name="title" />
            </label>
            <label class="form-field form-field--full">
              <span>Instructions</span>
              <textarea rows="4" [(ngModel)]="assignmentDraft.instructions" name="instructions"></textarea>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitAssignment()">
                {{ editingAssignmentId() ? 'Update Assignment' : 'Create Assignment' }}
              </button>
              @if (editingAssignmentId()) {
                <button type="button" class="button button--secondary" (click)="resetAssignmentDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Submission Desk</p>
              <h3>{{ editingSubmissionKey() ? 'Update student submission' : 'Select a progress row to update' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Assignment</span>
              <input type="text" [ngModel]="submissionAssignmentLabel()" name="submissionAssignmentLabel" readonly />
            </label>
            <label class="form-field">
              <span>Student</span>
              <input type="text" [ngModel]="submissionStudentLabel()" name="submissionStudentLabel" readonly />
            </label>
            <label class="form-field">
              <span>Status</span>
              <select [(ngModel)]="submissionDraft.status" name="submissionStatus">
                @for (status of submissionStatuses; track status) {
                  <option [value]="status">{{ status }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Submitted On</span>
              <input type="date" [(ngModel)]="submissionDraft.submittedOn" name="submittedOn" />
            </label>
            <label class="form-field form-field--full">
              <span>Remarks</span>
              <textarea rows="3" [(ngModel)]="submissionDraft.remarks" name="submissionRemarks"></textarea>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" [disabled]="!editingSubmissionKey()" (click)="submitSubmissionUpdate()">
                Update Submission
              </button>
              @if (editingSubmissionKey()) {
                <button type="button" class="button button--secondary" (click)="resetSubmissionDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Assignments</p>
              <h3>Current homework plan</h3>
            </div>
          </div>
          <div class="application-list">
            @for (assignment of dashboard.assignments; track assignment.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ assignment.title }}</strong>
                    <p>{{ assignment.subjectName }} - {{ assignment.className }} / {{ assignment.sectionName }}</p>
                  </div>
                  <span>{{ assignment.dueOn | date: 'dd MMM yyyy' }}</span>
                </div>
                <div class="application-meta">
                  <span>Assigned {{ assignment.assignedOn | date: 'dd MMM' }}</span>
                  <span>{{ assignment.assignedBy }}</span>
                </div>
                <p>{{ assignment.instructions }}</p>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editAssignment(assignment)">Edit</button>
                  <button type="button" class="button button--small button--danger" (click)="deleteAssignment(assignment.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card data-card--span">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Submission Progress</p>
              <h3>Student-wise homework tracking</h3>
            </div>
          </div>
          <div class="application-list">
            @for (entry of dashboard.progress; track progressKey(entry)) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ entry.studentName }}</strong>
                    <p>{{ entry.homeworkTitle }}</p>
                  </div>
                  <span class="status-chip status-chip--active">{{ entry.status }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ entry.admissionNumber }}</span>
                  <span>Due {{ entry.dueOn | date: 'dd MMM yyyy' }}</span>
                  <span>{{ entry.submittedOn ? ('Submitted ' + (entry.submittedOn | date: 'dd MMM')) : 'Awaiting submission' }}</span>
                </div>
                @if (entry.remarks) {
                  <p>{{ entry.remarks }}</p>
                }
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editSubmission(entry)">Update</button>
                </div>
              </article>
            }
          </div>
        </article>
      </div>
    } @else {
      <section class="notice notice--info">
        <strong>Homework details are loading.</strong>
        <p>Wait briefly while the module data is fetched.</p>
      </section>
    }
  `
})
export class HomeworkPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly selectedClassId = signal<number | null>(null);
  protected readonly selectedSectionId = signal<number | null>(null);
  protected readonly editingAssignmentId = signal<number | null>(null);
  protected readonly editingSubmissionKey = signal<string | null>(null);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );
  protected readonly assignmentSectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.assignmentDraft.classId)
  );
  protected readonly subjectOptions = computed(() => this.store.academicsDashboard()?.subjects ?? []);
  protected readonly submissionStatuses = ['Pending', 'Submitted', 'Reviewed', 'Completed', 'Overdue'];

  protected assignmentDraft: CreateHomeworkAssignment = this.createEmptyAssignmentDraft();
  protected submissionDraft: UpdateHomeworkSubmission = this.createEmptySubmissionDraft();

  constructor() {
    effect(() => {
      const dashboard = this.store.homeworkDashboard();

      if (!dashboard) {
        return;
      }

      this.selectedClassId.set(dashboard.selectedClassId);
      this.selectedSectionId.set(dashboard.selectedSectionId);

      if (!this.assignmentDraft.classId) {
        this.assignmentDraft.classId = dashboard.selectedClassId;
      }

      if (!this.assignmentDraft.sectionId) {
        this.assignmentDraft.sectionId = dashboard.selectedSectionId;
      }
    });
  }

  protected onClassChange(classId: number): void {
    this.selectedClassId.set(classId);
    const firstSection = this.sectionOptions()[0];
    this.selectedSectionId.set(firstSection?.id ?? null);
  }

  protected loadHomework(): void {
    this.store.loadHomeworkDashboard(this.selectedClassId(), this.selectedSectionId());
  }

  protected submitAssignment(): void {
    if (!this.assignmentDraft.classId || !this.assignmentDraft.sectionId || !this.assignmentDraft.subjectId || !this.assignmentDraft.title) {
      return;
    }

    const request = this.editingAssignmentId()
      ? this.store.updateHomeworkAssignment(this.editingAssignmentId()!, this.toUpdateHomeworkAssignment(this.assignmentDraft))
      : this.store.createHomeworkAssignment(this.assignmentDraft);

    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(this.editingAssignmentId() ? 'Homework assignment updated successfully.' : 'Homework assignment created successfully.');
        this.resetAssignmentDraft();
        this.store.loadHomeworkDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Homework assignment could not be saved.'));
      }
    });
  }

  protected editAssignment(assignment: HomeworkAssignment): void {
    this.editingAssignmentId.set(assignment.id);
    this.assignmentDraft = {
      classId: assignment.classId,
      sectionId: assignment.sectionId,
      subjectId: assignment.subjectId,
      assignedOn: assignment.assignedOn,
      dueOn: assignment.dueOn,
      title: assignment.title,
      instructions: assignment.instructions,
      assignedBy: assignment.assignedBy
    };
  }

  protected deleteAssignment(homeworkAssignmentId: number): void {
    this.store.deleteHomeworkAssignment(homeworkAssignmentId).subscribe({
      next: () => {
        if (this.editingAssignmentId() === homeworkAssignmentId) {
          this.resetAssignmentDraft();
        }

        this.feedbackTone.set('success');
        this.feedbackMessage.set('Homework assignment deleted successfully.');
        this.store.loadHomeworkDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Homework assignment could not be deleted.'));
      }
    });
  }

  protected editSubmission(entry: StudentHomeworkProgress): void {
    this.editingSubmissionKey.set(this.progressKey(entry));
    this.submissionDraft = {
      homeworkAssignmentId: entry.homeworkAssignmentId,
      studentId: entry.studentId,
      status: entry.status,
      submittedOn: entry.submittedOn,
      remarks: entry.remarks
    };
  }

  protected submitSubmissionUpdate(): void {
    if (!this.editingSubmissionKey()) {
      return;
    }

    this.store.updateHomeworkSubmission(this.submissionDraft).subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set('Homework submission updated successfully.');
        this.resetSubmissionDraft();
        this.store.loadHomeworkDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Homework submission could not be updated.'));
      }
    });
  }

  protected resetAssignmentDraft(): void {
    this.editingAssignmentId.set(null);
    this.assignmentDraft = this.createEmptyAssignmentDraft();
  }

  protected resetSubmissionDraft(): void {
    this.editingSubmissionKey.set(null);
    this.submissionDraft = this.createEmptySubmissionDraft();
  }

  protected completionRate(progress: StudentHomeworkProgress[]): number {
    if (progress.length === 0) {
      return 0;
    }

    const completed = progress.filter((entry) => entry.status === 'Completed').length;
    return Math.round((completed / progress.length) * 100);
  }

  protected progressKey(entry: StudentHomeworkProgress): string {
    return `${entry.homeworkAssignmentId}-${entry.studentId}`;
  }

  protected submissionAssignmentLabel(): string {
    if (!this.editingSubmissionKey()) {
      return 'Select a submission row';
    }

    const entry = this.store.homeworkDashboard()?.progress.find((item) => this.progressKey(item) === this.editingSubmissionKey());
    return entry?.homeworkTitle ?? 'Selected assignment';
  }

  protected submissionStudentLabel(): string {
    if (!this.editingSubmissionKey()) {
      return 'Select a submission row';
    }

    const entry = this.store.homeworkDashboard()?.progress.find((item) => this.progressKey(item) === this.editingSubmissionKey());
    return entry ? `${entry.studentName} (${entry.admissionNumber})` : 'Selected student';
  }

  private createEmptyAssignmentDraft(): CreateHomeworkAssignment {
    const today = new Date().toISOString().slice(0, 10);
    return {
      classId: this.selectedClassId() ?? 0,
      sectionId: this.selectedSectionId() ?? 0,
      subjectId: 0,
      assignedOn: today,
      dueOn: today,
      title: '',
      instructions: '',
      assignedBy: ''
    };
  }

  private createEmptySubmissionDraft(): UpdateHomeworkSubmission {
    return {
      homeworkAssignmentId: 0,
      studentId: 0,
      status: 'Pending',
      submittedOn: null,
      remarks: null
    };
  }

  private toUpdateHomeworkAssignment(draft: CreateHomeworkAssignment): UpdateHomeworkAssignment {
    return { ...draft };
  }
}
