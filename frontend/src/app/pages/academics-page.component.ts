import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { extractApiErrorMessage } from '../api-error.utils';
import { CreateSubject, CreateTimetablePeriod, Subject, TimetablePeriod, UpdateSubject, UpdateTimetablePeriod } from '../app.models';

@Component({
  selector: 'app-academics-page',
  imports: [FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Academics Module</p>
      <h2>Subject masters and class timetable planning</h2>
    </section>

    @if (store.academicsDashboard(); as academics) {
      <section class="metrics">
        <article class="metric-card">
          <span>Selected Roster</span>
          <strong>{{ academics.selectedClassName }} / {{ academics.selectedSectionName }}</strong>
          <p>Current class and section loaded for timetable work.</p>
        </article>
        <article class="metric-card">
          <span>Subjects</span>
          <strong>{{ academics.subjectCount }}</strong>
          <p>Total subjects available for planning.</p>
        </article>
        <article class="metric-card">
          <span>Weekly Periods</span>
          <strong>{{ academics.weeklyPeriodsPlanned }}</strong>
          <p>Planned timetable slots in this roster.</p>
        </article>
        <article class="metric-card">
          <span>Days Scheduled</span>
          <strong>{{ academics.weeklyTimetable.length }}</strong>
          <p>Distinct timetable days with assigned periods.</p>
        </article>
      </section>

      <div class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Timetable Filters</p>
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
            <button type="button" class="button button--secondary" [disabled]="!selectedClassId() || !selectedSectionId()" (click)="loadAcademics()">
              Load timetable
            </button>
          </div>
          @if (feedbackMessage(); as message) {
            <div class="notice" [class.notice--error]="feedbackTone() === 'error'">
              <strong>Academics update</strong>
              <p>{{ message }}</p>
            </div>
          }
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Subject Desk</p>
              <h3>{{ editingSubjectId() ? 'Update subject' : 'Add a subject to the master list' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Campus</span>
              <select [(ngModel)]="subjectDraft.campusId" name="campusId" required>
                <option [ngValue]="0">Select campus</option>
                @for (campus of campusOptions(); track campus.id) {
                  <option [ngValue]="campus.id">{{ campus.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Code</span>
              <input type="text" [(ngModel)]="subjectDraft.code" name="code" required />
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="subjectDraft.name" name="name" required />
            </label>
            <label class="form-field">
              <span>Category</span>
              <select [(ngModel)]="subjectDraft.category" name="category">
                @for (category of subjectCategories; track category) {
                  <option [value]="category">{{ category }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Weekly Periods</span>
              <input type="number" [(ngModel)]="subjectDraft.weeklyPeriods" name="weeklyPeriods" min="1" required />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitSubject()">
                {{ editingSubjectId() ? 'Update Subject' : 'Create Subject' }}
              </button>
              @if (editingSubjectId()) {
                <button type="button" class="button button--secondary" (click)="resetSubjectDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Timetable Desk</p>
              <h3>{{ editingTimetablePeriodId() ? 'Update timetable slot' : 'Schedule a period for a class' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Academic Year</span>
              <select [(ngModel)]="timetableDraft.academicYearId" name="academicYearId" required>
                <option [ngValue]="0">Select year</option>
                @for (year of yearOptions(); track year.id) {
                  <option [ngValue]="year.id">{{ year.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Class</span>
              <select [(ngModel)]="timetableDraft.classId" name="classId" required>
                <option [ngValue]="0">Select class</option>
                @for (schoolClass of classOptions(); track schoolClass.id) {
                  <option [ngValue]="schoolClass.id">{{ schoolClass.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Section</span>
              <select [(ngModel)]="timetableDraft.sectionId" name="sectionId" required>
                <option [ngValue]="0">Select section</option>
                @for (section of timetableSectionOptions(); track section.id) {
                  <option [ngValue]="section.id">{{ section.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Subject</span>
              <select [(ngModel)]="timetableDraft.subjectId" name="subjectId" required>
                <option [ngValue]="0">Select subject</option>
                @for (subject of subjectOptions(); track subject.id) {
                  <option [ngValue]="subject.id">{{ subject.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Day</span>
              <select [(ngModel)]="timetableDraft.dayOfWeek" name="dayOfWeek">
                @for (day of dayOptions; track day) {
                  <option [value]="day">{{ day }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Period #</span>
              <input type="number" [(ngModel)]="timetableDraft.periodNumber" name="periodNumber" min="1" required />
            </label>
            <label class="form-field">
              <span>Start Time</span>
              <input type="time" [(ngModel)]="timetableDraft.startTime" name="startTime" required />
            </label>
            <label class="form-field">
              <span>End Time</span>
              <input type="time" [(ngModel)]="timetableDraft.endTime" name="endTime" required />
            </label>
            <label class="form-field">
              <span>Teacher</span>
              <input type="text" [(ngModel)]="timetableDraft.teacherName" name="teacherName" required />
            </label>
            <label class="form-field">
              <span>Room</span>
              <input type="text" [(ngModel)]="timetableDraft.roomNumber" name="roomNumber" required />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitTimetablePeriod()">
                {{ editingTimetablePeriodId() ? 'Update Timetable Slot' : 'Add Timetable Slot' }}
              </button>
              @if (editingTimetablePeriodId()) {
                <button type="button" class="button button--secondary" (click)="resetTimetableDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Subject Master</p>
              <h3>Configured academic offerings</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (subject of academics.subjects; track subject.id) {
              <article class="guardian-item">
                <div class="guardian-item__main">
                  <div>
                    <strong>{{ subject.name }}</strong>
                    <p>{{ subject.code }} - {{ subject.category }}</p>
                  </div>
                  <div class="form-actions">
                    <button type="button" class="button button--small button--secondary" (click)="editSubject(subject)">Edit</button>
                    <button type="button" class="button button--small button--danger" (click)="deleteSubject(subject.id)">Delete</button>
                  </div>
                </div>
                <span>{{ subject.weeklyPeriods }} periods / week</span>
              </article>
            }
          </div>
        </article>

        <article class="data-card data-card--span">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Weekly Timetable</p>
              <h3>Planned periods by day</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (day of academics.weeklyTimetable; track day.dayOfWeek) {
              <article class="guardian-item">
                <strong>{{ day.dayOfWeek }}</strong>
                <div class="application-list">
                  @for (period of day.periods; track period.id) {
                    <article class="application-item">
                      <div class="application-item__main">
                        <div>
                          <strong>P{{ period.periodNumber }} - {{ period.subjectName }}</strong>
                          <p>{{ period.startTime }} to {{ period.endTime }}</p>
                        </div>
                        <span class="status-chip status-chip--active">{{ period.teacherName }}</span>
                      </div>
                      <div class="application-meta">
                        <span>{{ period.subjectCode }}</span>
                        <span>{{ period.roomNumber }}</span>
                      </div>
                      <div class="form-actions form-actions--inline">
                        <button type="button" class="button button--small button--secondary" (click)="editTimetablePeriod(period)">Edit</button>
                        <button type="button" class="button button--small button--danger" (click)="deleteTimetablePeriod(period.id)">Delete</button>
                      </div>
                    </article>
                  }
                </div>
              </article>
            }
          </div>
        </article>
      </div>
    } @else {
      <section class="notice" [class.notice--error]="!!store.loadError()">
        <strong>{{ store.loadError() ? 'Academics workspace could not be loaded.' : 'Academics workspace is loading.' }}</strong>
        <p>{{ store.loadError() || 'Subjects, sections, and timetable data are being fetched now.' }}</p>
      </section>
    }
  `
})
export class AcademicsPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly selectedClassId = signal<number | null>(null);
  protected readonly selectedSectionId = signal<number | null>(null);
  protected readonly editingSubjectId = signal<number | null>(null);
  protected readonly editingTimetablePeriodId = signal<number | null>(null);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );
  protected readonly timetableSectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.timetableDraft.classId)
  );
  protected readonly yearOptions = computed(() => this.store.academicStructure()?.academicYears ?? []);
  protected readonly campusOptions = computed(() => this.store.academicStructure()?.campuses ?? []);
  protected readonly subjectOptions = computed(() => this.store.academicsDashboard()?.subjects ?? []);
  protected readonly dayOptions = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  protected readonly subjectCategories = ['Core', 'Elective', 'Lab', 'Language'];

  protected subjectDraft: CreateSubject = this.createEmptySubjectDraft();
  protected timetableDraft: CreateTimetablePeriod = this.createEmptyTimetableDraft();

  constructor() {
    if (!this.store.academicStructure()) {
      this.store.loadAcademicStructure();
    }

    if (!this.store.academicsDashboard()) {
      this.store.loadAcademicsDashboard(null, null);
    }

    effect(() => {
      const academics = this.store.academicsDashboard();

      if (!academics) {
        return;
      }

      this.selectedClassId.set(academics.selectedClassId);
      this.selectedSectionId.set(academics.selectedSectionId);

      if (!this.timetableDraft.classId) {
        this.timetableDraft.classId = academics.selectedClassId;
      }

      if (!this.timetableDraft.sectionId) {
        this.timetableDraft.sectionId = academics.selectedSectionId;
      }

      if (!this.timetableDraft.academicYearId) {
        this.timetableDraft.academicYearId = this.yearOptions()[0]?.id ?? 0;
      }
    });
  }

  protected onClassChange(classId: number): void {
    this.selectedClassId.set(classId);
    const firstSection = this.sectionOptions()[0];
    this.selectedSectionId.set(firstSection?.id ?? null);
  }

  protected submitSubject(): void {
    if (!this.subjectDraft.campusId || !this.subjectDraft.code || !this.subjectDraft.name) {
      return;
    }

    this.feedbackMessage.set(null);
    const request = this.editingSubjectId()
      ? this.store.updateSubject(this.editingSubjectId()!, this.toUpdateSubject(this.subjectDraft))
      : this.store.createSubject(this.subjectDraft);

    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(this.editingSubjectId() ? 'Subject updated successfully.' : 'Subject created successfully.');
        this.resetSubjectDraft();
        this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Subject changes could not be saved.'));
      }
    });
  }

  protected editSubject(subject: Subject): void {
    this.editingSubjectId.set(subject.id);
    this.subjectDraft = {
      campusId: this.campusOptions()[0]?.id ?? 0,
      code: subject.code,
      name: subject.name,
      category: subject.category,
      weeklyPeriods: subject.weeklyPeriods
    };
  }

  protected deleteSubject(subjectId: number): void {
    this.feedbackMessage.set(null);
    this.store.deleteSubject(subjectId).subscribe({
      next: () => {
        if (this.editingSubjectId() === subjectId) {
          this.resetSubjectDraft();
        }

        this.feedbackTone.set('success');
        this.feedbackMessage.set('Subject deleted successfully.');
        this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Subject could not be deleted.'));
      }
    });
  }

  protected submitTimetablePeriod(): void {
    if (!this.timetableDraft.academicYearId || !this.timetableDraft.classId || !this.timetableDraft.sectionId || !this.timetableDraft.subjectId) {
      return;
    }

    this.feedbackMessage.set(null);
    const request = this.editingTimetablePeriodId()
      ? this.store.updateTimetablePeriod(this.editingTimetablePeriodId()!, this.toUpdateTimetablePeriod(this.timetableDraft))
      : this.store.createTimetablePeriod(this.timetableDraft);

    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(this.editingTimetablePeriodId() ? 'Timetable slot updated successfully.' : 'Timetable slot created successfully.');
        this.resetTimetableDraft();
        this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Timetable slot could not be saved.'));
      }
    });
  }

  protected editTimetablePeriod(period: TimetablePeriod): void {
    this.editingTimetablePeriodId.set(period.id);
    this.timetableDraft = {
      academicYearId: this.yearOptions()[0]?.id ?? 0,
      classId: period.classId,
      sectionId: period.sectionId,
      subjectId: period.subjectId,
      dayOfWeek: period.dayOfWeek,
      periodNumber: period.periodNumber,
      startTime: period.startTime,
      endTime: period.endTime,
      teacherName: period.teacherName,
      roomNumber: period.roomNumber
    };
  }

  protected deleteTimetablePeriod(periodId: number): void {
    this.feedbackMessage.set(null);
    this.store.deleteTimetablePeriod(periodId).subscribe({
      next: () => {
        if (this.editingTimetablePeriodId() === periodId) {
          this.resetTimetableDraft();
        }

        this.feedbackTone.set('success');
        this.feedbackMessage.set('Timetable slot deleted successfully.');
        this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
      },
      error: (error) => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set(extractApiErrorMessage(error, 'Timetable slot could not be deleted.'));
      }
    });
  }

  protected loadAcademics(): void {
    this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
  }

  protected resetSubjectDraft(): void {
    this.editingSubjectId.set(null);
    this.subjectDraft = this.createEmptySubjectDraft();
  }

  protected resetTimetableDraft(): void {
    this.editingTimetablePeriodId.set(null);
    this.timetableDraft = this.createEmptyTimetableDraft();
  }

  private createEmptySubjectDraft(): CreateSubject {
    return {
      campusId: this.campusOptions()[0]?.id ?? 0,
      code: '',
      name: '',
      category: 'Core',
      weeklyPeriods: 3
    };
  }

  private createEmptyTimetableDraft(): CreateTimetablePeriod {
    return {
      academicYearId: this.yearOptions()[0]?.id ?? 0,
      classId: this.selectedClassId() ?? 0,
      sectionId: this.selectedSectionId() ?? 0,
      subjectId: 0,
      dayOfWeek: 'Monday',
      periodNumber: 1,
      startTime: '08:00',
      endTime: '09:00',
      teacherName: '',
      roomNumber: ''
    };
  }

  private toUpdateSubject(draft: CreateSubject): UpdateSubject {
    return {
      code: draft.code,
      name: draft.name,
      category: draft.category,
      weeklyPeriods: draft.weeklyPeriods
    };
  }

  private toUpdateTimetablePeriod(draft: CreateTimetablePeriod): UpdateTimetablePeriod {
    return {
      academicYearId: draft.academicYearId,
      classId: draft.classId,
      sectionId: draft.sectionId,
      subjectId: draft.subjectId,
      dayOfWeek: draft.dayOfWeek,
      periodNumber: draft.periodNumber,
      startTime: draft.startTime,
      endTime: draft.endTime,
      teacherName: draft.teacherName,
      roomNumber: draft.roomNumber
    };
  }
}
