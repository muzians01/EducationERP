import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';

import {
  AcademicStructure,
  AcademicsDashboard,
  AdmissionsDashboard,
  AttendanceDashboard,
  AttendanceEntryBoard,
  AttendanceEntryDraft,
  AttendanceLeaveRequest,
  AttendanceMonthlyReport,
  ClassAttendanceRegister,
  ExaminationsDashboard,
  FeesDashboard,
  Student,
  StudentDocument,
  StudentProfileOverview
} from './app.models';

@Injectable({ providedIn: 'root' })
export class AppDataStore {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = 'http://localhost:5093/api';
  private hasLoaded = false;

  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly admissionsDashboard = signal<AdmissionsDashboard | null>(null);
  readonly feesDashboard = signal<FeesDashboard | null>(null);
  readonly academicsDashboard = signal<AcademicsDashboard | null>(null);
  readonly examinationsDashboard = signal<ExaminationsDashboard | null>(null);
  readonly attendanceDashboard = signal<AttendanceDashboard | null>(null);
  readonly attendanceMonthlyReport = signal<AttendanceMonthlyReport | null>(null);
  readonly attendanceEntryBoard = signal<AttendanceEntryBoard | null>(null);
  readonly classAttendanceRegister = signal<ClassAttendanceRegister | null>(null);
  readonly attendanceLeaveRequests = signal<AttendanceLeaveRequest[]>([]);
  readonly academicStructure = signal<AcademicStructure | null>(null);
  readonly students = signal<Student[]>([]);
  readonly studentProfiles = signal<StudentProfileOverview[]>([]);
  readonly studentDocuments = signal<StudentDocument[]>([]);
  readonly isSavingAttendance = signal(false);
  readonly isUpdatingLeave = signal(false);
  readonly attendanceSaveMessage = signal<string | null>(null);
  readonly selectedAttendanceDate = signal<string | null>(null);
  readonly selectedClassId = signal<number | null>(null);
  readonly selectedSectionId = signal<number | null>(null);

  readonly pendingDocumentsCount = computed(
    () => this.studentDocuments().filter((document) => document.status !== 'Verified').length
  );

  load(): void {
    if (this.hasLoaded) {
      return;
    }

    this.hasLoaded = true;
    this.isLoading.set(true);
    this.loadError.set(null);
    this.attendanceSaveMessage.set(null);

    this.buildWorkspaceRequest().subscribe({
      next: (payload) => {
        this.applyWorkspacePayload(payload);
        this.captureAttendanceSelection(payload.attendanceEntryBoard);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set('Could not load the ERP workspace. Start the backend API on localhost:5093 and refresh.');
        this.isLoading.set(false);
      }
    });
  }

  saveAttendance(attendanceDate: string, students: AttendanceEntryDraft[]): void {
    this.isSavingAttendance.set(true);
    this.attendanceSaveMessage.set(null);
    this.selectedAttendanceDate.set(attendanceDate);

    this.http.post<AttendanceEntryBoard>(`${this.apiBaseUrl}/attendance/entry-board`, {
      attendanceDate,
      students
    }).subscribe({
      next: (board) => {
        this.captureAttendanceSelection(board);
        this.attendanceEntryBoard.set(board);
        this.refreshAttendanceModule(board.attendanceDate, board.classId, board.sectionId, 'Attendance saved for the selected roster.');
      },
      error: () => {
        this.isSavingAttendance.set(false);
        this.attendanceSaveMessage.set('Attendance could not be saved. Please retry with the API running.');
      }
    });
  }

  loadAttendanceForSelection(attendanceDate: string, classId: number | null, sectionId: number | null): void {
    this.isLoading.set(true);
    this.loadError.set(null);
    this.attendanceSaveMessage.set(null);
    this.selectedAttendanceDate.set(attendanceDate);
    this.selectedClassId.set(classId);
    this.selectedSectionId.set(sectionId);

    this.fetchAttendanceModule(attendanceDate, classId, sectionId).subscribe({
      next: (payload) => {
        this.attendanceDashboard.set(payload.attendanceDashboard);
        this.attendanceMonthlyReport.set(payload.attendanceMonthlyReport);
        this.attendanceEntryBoard.set(payload.attendanceEntryBoard);
        this.classAttendanceRegister.set(payload.classAttendanceRegister);
        this.attendanceLeaveRequests.set(payload.attendanceLeaveRequests);
        this.captureAttendanceSelection(payload.attendanceEntryBoard);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set('Attendance data could not be loaded for the selected roster.');
        this.isLoading.set(false);
      }
    });
  }

  updateLeaveStatus(leaveRequestId: number, status: string): void {
    const attendanceDate = this.selectedAttendanceDate();
    if (!attendanceDate) {
      return;
    }

    this.isUpdatingLeave.set(true);
    this.attendanceSaveMessage.set(null);

    this.http.post<AttendanceLeaveRequest[]>(
      `${this.apiBaseUrl}/attendance/leave-requests/${leaveRequestId}/status${this.buildAttendanceQuery(attendanceDate, this.selectedClassId(), this.selectedSectionId())}`,
      { status }
    ).subscribe({
      next: (requests) => {
        this.attendanceLeaveRequests.set(requests);
        this.isUpdatingLeave.set(false);
        this.refreshAttendanceModule(attendanceDate, this.selectedClassId(), this.selectedSectionId(), `Leave request ${status.toLowerCase()} successfully.`);
      },
      error: () => {
        this.isUpdatingLeave.set(false);
        this.attendanceSaveMessage.set('Leave request status could not be updated.');
      }
    });
  }

  loadAcademicsDashboard(classId: number | null, sectionId: number | null): void {
    const query = this.buildClassSectionQuery(classId, sectionId);

    this.http.get<AcademicsDashboard>(`${this.apiBaseUrl}/academics/dashboard${query}`).subscribe({
      next: (dashboard) => {
        this.academicsDashboard.set(dashboard);
      }
    });
  }

  loadExaminationsDashboard(examTermId: number | null, classId: number | null, sectionId: number | null): void {
    const query = this.buildExamQuery(examTermId, classId, sectionId);

    this.http.get<ExaminationsDashboard>(`${this.apiBaseUrl}/examinations/dashboard${query}`).subscribe({
      next: (dashboard) => {
        this.examinationsDashboard.set(dashboard);
      }
    });
  }

  private refreshAttendanceModule(attendanceDate: string, classId: number | null, sectionId: number | null, successMessage: string): void {
    this.fetchAttendanceModule(attendanceDate, classId, sectionId).subscribe({
      next: (payload) => {
        this.attendanceDashboard.set(payload.attendanceDashboard);
        this.attendanceMonthlyReport.set(payload.attendanceMonthlyReport);
        this.attendanceEntryBoard.set(payload.attendanceEntryBoard);
        this.classAttendanceRegister.set(payload.classAttendanceRegister);
        this.attendanceLeaveRequests.set(payload.attendanceLeaveRequests);
        this.captureAttendanceSelection(payload.attendanceEntryBoard);
        this.isSavingAttendance.set(false);
        this.isUpdatingLeave.set(false);
        this.attendanceSaveMessage.set(successMessage);
      },
      error: () => {
        this.isSavingAttendance.set(false);
        this.isUpdatingLeave.set(false);
        this.attendanceSaveMessage.set('The roster updated, but the dashboard refresh failed. Refresh the page.');
      }
    });
  }

  private fetchAttendanceModule(attendanceDate: string, classId: number | null, sectionId: number | null) {
    const attendanceQuery = this.buildAttendanceQuery(attendanceDate, classId, sectionId);
    const registerQuery = this.buildReferenceDateQuery(attendanceDate, classId, sectionId);

    return forkJoin({
      attendanceDashboard: this.http.get<AttendanceDashboard>(`${this.apiBaseUrl}/attendance/dashboard${attendanceQuery}`),
      attendanceMonthlyReport: this.http.get<AttendanceMonthlyReport>(`${this.apiBaseUrl}/attendance/monthly-report?referenceDate=${encodeURIComponent(attendanceDate)}`),
      attendanceEntryBoard: this.http.get<AttendanceEntryBoard>(`${this.apiBaseUrl}/attendance/entry-board${attendanceQuery}`),
      classAttendanceRegister: this.http.get<ClassAttendanceRegister>(`${this.apiBaseUrl}/attendance/class-register${registerQuery}`),
      attendanceLeaveRequests: this.http.get<AttendanceLeaveRequest[]>(`${this.apiBaseUrl}/attendance/leave-requests${attendanceQuery}`)
    });
  }

  private buildWorkspaceRequest() {
    return forkJoin({
      admissionsDashboard: this.http.get<AdmissionsDashboard>(`${this.apiBaseUrl}/admissions/dashboard`),
      feesDashboard: this.http.get<FeesDashboard>(`${this.apiBaseUrl}/fees/dashboard`),
      academicsDashboard: this.http.get<AcademicsDashboard>(`${this.apiBaseUrl}/academics/dashboard`),
      examinationsDashboard: this.http.get<ExaminationsDashboard>(`${this.apiBaseUrl}/examinations/dashboard`),
      attendanceDashboard: this.http.get<AttendanceDashboard>(`${this.apiBaseUrl}/attendance/dashboard`),
      attendanceMonthlyReport: this.http.get<AttendanceMonthlyReport>(`${this.apiBaseUrl}/attendance/monthly-report`),
      attendanceEntryBoard: this.http.get<AttendanceEntryBoard>(`${this.apiBaseUrl}/attendance/entry-board`),
      classAttendanceRegister: this.http.get<ClassAttendanceRegister>(`${this.apiBaseUrl}/attendance/class-register`),
      attendanceLeaveRequests: this.http.get<AttendanceLeaveRequest[]>(`${this.apiBaseUrl}/attendance/leave-requests`),
      academicStructure: this.http.get<AcademicStructure>(`${this.apiBaseUrl}/academic-structure`),
      students: this.http.get<Student[]>(`${this.apiBaseUrl}/students`),
      studentProfiles: this.http.get<StudentProfileOverview[]>(`${this.apiBaseUrl}/students/profile-overview`),
      studentDocuments: this.http.get<StudentDocument[]>(`${this.apiBaseUrl}/students/documents`)
    });
  }

  private applyWorkspacePayload(payload: {
    admissionsDashboard: AdmissionsDashboard;
    feesDashboard: FeesDashboard;
    academicsDashboard: AcademicsDashboard;
    examinationsDashboard: ExaminationsDashboard;
    attendanceDashboard: AttendanceDashboard;
    attendanceMonthlyReport: AttendanceMonthlyReport;
    attendanceEntryBoard: AttendanceEntryBoard;
    classAttendanceRegister: ClassAttendanceRegister;
    attendanceLeaveRequests: AttendanceLeaveRequest[];
    academicStructure: AcademicStructure;
    students: Student[];
    studentProfiles: StudentProfileOverview[];
    studentDocuments: StudentDocument[];
  }): void {
    this.admissionsDashboard.set(payload.admissionsDashboard);
    this.feesDashboard.set(payload.feesDashboard);
    this.academicsDashboard.set(payload.academicsDashboard);
    this.examinationsDashboard.set(payload.examinationsDashboard);
    this.attendanceDashboard.set(payload.attendanceDashboard);
    this.attendanceMonthlyReport.set(payload.attendanceMonthlyReport);
    this.attendanceEntryBoard.set(payload.attendanceEntryBoard);
    this.classAttendanceRegister.set(payload.classAttendanceRegister);
    this.attendanceLeaveRequests.set(payload.attendanceLeaveRequests);
    this.academicStructure.set(payload.academicStructure);
    this.students.set(payload.students);
    this.studentProfiles.set(payload.studentProfiles);
    this.studentDocuments.set(payload.studentDocuments);
  }

  private captureAttendanceSelection(board: AttendanceEntryBoard): void {
    this.selectedAttendanceDate.set(board.attendanceDate);
    this.selectedClassId.set(board.classId);
    this.selectedSectionId.set(board.sectionId);
  }

  private buildAttendanceQuery(attendanceDate: string, classId: number | null, sectionId: number | null): string {
    const parameters = new URLSearchParams();
    parameters.set('attendanceDate', attendanceDate);

    if (classId) {
      parameters.set('classId', classId.toString());
    }

    if (sectionId) {
      parameters.set('sectionId', sectionId.toString());
    }

    return `?${parameters.toString()}`;
  }

  private buildReferenceDateQuery(referenceDate: string, classId: number | null, sectionId: number | null): string {
    const parameters = new URLSearchParams();
    parameters.set('referenceDate', referenceDate);

    if (classId) {
      parameters.set('classId', classId.toString());
    }

    if (sectionId) {
      parameters.set('sectionId', sectionId.toString());
    }

    return `?${parameters.toString()}`;
  }

  private buildClassSectionQuery(classId: number | null, sectionId: number | null): string {
    const parameters = new URLSearchParams();

    if (classId) {
      parameters.set('classId', classId.toString());
    }

    if (sectionId) {
      parameters.set('sectionId', sectionId.toString());
    }

    return parameters.size === 0 ? '' : `?${parameters.toString()}`;
  }

  private buildExamQuery(examTermId: number | null, classId: number | null, sectionId: number | null): string {
    const parameters = new URLSearchParams();

    if (examTermId) {
      parameters.set('examTermId', examTermId.toString());
    }

    if (classId) {
      parameters.set('classId', classId.toString());
    }

    if (sectionId) {
      parameters.set('sectionId', sectionId.toString());
    }

    return parameters.size === 0 ? '' : `?${parameters.toString()}`;
  }
}
