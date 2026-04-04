import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';

@Component({
  selector: 'app-examinations-page',
  imports: [FormsModule, DatePipe],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Examinations Module</p>
      <h2>Exam terms, schedules, and report cards</h2>
    </section>

    @if (store.examinationsDashboard(); as examinations) {
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
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>{{ examinations.selectedExamTermName }}</strong>
              <p>{{ examinations.selectedClassName }} / {{ examinations.selectedSectionName }}</p>
              <span>{{ examinations.schedule.length }} paper(s) scheduled</span>
            </article>
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
                <strong>{{ term.name }}</strong>
                <p>{{ term.examType }}</p>
                <span>{{ term.startDate | date: 'dd MMM' }} to {{ term.endDate | date: 'dd MMM yyyy' }}</span>
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
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );
  protected readonly canLoad = computed(() => Boolean(this.selectedExamTermId() && this.selectedClassId() && this.selectedSectionId()));

  constructor() {
    effect(() => {
      const examinations = this.store.examinationsDashboard();

      if (!examinations) {
        return;
      }

      this.selectedExamTermId.set(examinations.selectedExamTermId);
      this.selectedClassId.set(examinations.selectedClassId);
      this.selectedSectionId.set(examinations.selectedSectionId);
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
}
