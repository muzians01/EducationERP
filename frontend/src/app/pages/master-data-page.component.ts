import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import {
  AcademicYear,
  Campus,
  CreateAcademicYear,
  CreateCampus,
  CreateInstitution,
  CreateSchoolClass,
  CreateSection,
  Institution,
  SchoolClass,
  Section,
  UpdateAcademicYear,
  UpdateCampus,
  UpdateInstitution,
  UpdateSchoolClass,
  UpdateSection
} from '../app.models';

@Component({
  selector: 'app-master-data-page',
  imports: [FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Master Data</p>
      <h2>Institutions, campuses, academic years, classes, and sections</h2>
    </section>

    @if (feedbackMessage(); as message) {
      <section class="notice" [class.notice--error]="feedbackTone() === 'error'">
        <strong>{{ feedbackTone() === 'error' ? 'Master data action failed' : 'Master data updated' }}</strong>
        <p>{{ message }}</p>
      </section>
    }

    @if (store.isLoading() && !store.academicStructure()) {
      <section class="notice">
        <strong>Loading master data...</strong>
        <p>Institutions and campuses are being fetched from the API.</p>
      </section>
    }

    @if (store.loadError() && !store.academicStructure()) {
      <section class="notice notice--error">
        <strong>Master data could not be loaded.</strong>
        <p>{{ store.loadError() }}</p>
      </section>
    }

    @if (store.academicStructure(); as structure) {
      <section class="metrics">
        <article class="metric-card">
          <span>Institutions</span>
          <strong>{{ structure.institutions.length }}</strong>
          <p>Legal or management entities that own campuses.</p>
        </article>
        <article class="metric-card">
          <span>Campuses</span>
          <strong>{{ structure.campuses.length }}</strong>
          <p>Operational campuses configured under institutions.</p>
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
              <p class="eyebrow">Institution Desk</p>
              <h3>{{ editingInstitutionId() ? 'Update institution' : 'Create institution' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Code</span>
              <input type="text" [(ngModel)]="institutionDraft.code" name="institutionCode" />
            </label>
            <label class="form-field">
              <span>Name</span>
              <input type="text" [(ngModel)]="institutionDraft.name" name="institutionName" />
            </label>
            <label class="form-field">
              <span>City</span>
              <input type="text" [(ngModel)]="institutionDraft.city" name="institutionCity" />
            </label>
            <label class="form-field">
              <span>State</span>
              <input type="text" [(ngModel)]="institutionDraft.state" name="institutionState" />
            </label>
            <label class="form-field">
              <span>Country</span>
              <input type="text" [(ngModel)]="institutionDraft.country" name="institutionCountry" />
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitInstitution()">
                {{ editingInstitutionId() ? 'Update Institution' : 'Create Institution' }}
              </button>
              @if (editingInstitutionId()) {
                <button type="button" class="button button--secondary" (click)="resetInstitutionDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Campus Desk</p>
              <h3>{{ editingCampusId() ? 'Update campus' : 'Create campus' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Institution</span>
              <select [(ngModel)]="campusDraft.institutionId" name="campusInstitutionId">
                <option [ngValue]="0">Select institution</option>
                @for (institution of structure.institutions; track institution.id) {
                  <option [ngValue]="institution.id">{{ institution.name }}</option>
                }
              </select>
            </label>
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
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Institutions</p>
              <h3>Parent organizations</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (institution of structure.institutions; track institution.id) {
              <article class="guardian-item">
                <div class="guardian-item__main">
                  <div>
                    <strong>{{ institution.name }}</strong>
                    <p>{{ institution.code }} - {{ institution.city }}, {{ institution.state }}</p>
                  </div>
                  <div class="form-actions">
                    <button type="button" class="button button--small button--secondary" (click)="editInstitution(institution)">Edit</button>
                    <button type="button" class="button button--small button--danger" (click)="deleteInstitution(institution.id)">Delete</button>
                  </div>
                </div>
                <span>{{ campusesForInstitution(institution.id).length }} campus(es)</span>
                <p>{{ campusNamesForInstitution(institution.id) }}</p>
              </article>
            }
          </div>
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
                    <p>{{ campus.code }} - {{ campus.city }}, {{ campus.state }}</p>
                  </div>
                  <div class="form-actions">
                    <button type="button" class="button button--small button--secondary" (click)="editCampus(campus)">Edit</button>
                    <button type="button" class="button button--small button--danger" (click)="deleteCampus(campus.id)">Delete</button>
                  </div>
                </div>
                <span>{{ campus.institutionName }} / {{ campus.boardAffiliation }}</span>
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
                    <p>{{ schoolClass.code }} - {{ campusName(schoolClass.campusId) }}</p>
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
  protected readonly editingInstitutionId = signal<number | null>(null);
  protected readonly editingCampusId = signal<number | null>(null);
  protected readonly editingAcademicYearId = signal<number | null>(null);
  protected readonly editingSchoolClassId = signal<number | null>(null);
  protected readonly editingSectionId = signal<number | null>(null);
  protected readonly institutions = computed(() => this.store.academicStructure()?.institutions ?? []);
  protected readonly campuses = computed(() => this.store.academicStructure()?.campuses ?? []);
  protected readonly schoolClasses = computed(() => this.store.academicStructure()?.classes ?? []);

  protected institutionDraft: CreateInstitution = this.createEmptyInstitutionDraft();
  protected campusDraft: CreateCampus = this.createEmptyCampusDraft();
  protected academicYearDraft: CreateAcademicYear = this.createEmptyAcademicYearDraft();
  protected schoolClassDraft: CreateSchoolClass = this.createEmptySchoolClassDraft();
  protected sectionDraft: CreateSection = this.createEmptySectionDraft();

  constructor() {
    if (!this.store.academicStructure()) {
      this.store.loadAcademicStructure();
    }

    effect(() => {
      const institutions = this.institutions();
      const campuses = this.campuses();
      const schoolClasses = this.schoolClasses();

      if (!this.editingCampusId()) {
        this.campusDraft = this.syncCampusDraft(this.campusDraft, institutions);
      }

      if (!this.editingAcademicYearId()) {
        this.academicYearDraft = this.syncAcademicYearDraft(this.academicYearDraft, campuses);
      }

      if (!this.editingSchoolClassId()) {
        this.schoolClassDraft = this.syncSchoolClassDraft(this.schoolClassDraft, campuses);
      }

      if (!this.editingSectionId()) {
        this.sectionDraft = this.syncSectionDraft(this.sectionDraft, schoolClasses);
      }
    }, { allowSignalWrites: true });
  }

  protected submitInstitution(): void {
    if (!this.institutionDraft.code || !this.institutionDraft.name) {
      this.setFeedback('error', 'Institution code and name are required.');
      return;
    }

    const request = this.editingInstitutionId()
      ? this.store.updateInstitution(this.editingInstitutionId()!, this.toUpdateInstitution(this.institutionDraft))
      : this.store.createInstitution(this.institutionDraft);

    this.handleMutation(request, this.editingInstitutionId() ? 'Institution updated successfully.' : 'Institution created successfully.', () => this.resetInstitutionDraft());
  }

  protected submitCampus(): void {
    if (!this.campusDraft.institutionId || !this.campusDraft.code || !this.campusDraft.name) {
      this.setFeedback('error', 'Institution, campus code, and campus name are required.');
      return;
    }

    const request = this.editingCampusId()
      ? this.store.updateCampus(this.editingCampusId()!, this.toUpdateCampus(this.campusDraft))
      : this.store.createCampus(this.campusDraft);

    this.handleMutation(request, this.editingCampusId() ? 'Campus updated successfully.' : 'Campus created successfully.', () => this.resetCampusDraft());
  }

  protected submitAcademicYear(): void {
    if (!this.academicYearDraft.campusId || !this.academicYearDraft.name || !this.academicYearDraft.startDate || !this.academicYearDraft.endDate) {
      this.setFeedback('error', 'Campus, name, start date, and end date are required for an academic year.');
      return;
    }

    const request = this.editingAcademicYearId()
      ? this.store.updateAcademicYear(this.editingAcademicYearId()!, this.toUpdateAcademicYear(this.academicYearDraft))
      : this.store.createAcademicYear(this.academicYearDraft);

    this.handleMutation(request, this.editingAcademicYearId() ? 'Academic year updated successfully.' : 'Academic year created successfully.', () => this.resetAcademicYearDraft());
  }

  protected submitSchoolClass(): void {
    if (!this.schoolClassDraft.campusId || !this.schoolClassDraft.code || !this.schoolClassDraft.name) {
      this.setFeedback('error', 'Campus, class code, and class name are required.');
      return;
    }

    const request = this.editingSchoolClassId()
      ? this.store.updateSchoolClass(this.editingSchoolClassId()!, this.toUpdateSchoolClass(this.schoolClassDraft))
      : this.store.createSchoolClass(this.schoolClassDraft);

    this.handleMutation(request, this.editingSchoolClassId() ? 'Class updated successfully.' : 'Class created successfully.', () => this.resetSchoolClassDraft());
  }

  protected submitSection(): void {
    if (!this.sectionDraft.schoolClassId || !this.sectionDraft.name) {
      this.setFeedback('error', 'Class and section name are required.');
      return;
    }

    const request = this.editingSectionId()
      ? this.store.updateSection(this.editingSectionId()!, this.toUpdateSection(this.sectionDraft))
      : this.store.createSection(this.sectionDraft);

    this.handleMutation(request, this.editingSectionId() ? 'Section updated successfully.' : 'Section created successfully.', () => this.resetSectionDraft());
  }

  protected editInstitution(institution: Institution): void {
    this.editingInstitutionId.set(institution.id);
    this.institutionDraft = {
      code: institution.code,
      name: institution.name,
      city: institution.city,
      state: institution.state,
      country: institution.country
    };
  }

  protected editCampus(campus: Campus): void {
    this.editingCampusId.set(campus.id);
    this.campusDraft = {
      institutionId: campus.institutionId,
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

  protected deleteInstitution(institutionId: number): void {
    this.handleDelete(this.store.deleteInstitution(institutionId), 'Institution deleted successfully.', () => {
      if (this.editingInstitutionId() === institutionId) {
        this.resetInstitutionDraft();
      }
    });
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

  protected resetInstitutionDraft(): void {
    this.editingInstitutionId.set(null);
    this.institutionDraft = this.createEmptyInstitutionDraft();
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

  protected campusesForInstitution(institutionId: number): Campus[] {
    return this.campuses().filter((campus) => campus.institutionId === institutionId);
  }

  protected campusNamesForInstitution(institutionId: number): string {
    const names = this.campusesForInstitution(institutionId).map((campus) => campus.name);
    return names.length > 0 ? names.join(', ') : 'No campuses assigned yet.';
  }

  private handleMutation<T>(request: import('rxjs').Observable<T>, message: string, onSuccess: () => void): void {
    request.subscribe({
      next: () => {
        this.store.refreshAcademicStructure().subscribe({
          next: (structure) => {
            this.store.academicStructure.set(structure);
            onSuccess();
            this.setFeedback('success', message);
          },
          error: (error) => {
            this.setFeedback('error', this.extractErrorMessage(error, 'Master data changes were saved, but the latest structure could not be refreshed.'));
          }
        });
      },
      error: (error) => {
        this.setFeedback('error', this.extractErrorMessage(error, 'Master data changes could not be saved.'));
      }
    });
  }

  private handleDelete(request: import('rxjs').Observable<void>, message: string, onSuccess: () => void): void {
    request.subscribe({
      next: () => {
        this.store.refreshAcademicStructure().subscribe({
          next: (structure) => {
            this.store.academicStructure.set(structure);
            onSuccess();
            this.setFeedback('success', message);
          },
          error: (error) => {
            this.setFeedback('error', this.extractErrorMessage(error, 'The record was removed, but the latest structure could not be refreshed.'));
          }
        });
      },
      error: (error) => {
        this.setFeedback('error', this.extractErrorMessage(error, 'Master data record could not be deleted.'));
      }
    });
  }

  private createEmptyInstitutionDraft(): CreateInstitution {
    return {
      code: '',
      name: '',
      city: '',
      state: '',
      country: 'India'
    };
  }

  private createEmptyCampusDraft(): CreateCampus {
    return this.syncCampusDraft({
      institutionId: this.institutions()[0]?.id ?? 0,
      code: '',
      name: '',
      city: '',
      state: '',
      country: 'India',
      boardAffiliation: 'CBSE'
    }, this.institutions());
  }

  private createEmptyAcademicYearDraft(): CreateAcademicYear {
    return this.syncAcademicYearDraft({
      campusId: this.campuses()[0]?.id ?? 0,
      name: '',
      startDate: '',
      endDate: '',
      isActive: true
    }, this.campuses());
  }

  private createEmptySchoolClassDraft(): CreateSchoolClass {
    return this.syncSchoolClassDraft({
      campusId: this.campuses()[0]?.id ?? 0,
      code: '',
      name: '',
      displayOrder: (this.store.academicStructure()?.classes.length ?? 0) + 1
    }, this.campuses());
  }

  private createEmptySectionDraft(): CreateSection {
    return this.syncSectionDraft({
      schoolClassId: this.schoolClasses()[0]?.id ?? 0,
      name: '',
      capacity: 30,
      roomNumber: ''
    }, this.schoolClasses());
  }

  private toUpdateInstitution(draft: CreateInstitution): UpdateInstitution {
    return { ...draft };
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

  private syncCampusDraft(draft: CreateCampus, institutions: Institution[]): CreateCampus {
    const fallbackInstitutionId = institutions[0]?.id ?? 0;
    const institutionId = institutions.some((institution) => institution.id === draft.institutionId) ? draft.institutionId : fallbackInstitutionId;
    return { ...draft, institutionId };
  }

  private syncAcademicYearDraft(draft: CreateAcademicYear, campuses: Campus[]): CreateAcademicYear {
    const fallbackCampusId = campuses[0]?.id ?? 0;
    const campusId = campuses.some((campus) => campus.id === draft.campusId) ? draft.campusId : fallbackCampusId;
    return { ...draft, campusId };
  }

  private syncSchoolClassDraft(draft: CreateSchoolClass, campuses: Campus[]): CreateSchoolClass {
    const fallbackCampusId = campuses[0]?.id ?? 0;
    const campusId = campuses.some((campus) => campus.id === draft.campusId) ? draft.campusId : fallbackCampusId;
    return { ...draft, campusId };
  }

  private syncSectionDraft(draft: CreateSection, schoolClasses: SchoolClass[]): CreateSection {
    const fallbackSchoolClassId = schoolClasses[0]?.id ?? 0;
    const schoolClassId = schoolClasses.some((schoolClass) => schoolClass.id === draft.schoolClassId) ? draft.schoolClassId : fallbackSchoolClassId;
    return { ...draft, schoolClassId };
  }

  private setFeedback(tone: 'success' | 'error', message: string): void {
    this.feedbackTone.set(tone);
    this.feedbackMessage.set(message);
  }

  private extractErrorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse) {
      const detail = error.error;

      if (typeof detail === 'string' && detail.trim()) {
        return detail;
      }

      if (detail && typeof detail === 'object' && 'errors' in detail && detail.errors && typeof detail.errors === 'object') {
        const messages = Object.values(detail.errors)
          .flatMap((value) => Array.isArray(value) ? value : [])
          .filter((value): value is string => typeof value === 'string' && value.trim().length > 0);

        if (messages.length > 0) {
          return messages[0];
        }
      }
    }

    return fallback;
  }
}
