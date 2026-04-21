import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { extractApiErrorMessage } from '../api-error.utils';
import { CreateExamSchedule, CreateExamTerm, ExamSchedule, ExamTerm, UpdateExamSchedule, UpdateExamTerm } from '../app.models';

@Component({
  selector: 'app-examinations-page',
  imports: [FormsModule, DatePipe],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Examinations Module</p>
      <h2>Exam terms, schedules, and report cards</h2>
    </section>

    @if (store.examinationsDashboard(); as examinations) {
      <section class="metrics">
        <article class="metric-card">
          <span>Exam Term</span>
          <strong>{{ examinations.selectedExamTermName }}</strong>
          <p>Current assessment window in focus.</p>
        </article>
        <article class="metric-card">
          <span>Scheduled Papers</span>
          <strong>{{ examinations.schedule.length }}</strong>
          <p>Subject papers configured for this roster.</p>
        </article>
        <article class="metric-card">
          <span>Report Cards</span>
          <strong>{{ examinations.reportCards.length }}</strong>
          <p>Students with computed results in this exam term.</p>
        </article>
        <article class="metric-card">
          <span>Average Result</span>
          <strong>{{ averagePercentage(examinations.reportCards) }}%</strong>
          <p>Overall class performance snapshot.</p>
        </article>
      </section>

      <div class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Exam Filters</p>
              <h3>Choose term, class, and section</h3>
            </div>
          </div>
          <div class="attendance-filter-grid">
            <label class="attendance-remark-field attendance-remark-field--compact">
              <span>Exam Term</span>
              <select [ngModel]="selectedExamTermId()" (ngModelChange)="selectedExamTermId.set($event)">
                @for (term of examinations.examTerms; track term.id) {
                  <option [ngValue]="term.id">{{ term.name }}</option>
                }
              </select>
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
            <button type="button" class="button button--secondary" [disabled]="!canLoad()" (click)="loadExaminations()">
              Load exam desk
            </button>
          </div>
          @if (feedbackMessage(); as message) {
            <div class="notice" [class.notice--error]="feedbackTone() === 'error'">
              <strong>Examinations update</strong>
              <p>{{ message }}</p>
            </div>
          }
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Exam Term Desk</p>
              <h3>{{ editingExamTermId() ? 'Update exam term' : 'Create assessment window' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Campus</span>
              <select [(ngModel)]="examTermDraft.campusId" name="examCampusId">
                <option [ngValue]="0">Select campus</option>
                @for (campus of campusOptions(); track campus.id) {
                  <option [ngValue]="campus.id">{{ campus.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Academic Year</span>
              <select [(ngModel)]="examTermDraft.academicYearId" name="examAcademicYearId">
                <option [ngValue]="0">Select year</option>
                @for (year of yearOptions(); track year.id) {
                  <option [ngValue]="year.id">{{ year.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Term Name</span>
              <input type="text" [(ngModel)]="examTermDraft.name" name="examTermName" />
            </label>
            <label class="form-field">
              <span>Exam Type</span>
              <select [(ngModel)]="examTermDraft.examType" name="examType">
                @for (type of examTypes; track type) {
                  <option [value]="type">{{ type }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Start Date</span>
              <input type="date" [(ngModel)]="examTermDraft.startDate" name="examStartDate" />
            </label>
            <label class="form-field">
              <span>End Date</span>
              <input type="date" [(ngModel)]="examTermDraft.endDate" name="examEndDate" />
            </label>
            <label class="form-field">
              <span>Status</span>
              <select [(ngModel)]="examTermDraft.status" name="examStatus">
                @for (status of examStatuses; track status) {
                  <option [value]="status">{{ status }}</option>
                }
              </select>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitExamTerm()">
                {{ editingExamTermId() ? 'Update Exam Term' : 'Create Exam Term' }}
              </button>
              @if (editingExamTermId()) {
                <button type="button" class="button button--secondary" (click)="resetExamTermDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Schedule Desk</p>
              <h3>{{ editingExamScheduleId() ? 'Update exam paper' : 'Add subject-wise exam plan' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Exam Term</span>
              <select [(ngModel)]="examScheduleDraft.examTermId" name="scheduleExamTermId">
                <option [ngValue]="0">Select term</option>
                @for (term of examinations.examTerms; track term.id) {
                  <option [ngValue]="term.id">{{ term.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Class</span>
              <select [(ngModel)]="examScheduleDraft.classId" name="scheduleClassId">
                <option [ngValue]="0">Select class</option>
                @for (schoolClass of classOptions(); track schoolClass.id) {
                  <option [ngValue]="schoolClass.id">{{ schoolClass.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Section</span>
              <select [(ngModel)]="examScheduleDraft.sectionId" name="scheduleSectionId">
                <option [ngValue]="0">Select section</option>
                @for (section of scheduleSectionOptions(); track section.id) {
                  <option [ngValue]="section.id">{{ section.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Subject</span>
              <select [(ngModel)]="examScheduleDraft.subjectId" name="scheduleSubjectId">
                <option [ngValue]="0">Select subject</option>
                @for (subject of subjectOptions(); track subject.id) {
                  <option [ngValue]="subject.id">{{ subject.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Exam Date</span>
              <input type="date" [(ngModel)]="examScheduleDraft.examDate" name="scheduleExamDate" />
            </label>
            <label class="form-field">
              <span>Start Time</span>
              <input type="time" [(ngModel)]="examScheduleDraft.startTime" name="scheduleStartTime" />
            </label>
            <label class="form-field">
              <span>Duration</span>
              <input type="number" [(ngModel)]="examScheduleDraft.durationMinutes" name="scheduleDuration" min="1" />
            </label>
            <label class="form-field">
              <span>Max Marks</span>
              <input type="number" [(ngModel)]="examScheduleDraft.maxMarks" name="scheduleMaxMarks" min="1" />
            </label>
            <label class="form-field">
              <span>Pass Marks</span>
              <input type="number" [(ngModel)]="examScheduleDraft.passMarks" name="schedulePassMarks" min="1" />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitExamSchedule()">
                {{ editingExamScheduleId() ? 'Update Schedule' : 'Create Schedule' }}
              </button>
              @if (editingExamScheduleId()) {
                <button type="button" class="button button--secondary" (click)="resetExamScheduleDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Exam Terms</p>
              <h3>Assessment windows</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (term of examinations.examTerms; track term.id) {
              <article class="guardian-item">
                <div class="guardian-item__main">
                  <div>
                    <strong>{{ term.name }}</strong>
                    <p>{{ term.examType }}</p>
                  </div>
                  <div class="form-actions">
                    <button type="button" class="button button--small button--secondary" (click)="editExamTerm(term)">Edit</button>
                    <button type="button" class="button button--small button--danger" (click)="deleteExamTerm(term.id)">Delete</button>
                  </div>
                </div>
                <span>{{ term.startDate | date: 'dd MMM' }} to {{ term.endDate | date: 'dd MMM yyyy' }} - {{ term.status }}</span>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Schedule</p>
              <h3>Subject-wise exam plan</h3>
            </div>
          </div>
          <div class="application-list">
            @for (paper of examinations.schedule; track paper.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ paper.subjectName }}</strong>
                    <p>{{ paper.examDate | date: 'dd MMM yyyy' }} at {{ paper.startTime }}</p>
                  </div>
                  <span class="status-chip status-chip--active">{{ paper.durationMinutes }} min</span>
                </div>
                <div class="application-meta">
                  <span>Max {{ paper.maxMarks }}</span>
                  <span>Pass {{ paper.passMarks }}</span>
                </div>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editExamSchedule(paper)">Edit</button>
                  <button type="button" class="button button--small button--danger" (click)="deleteExamSchedule(paper.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card data-card--span">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Report Cards</p>
              <h3>Result snapshot by student</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (report of examinations.reportCards; track report.studentId) {
              <article class="guardian-item">
                <strong>{{ report.studentName }}</strong>
                <p>{{ report.admissionNumber }} - {{ report.percentage }}%</p>
                <span>{{ report.resultStatus }}</span>
                <div class="application-list">
                  @for (subject of report.subjectResults; track subject.subjectId) {
                    <article class="application-item">
                      <div class="application-item__main">
                        <div>
                          <strong>{{ subject.subjectName }}</strong>
                          <p>{{ subject.marksObtained }} / {{ subject.maxMarks }}</p>
                        </div>
                        <span class="status-chip status-chip--active">{{ subject.grade }}</span>
                      </div>
                      <div class="application-meta">
                        <span>Pass {{ subject.passMarks }}</span>
                        <span>{{ subject.resultStatus }}</span>
                      </div>
                    </article>
                  }
                </div>
              </article>
            }
          </div>
        </article>
      </div>
    }
  `
})
export class ExaminationsPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly selectedExamTermId = signal<number | null>(null);
  protected readonly selectedClassId = signal<number | null>(null);
  protected readonly selectedSectionId = signal<number | null>(null);
  protected readonly editingExamTermId = signal<number | null>(null);
  protected readonly editingExamScheduleId = signal<number | null>(null);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );
  protected readonly scheduleSectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.examScheduleDraft.classId)
  );
  protected readonly yearOptions = computed(() => this.store.academicStructure()?.academicYears ?? []);
  protected readonly campusOptions = computed(() => this.store.academicStructure()?.campuses ?? []);
  protected readonly subjectOptions = computed(() => this.store.academicsDashboard()?.subjects ?? []);
  protected readonly examTypes = ['Scholastic', 'Practical', 'Oral', 'Unit Test'];
  protected readonly examStatuses = ['Draft', 'Scheduled', 'Completed', 'Published'];
  protected readonly canLoad = computed(() => Boolean(this.selectedExamTermId() && this.selectedClassId() && this.selectedSectionId()));

  protected examTermDraft: CreateExamTerm = this.createEmptyExamTermDraft();
  protected examScheduleDraft: CreateExamSchedule = this.createEmptyExamScheduleDraft();

  constructor() {
    effect(() => {
      const examinations = this.store.examinationsDashboard();

      if (!examinations) {
        return;
      }

      this.selectedExamTermId.set(examinations.selectedExamTermId);
      this.selectedClassId.set(examinations.selectedClassId);
      this.selectedSectionId.set(examinations.selectedSectionId);

      if (!this.examScheduleDraft.examTermId) {
        this.examScheduleDraft.examTermId = examinations.selectedExamTermId;
      }

      if (!this.examScheduleDraft.classId) {
        this.examScheduleDraft.classId = examinations.selectedClassId;
      }

      if (!this.examScheduleDraft.sectionId) {
        this.examScheduleDraft.sectionId = examinations.selectedSectionId;
      }
    });
  }

  protected onClassChange(classId: number): void {
    this.selectedClassId.set(classId);
    const firstSection = this.sectionOptions()[0];
    this.selectedSectionId.set(firstSection?.id ?? null);
  }

  protected loadExaminations(): void {
    this.store.loadExaminationsDashboard(this.selectedExamTermId(), this.selectedClassId(), this.selectedSectionId());
  }

  protected submitExamTerm(): void {
    if (!this.examTermDraft.campusId || !this.examTermDraft.academicYearId || !this.examTermDraft.name) {
      return;
    }

    const request = this.editingExamTermId()
      ? this.store.updateExamTerm(this.editingExamTermId()!, this.toUpdateExamTerm(this.examTermDraft))
      : this.store.createExamTerm(this.examTermDraft);

    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(this.editingExamTermId() ? 'Exam term updated successfully.' : 'Exam term created successfully.');
        this.resetExamTermDraft();
        this.store.loadExaminationsDashboard(this.selectedExamTermId(), this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Exam term could not be saved.'));
      }
    });
  }

  protected editExamTerm(term: ExamTerm): void {
    this.editingExamTermId.set(term.id);
    this.examTermDraft = {
      campusId: this.campusOptions()[0]?.id ?? 0,
      academicYearId: this.yearOptions()[0]?.id ?? 0,
      name: term.name,
      examType: term.examType,
      startDate: term.startDate,
      endDate: term.endDate,
      status: term.status
    };
  }

  protected deleteExamTerm(examTermId: number): void {
    this.store.deleteExamTerm(examTermId).subscribe({
      next: () => {
        if (this.editingExamTermId() === examTermId) {
          this.resetExamTermDraft();
        }

        this.feedbackTone.set('success');
        this.feedbackMessage.set('Exam term deleted successfully.');
        this.store.loadExaminationsDashboard(this.selectedExamTermId(), this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Exam term could not be deleted.'));
      }
    });
  }

  protected submitExamSchedule(): void {
    if (!this.examScheduleDraft.examTermId || !this.examScheduleDraft.classId || !this.examScheduleDraft.sectionId || !this.examScheduleDraft.subjectId) {
      return;
    }

    const request = this.editingExamScheduleId()
      ? this.store.updateExamSchedule(this.editingExamScheduleId()!, this.toUpdateExamSchedule(this.examScheduleDraft))
      : this.store.createExamSchedule(this.examScheduleDraft);

    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(this.editingExamScheduleId() ? 'Exam schedule updated successfully.' : 'Exam schedule created successfully.');
        this.resetExamScheduleDraft();
        this.store.loadExaminationsDashboard(this.selectedExamTermId(), this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Exam schedule could not be saved.'));
      }
    });
  }

  protected editExamSchedule(schedule: ExamSchedule): void {
    this.editingExamScheduleId.set(schedule.id);
    this.examScheduleDraft = {
      examTermId: schedule.examTermId,
      classId: schedule.classId,
      sectionId: schedule.sectionId,
      subjectId: schedule.subjectId,
      examDate: schedule.examDate,
      startTime: schedule.startTime,
      durationMinutes: schedule.durationMinutes,
      maxMarks: schedule.maxMarks,
      passMarks: schedule.passMarks
    };
  }

  protected deleteExamSchedule(examScheduleId: number): void {
    this.store.deleteExamSchedule(examScheduleId).subscribe({
      next: () => {
        if (this.editingExamScheduleId() === examScheduleId) {
          this.resetExamScheduleDraft();
        }

        this.feedbackTone.set('success');
        this.feedbackMessage.set('Exam schedule deleted successfully.');
        this.store.loadExaminationsDashboard(this.selectedExamTermId(), this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Exam schedule could not be deleted.'));
      }
    });
  }

  protected averagePercentage(reports: { percentage: number }[]): number {
    if (reports.length === 0) {
      return 0;
    }

    const total = reports.reduce((sum, report) => sum + report.percentage, 0);
    return Math.round((total / reports.length) * 10) / 10;
  }

  protected resetExamTermDraft(): void {
    this.editingExamTermId.set(null);
    this.examTermDraft = this.createEmptyExamTermDraft();
  }

  protected resetExamScheduleDraft(): void {
    this.editingExamScheduleId.set(null);
    this.examScheduleDraft = this.createEmptyExamScheduleDraft();
  }

  private createEmptyExamTermDraft(): CreateExamTerm {
    return {
      campusId: this.campusOptions()[0]?.id ?? 0,
      academicYearId: this.yearOptions()[0]?.id ?? 0,
      name: '',
      examType: 'Scholastic',
      startDate: '',
      endDate: '',
      status: 'Draft'
    };
  }

  private createEmptyExamScheduleDraft(): CreateExamSchedule {
    return {
      examTermId: this.selectedExamTermId() ?? 0,
      classId: this.selectedClassId() ?? 0,
      sectionId: this.selectedSectionId() ?? 0,
      subjectId: 0,
      examDate: '',
      startTime: '09:00',
      durationMinutes: 90,
      maxMarks: 100,
      passMarks: 35
    };
  }

  private toUpdateExamTerm(draft: CreateExamTerm): UpdateExamTerm {
    return { ...draft };
  }

  private toUpdateExamSchedule(draft: CreateExamSchedule): UpdateExamSchedule {
    return { ...draft };
  }
}
