import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';

@Component({
  selector: 'app-academics-page',
  imports: [FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Academics Module</p>
      <h2>Subject masters and class timetable planning</h2>
    </section>

    @if (store.academicsDashboard(); as academics) {
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
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>{{ academics.selectedClassName }} / {{ academics.selectedSectionName }}</strong>
              <p>{{ academics.subjectCount }} subjects with {{ academics.weeklyPeriodsPlanned }} planned periods</p>
              <span>Weekly schedule alignment</span>
            </article>
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
                <strong>{{ subject.name }}</strong>
                <p>{{ subject.code }} - {{ subject.category }}</p>
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
export class AcademicsPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly selectedClassId = signal<number | null>(null);
  protected readonly selectedSectionId = signal<number | null>(null);
  protected readonly classOptions = computed(() => this.store.academicStructure()?.classes ?? []);
  protected readonly sectionOptions = computed(() =>
    (this.store.academicStructure()?.sections ?? []).filter((section) => section.schoolClassId === this.selectedClassId())
  );

  constructor() {
    effect(() => {
      const academics = this.store.academicsDashboard();

      if (!academics) {
        return;
      }

      this.selectedClassId.set(academics.selectedClassId);
      this.selectedSectionId.set(academics.selectedSectionId);
    });
  }

  protected onClassChange(classId: number): void {
    this.selectedClassId.set(classId);
    const firstSection = this.sectionOptions()[0];
    this.selectedSectionId.set(firstSection?.id ?? null);
  }

  protected loadAcademics(): void {
    this.store.loadAcademicsDashboard(this.selectedClassId(), this.selectedSectionId());
  }
}
