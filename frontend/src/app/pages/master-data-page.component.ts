import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import {
  CreateAcademicYear,
  CreateCampus,
  CreateSchoolClass,
  CreateSection,
  AcademicYear,
  Campus,
  SchoolClass,
  Section,
  UpdateAcademicYear,
  UpdateCampus,
  UpdateSchoolClass,
  UpdateSection
} from '../app.models';

@Component({
  selector: 'app-master-data-page',
  imports: [FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Master Data</p>
      <h2>Campuses, academic years, classes, and sections</h2>
    </section>

    @if (store.academicStructure(); as structure) {
      <section class="metrics">
        <article class="metric-card">
          <span>Campuses</span>
          <strong>{{ structure.campuses.length }}</strong>
          <p>Operational campuses configured for the ERP.</p>
        </article>
        <article class="metric-card">
          <span>Academic Years</span>
          <strong>{{ structure.academicYears.length }}</strong>
          <p>Session calendars available for admissions and planning.</p>
        </article>
        <article class="metric-card">
          <span>Classes</span>
          <strong>{{ structure.classes.length }}</strong>
          <p>Grade levels mapped into the academic hierarchy.</p>
        </article>
        <article class="metric-card">
          <span>Sections</span>
          <strong>{{ structure.sections.length }}</strong>
          <p>Operational classroom sections ready for roster use.</p>
        </article>
      </section>

      <div class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Campus Desk</p>
              <h3>{{ editingCampusId() ? 'Update campus' : 'Create campus' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Code</span>
              <input type="text" [(ngModel)]="campusDraft.code" name="campusCode" />
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="campusDraft.name" name="campusName" />
            </label>
            <label class="form-field">
              <span>City</span>
              <input type="text" [(ngModel)]="campusDraft.city" name="campusCity" />
            </label>
            <label class="form-field">
              <span>State</span>
              <input type="text" [(ngModel)]="campusDraft.state" name="campusState" />
            </label>
            <label class="form-field">
              <span>Country</span>
              <input type="text" [(ngModel)]="campusDraft.country" name="campusCountry" />
            </label>
            <label class="form-field">
              <span>Board</span>
              <input type="text" [(ngModel)]="campusDraft.boardAffiliation" name="campusBoard" />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitCampus()">
                {{ editingCampusId() ? 'Update Campus' : 'Create Campus' }}
              </button>
              @if (editingCampusId()) {
                <button type="button" class="button button--secondary" (click)="resetCampusDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Academic Year Desk</p>
              <h3>{{ editingAcademicYearId() ? 'Update academic year' : 'Create academic year' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Campus</span>
              <select [(ngModel)]="academicYearDraft.campusId" name="academicYearCampusId">
                <option [ngValue]="0">Select campus</option>
                @for (campus of structure.campuses; track campus.id) {
                  <option [ngValue]="campus.id">{{ campus.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="academicYearDraft.name" name="academicYearName" />
            </label>
            <label class="form-field">
              <span>Start Date</span>
              <input type="date" [(ngModel)]="academicYearDraft.startDate" name="academicYearStartDate" />
            </label>
            <label class="form-field">
              <span>End Date</span>
              <input type="date" [(ngModel)]="academicYearDraft.endDate" name="academicYearEndDate" />
            </label>
            <label class="form-field">
              <span>Status</span>
              <select [(ngModel)]="academicYearDraft.isActive" name="academicYearIsActive">
                <option [ngValue]="true">Active</option>
                <option [ngValue]="false">Inactive</option>
              </select>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitAcademicYear()">
                {{ editingAcademicYearId() ? 'Update Academic Year' : 'Create Academic Year' }}
              </button>
              @if (editingAcademicYearId()) {
                <button type="button" class="button button--secondary" (click)="resetAcademicYearDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Class Desk</p>
              <h3>{{ editingSchoolClassId() ? 'Update class' : 'Create class' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Campus</span>
              <select [(ngModel)]="schoolClassDraft.campusId" name="schoolClassCampusId">
                <option [ngValue]="0">Select campus</option>
                @for (campus of structure.campuses; track campus.id) {
                  <option [ngValue]="campus.id">{{ campus.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Code</span>
              <input type="text" [(ngModel)]="schoolClassDraft.code" name="schoolClassCode" />
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="schoolClassDraft.name" name="schoolClassName" />
            </label>
            <label class="form-field">
              <span>Display Order</span>
              <input type="number" [(ngModel)]="schoolClassDraft.displayOrder" name="schoolClassDisplayOrder" min="1" />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitSchoolClass()">
                {{ editingSchoolClassId() ? 'Update Class' : 'Create Class' }}
              </button>
              @if (editingSchoolClassId()) {
                <button type="button" class="button button--secondary" (click)="resetSchoolClassDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Section Desk</p>
              <h3>{{ editingSectionId() ? 'Update section' : 'Create section' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Class</span>
              <select [(ngModel)]="sectionDraft.schoolClassId" name="sectionClassId">
                <option [ngValue]="0">Select class</option>
                @for (schoolClass of structure.classes; track schoolClass.id) {
                  <option [ngValue]="schoolClass.id">{{ schoolClass.name }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="sectionDraft.name" name="sectionName" />
            </label>
            <label class="form-field">
              <span>Capacity</span>
              <input type="number" [(ngModel)]="sectionDraft.capacity" name="sectionCapacity" min="1" />
            </label>
            <label class="form-field">
              <span>Room Number</span>
              <input type="text" [(ngModel)]="sectionDraft.roomNumber" name="sectionRoomNumber" />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitSection()">
                {{ editingSectionId() ? 'Update Section' : 'Create Section' }}
              </button>
              @if (editingSectionId()) {
                <button type="button" class="button button--secondary" (click)="resetSectionDraft()">Cancel</button>
              }
            </div>
          </div>
          @if (feedbackMessage(); as message) {
            <div class="notice" [class.notice--error]="feedbackTone() === 'error'">
              <strong>Master data update</strong>
              <p>{{ message }}</p>
            </div>
          }
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Campuses</p>
              <h3>Operational locations</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (campus of structure.campuses; track campus.id) {
              <article class="guardian-item">
                <div class="guardian-item__main">
                  <div>
                    <strong>{{ campus.name }}</strong>
                    <p>{{ campus.code }} · {{ campus.city }}, {{ campus.state }}</p>
                  </div>
                  <div class="form-actions">
                    <button type="button" class="button button--small button--secondary" (click)="editCampus(campus)">Edit</button>
                    <button type="button" class="button button--small button--danger" (click)="deleteCampus(campus.id)">Delete</button>
                  </div>
                </div>
                <span>{{ campus.boardAffiliation }}</span>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Academic Years</p>
              <h3>Session calendars</h3>
            </div>
          </div>
          <div class="application-list">
            @for (academicYear of structure.academicYears; track academicYear.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ academicYear.name }}</strong>
                    <p>{{ campusName(academicYear.campusId) }}</p>
                  </div>
                  <span class="status-chip" [class]="academicYear.isActive ? 'status-chip status-chip--active' : 'status-chip status-chip--inactive'">
                    {{ academicYear.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </div>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editAcademicYear(academicYear)">Edit</button>
                  <button type="button" class="button button--small button--danger" (click)="deleteAcademicYear(academicYear.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Classes</p>
              <h3>Grade structure</h3>
            </div>
          </div>
          <div class="application-list">
            @for (schoolClass of structure.classes; track schoolClass.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ schoolClass.name }}</strong>
                    <p>{{ schoolClass.code }} · {{ campusName(schoolClass.campusId) }}</p>
                  </div>
                  <span>#{{ schoolClass.displayOrder }}</span>
                </div>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editSchoolClass(schoolClass)">Edit</button>
                  <button type="button" class="button button--small button--danger" (click)="deleteSchoolClass(schoolClass.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Sections</p>
              <h3>Classroom allocation</h3>
            </div>
          </div>
          <div class="application-list">
            @for (section of structure.sections; track section.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ section.schoolClassName }} / {{ section.name }}</strong>
                    <p>{{ section.roomNumber }}</p>
                  </div>
                  <span>{{ section.capacity }} seats</span>
                </div>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--small button--secondary" (click)="editSection(section)">Edit</button>
                  <button type="button" class="button button--small button--danger" (click)="deleteSection(section.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>
      </div>
    }
  `
})
export class MasterDataPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly editingCampusId = signal<number | null>(null);
  protected readonly editingAcademicYearId = signal<number | null>(null);
  protected readonly editingSchoolClassId = signal<number | null>(null);
  protected readonly editingSectionId = signal<number | null>(null);
  protected readonly campuses = computed(() => this.store.academicStructure()?.campuses ?? []);
  protected readonly schoolClasses = computed(() => this.store.academicStructure()?.classes ?? []);

  protected campusDraft: CreateCampus = this.createEmptyCampusDraft();
  protected academicYearDraft: CreateAcademicYear = this.createEmptyAcademicYearDraft();
  protected schoolClassDraft: CreateSchoolClass = this.createEmptySchoolClassDraft();
  protected sectionDraft: CreateSection = this.createEmptySectionDraft();

  protected submitCampus(): void {
    if (!this.campusDraft.code || !this.campusDraft.name) {
      return;
    }

    const request = this.editingCampusId()
      ? this.store.updateCampus(this.editingCampusId()!, this.toUpdateCampus(this.campusDraft))
      : this.store.createCampus(this.campusDraft);

    this.handleMutation(request, this.editingCampusId() ? 'Campus updated successfully.' : 'Campus created successfully.', () => this.resetCampusDraft());
  }

  protected submitAcademicYear(): void {
    if (!this.academicYearDraft.campusId || !this.academicYearDraft.name) {
      return;
    }

    const request = this.editingAcademicYearId()
      ? this.store.updateAcademicYear(this.editingAcademicYearId()!, this.toUpdateAcademicYear(this.academicYearDraft))
      : this.store.createAcademicYear(this.academicYearDraft);

    this.handleMutation(request, this.editingAcademicYearId() ? 'Academic year updated successfully.' : 'Academic year created successfully.', () => this.resetAcademicYearDraft());
  }

  protected submitSchoolClass(): void {
    if (!this.schoolClassDraft.campusId || !this.schoolClassDraft.code || !this.schoolClassDraft.name) {
      return;
    }

    const request = this.editingSchoolClassId()
      ? this.store.updateSchoolClass(this.editingSchoolClassId()!, this.toUpdateSchoolClass(this.schoolClassDraft))
      : this.store.createSchoolClass(this.schoolClassDraft);

    this.handleMutation(request, this.editingSchoolClassId() ? 'Class updated successfully.' : 'Class created successfully.', () => this.resetSchoolClassDraft());
  }

  protected submitSection(): void {
    if (!this.sectionDraft.schoolClassId || !this.sectionDraft.name) {
      return;
    }

    const request = this.editingSectionId()
      ? this.store.updateSection(this.editingSectionId()!, this.toUpdateSection(this.sectionDraft))
      : this.store.createSection(this.sectionDraft);

    this.handleMutation(request, this.editingSectionId() ? 'Section updated successfully.' : 'Section created successfully.', () => this.resetSectionDraft());
  }

  protected editCampus(campus: Campus): void {
    this.editingCampusId.set(campus.id);
    this.campusDraft = {
      code: campus.code,
      name: campus.name,
      city: campus.city,
      state: campus.state,
      country: campus.country,
      boardAffiliation: campus.boardAffiliation
    };
  }

  protected editAcademicYear(academicYear: AcademicYear): void {
    this.editingAcademicYearId.set(academicYear.id);
    this.academicYearDraft = {
      campusId: academicYear.campusId,
      name: academicYear.name,
      startDate: academicYear.startDate,
      endDate: academicYear.endDate,
      isActive: academicYear.isActive
    };
  }

  protected editSchoolClass(schoolClass: SchoolClass): void {
    this.editingSchoolClassId.set(schoolClass.id);
    this.schoolClassDraft = {
      campusId: schoolClass.campusId,
      code: schoolClass.code,
      name: schoolClass.name,
      displayOrder: schoolClass.displayOrder
    };
  }

  protected editSection(section: Section): void {
    this.editingSectionId.set(section.id);
    this.sectionDraft = {
      schoolClassId: section.schoolClassId,
      name: section.name,
      capacity: section.capacity,
      roomNumber: section.roomNumber
    };
  }

  protected deleteCampus(campusId: number): void {
    this.handleDelete(this.store.deleteCampus(campusId), 'Campus deleted successfully.', () => {
      if (this.editingCampusId() === campusId) {
        this.resetCampusDraft();
      }
    });
  }

  protected deleteAcademicYear(academicYearId: number): void {
    this.handleDelete(this.store.deleteAcademicYear(academicYearId), 'Academic year deleted successfully.', () => {
      if (this.editingAcademicYearId() === academicYearId) {
        this.resetAcademicYearDraft();
      }
    });
  }

  protected deleteSchoolClass(schoolClassId: number): void {
    this.handleDelete(this.store.deleteSchoolClass(schoolClassId), 'Class deleted successfully.', () => {
      if (this.editingSchoolClassId() === schoolClassId) {
        this.resetSchoolClassDraft();
      }
    });
  }

  protected deleteSection(sectionId: number): void {
    this.handleDelete(this.store.deleteSection(sectionId), 'Section deleted successfully.', () => {
      if (this.editingSectionId() === sectionId) {
        this.resetSectionDraft();
      }
    });
  }

  protected resetCampusDraft(): void {
    this.editingCampusId.set(null);
    this.campusDraft = this.createEmptyCampusDraft();
  }

  protected resetAcademicYearDraft(): void {
    this.editingAcademicYearId.set(null);
    this.academicYearDraft = this.createEmptyAcademicYearDraft();
  }

  protected resetSchoolClassDraft(): void {
    this.editingSchoolClassId.set(null);
    this.schoolClassDraft = this.createEmptySchoolClassDraft();
  }

  protected resetSectionDraft(): void {
    this.editingSectionId.set(null);
    this.sectionDraft = this.createEmptySectionDraft();
  }

  protected campusName(campusId: number): string {
    return this.campuses().find((campus) => campus.id === campusId)?.name ?? 'Unknown campus';
  }

  private handleMutation<T>(request: import('rxjs').Observable<T>, message: string, onSuccess: () => void): void {
    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(message);
        onSuccess();
        this.store.loadAcademicStructure();
      },
      error: () => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Master data changes could not be saved.');
      }
    });
  }

  private handleDelete(request: import('rxjs').Observable<void>, message: string, onSuccess: () => void): void {
    request.subscribe({
      next: () => {
        this.feedbackTone.set('success');
        this.feedbackMessage.set(message);
        onSuccess();
        this.store.loadAcademicStructure();
      },
      error: () => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Master data record could not be deleted.');
      }
    });
  }

  private createEmptyCampusDraft(): CreateCampus {
    return {
      code: '',
      name: '',
      city: '',
      state: '',
      country: 'India',
      boardAffiliation: 'CBSE'
    };
  }

  private createEmptyAcademicYearDraft(): CreateAcademicYear {
    return {
      campusId: this.campuses()[0]?.id ?? 0,
      name: '',
      startDate: '',
      endDate: '',
      isActive: true
    };
  }

  private createEmptySchoolClassDraft(): CreateSchoolClass {
    return {
      campusId: this.campuses()[0]?.id ?? 0,
      code: '',
      name: '',
      displayOrder: (this.store.academicStructure()?.classes.length ?? 0) + 1
    };
  }

  private createEmptySectionDraft(): CreateSection {
    return {
      schoolClassId: this.schoolClasses()[0]?.id ?? 0,
      name: '',
      capacity: 30,
      roomNumber: ''
    };
  }

  private toUpdateCampus(draft: CreateCampus): UpdateCampus {
    return { ...draft };
  }

  private toUpdateAcademicYear(draft: CreateAcademicYear): UpdateAcademicYear {
    return { ...draft };
  }

  private toUpdateSchoolClass(draft: CreateSchoolClass): UpdateSchoolClass {
    return { ...draft };
  }

  private toUpdateSection(draft: CreateSection): UpdateSection {
    return { ...draft };
  }
}
