import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, Observable } from 'rxjs';
import { extractApiErrorMessage } from './api-error.utils';

import {
  AcademicStructure,
  AcademicsDashboard,
  AdmissionApplication,
  AdmissionsDashboard,
  AttendanceDashboard,
  AttendanceEntryBoard,
  AttendanceEntryDraft,
  AttendanceLeaveRequest,
  AttendanceMonthlyReport,
  Campus,
  ClassAttendanceRegister,
  CreateAcademicYear,
  CreateCampus,
  CreateInstitution,
  CreateExamSchedule,
  CreateExamTerm,
  CreateAttendanceLeaveRequest,
  CreateAdmissionApplication,
  CreateFeeConcession,
  CreateHomeworkAssignment,
  CreateSchoolClass,
  CreateSection,
  CreateTransportRoute,
  CreateTransportVehicle,
  CreateSubject,
  CreateTimetablePeriod,
  AcademicYear,
  ExaminationsDashboard,
  ExamSchedule,
  ExamTerm,
  FeeReceipt,
  FeeConcession,
  FeePayment,
  FeeStructure,
  FeesDashboard,
  Guardian,
  HomeworkAssignment,
  Institution,
  SchoolClass,
  Section,
  Subject,
  StudentHomeworkProgress,
  TimetablePeriod,
  UpdateAcademicYear,
  UpdateCampus,
  UpdateInstitution,
  UpdateExamSchedule,
  UpdateExamTerm,
  UpdateHomeworkAssignment,
  UpdateHomeworkSubmission,
  UpdateSchoolClass,
  UpdateSection,
  UpdateSubject,
  UpdateTimetablePeriod,
  HomeworkDashboard,
  ParentPortalDashboard,
  RecordFeePayment,
  Student,
  StudentDocument,
  StudentProfileOverview,
  TransportDashboard,
  TransportRoute,
  TransportVehicle,
  UpdateTransportRoute,
  UpdateTransportVehicle,
  UpdateFeePayment
} from './app.models';

@Injectable({ providedIn: 'root' })
export class AppDataStore {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = 'http://localhost:5093/api';
  private hasLoaded = false;

  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly admissionsDashboard = signal<AdmissionsDashboard | null>(null);
  readonly admissionApplications = signal<AdmissionApplication[]>([]);
  readonly admissionGuardians = signal<Guardian[]>([]);
  readonly feesDashboard = signal<FeesDashboard | null>(null);
  readonly feeStructures = signal<FeeStructure[]>([]);
  readonly feePayments = signal<FeePayment[]>([]);
  readonly feeConcessions = signal<FeeConcession[]>([]);
  readonly feeReceipts = signal<FeeReceipt[]>([]);
  readonly academicsDashboard = signal<AcademicsDashboard | null>(null);
  readonly examinationsDashboard = signal<ExaminationsDashboard | null>(null);
  readonly homeworkDashboard = signal<HomeworkDashboard | null>(null);
  readonly parentPortalDashboard = signal<ParentPortalDashboard | null>(null);
  readonly isLoadingParentPortal = signal(false);
  readonly parentPortalError = signal<string | null>(null);
  readonly attendanceDashboard = signal<AttendanceDashboard | null>(null);
  readonly attendanceMonthlyReport = signal<AttendanceMonthlyReport | null>(null);
  readonly attendanceEntryBoard = signal<AttendanceEntryBoard | null>(null);
  readonly classAttendanceRegister = signal<ClassAttendanceRegister | null>(null);
  readonly attendanceLeaveRequests = signal<AttendanceLeaveRequest[]>([]);
  readonly transportDashboard = signal<TransportDashboard | null>(null);
  readonly transportRoutes = signal<TransportRoute[]>([]);
  readonly transportVehicles = signal<TransportVehicle[]>([]);
  readonly academicStructure = signal<AcademicStructure | null>(null);
  readonly students = signal<Student[]>([]);
  readonly studentProfiles = signal<StudentProfileOverview[]>([]);
  readonly studentDocuments = signal<StudentDocument[]>([]);
  readonly isSavingAttendance = signal(false);
  readonly isUpdatingLeave = signal(false);
  readonly isCreatingLeave = signal(false);
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

  createLeaveRequest(request: CreateAttendanceLeaveRequest): void {
    this.isCreatingLeave.set(true);
    this.attendanceSaveMessage.set(null);

    this.http.post(`${this.apiBaseUrl}/attendance/leave-requests`, request).subscribe({
      next: () => {
        this.isCreatingLeave.set(false);
        this.refreshAttendanceModule(request.leaveDate, this.selectedClassId(), this.selectedSectionId(), 'Leave request submitted successfully.');
      },
      error: () => {
        this.isCreatingLeave.set(false);
        this.attendanceSaveMessage.set('Leave request could not be submitted.');
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

  loadAcademicStructure(): void {
    this.refreshAcademicStructure().subscribe({
      next: (structure) => {
        this.academicStructure.set(structure);
      }
    });
  }

  refreshAcademicStructure(): Observable<AcademicStructure> {
    return this.http.get<AcademicStructure>(`${this.apiBaseUrl}/academic-structure`);
  }

  createInstitution(dto: CreateInstitution): Observable<Institution> {
    return this.http.post<Institution>(`${this.apiBaseUrl}/academic-structure/institutions`, dto);
  }

  updateInstitution(institutionId: number, dto: UpdateInstitution): Observable<Institution> {
    return this.http.put<Institution>(`${this.apiBaseUrl}/academic-structure/institutions/${institutionId}`, dto);
  }

  deleteInstitution(institutionId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academic-structure/institutions/${institutionId}`);
  }

  createCampus(dto: CreateCampus): Observable<Campus> {
    return this.http.post<Campus>(`${this.apiBaseUrl}/campuses`, dto);
  }

  updateCampus(campusId: number, dto: UpdateCampus): Observable<Campus> {
    return this.http.put<Campus>(`${this.apiBaseUrl}/campuses/${campusId}`, dto);
  }

  deleteCampus(campusId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/campuses/${campusId}`);
  }

  createAcademicYear(dto: CreateAcademicYear): Observable<AcademicYear> {
    return this.http.post<AcademicYear>(`${this.apiBaseUrl}/academic-structure/academic-years`, dto);
  }

  updateAcademicYear(academicYearId: number, dto: UpdateAcademicYear): Observable<AcademicYear> {
    return this.http.put<AcademicYear>(`${this.apiBaseUrl}/academic-structure/academic-years/${academicYearId}`, dto);
  }

  deleteAcademicYear(academicYearId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academic-structure/academic-years/${academicYearId}`);
  }

  createSchoolClass(dto: CreateSchoolClass): Observable<SchoolClass> {
    return this.http.post<SchoolClass>(`${this.apiBaseUrl}/academic-structure/classes`, dto);
  }

  updateSchoolClass(schoolClassId: number, dto: UpdateSchoolClass): Observable<SchoolClass> {
    return this.http.put<SchoolClass>(`${this.apiBaseUrl}/academic-structure/classes/${schoolClassId}`, dto);
  }

  deleteSchoolClass(schoolClassId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academic-structure/classes/${schoolClassId}`);
  }

  createSection(dto: CreateSection): Observable<Section> {
    return this.http.post<Section>(`${this.apiBaseUrl}/academic-structure/sections`, dto);
  }

  updateSection(sectionId: number, dto: UpdateSection): Observable<Section> {
    return this.http.put<Section>(`${this.apiBaseUrl}/academic-structure/sections/${sectionId}`, dto);
  }

  deleteSection(sectionId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academic-structure/sections/${sectionId}`);
  }

  createSubject(dto: CreateSubject): Observable<Subject> {
    return this.http.post<Subject>(`${this.apiBaseUrl}/academics/subjects`, dto);
  }

  updateSubject(subjectId: number, dto: UpdateSubject): Observable<Subject> {
    return this.http.put<Subject>(`${this.apiBaseUrl}/academics/subjects/${subjectId}`, dto);
  }

  deleteSubject(subjectId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academics/subjects/${subjectId}`);
  }

  createTimetablePeriod(dto: CreateTimetablePeriod): Observable<TimetablePeriod> {
    return this.http.post<TimetablePeriod>(`${this.apiBaseUrl}/academics/timetable`, dto);
  }

  updateTimetablePeriod(periodId: number, dto: UpdateTimetablePeriod): Observable<TimetablePeriod> {
    return this.http.put<TimetablePeriod>(`${this.apiBaseUrl}/academics/timetable/${periodId}`, dto);
  }

  deleteTimetablePeriod(periodId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/academics/timetable/${periodId}`);
  }

  loadExaminationsDashboard(examTermId: number | null, classId: number | null, sectionId: number | null): void {
    const query = this.buildExamQuery(examTermId, classId, sectionId);

    this.http.get<ExaminationsDashboard>(`${this.apiBaseUrl}/examinations/dashboard${query}`).subscribe({
      next: (dashboard) => {
        this.examinationsDashboard.set(dashboard);
      }
    });
  }

  createExamTerm(dto: CreateExamTerm): Observable<ExamTerm> {
    return this.http.post<ExamTerm>(`${this.apiBaseUrl}/examinations/terms`, dto);
  }

  updateExamTerm(examTermId: number, dto: UpdateExamTerm): Observable<ExamTerm> {
    return this.http.put<ExamTerm>(`${this.apiBaseUrl}/examinations/terms/${examTermId}`, dto);
  }

  deleteExamTerm(examTermId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/examinations/terms/${examTermId}`);
  }

  createExamSchedule(dto: CreateExamSchedule): Observable<ExamSchedule> {
    return this.http.post<ExamSchedule>(`${this.apiBaseUrl}/examinations/schedule`, dto);
  }

  updateExamSchedule(examScheduleId: number, dto: UpdateExamSchedule): Observable<ExamSchedule> {
    return this.http.put<ExamSchedule>(`${this.apiBaseUrl}/examinations/schedule/${examScheduleId}`, dto);
  }

  deleteExamSchedule(examScheduleId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/examinations/schedule/${examScheduleId}`);
  }

  loadHomeworkDashboard(classId: number | null, sectionId: number | null): void {
    const query = this.buildClassSectionQuery(classId, sectionId);

    this.http.get<HomeworkDashboard>(`${this.apiBaseUrl}/homework/dashboard${query}`).subscribe({
      next: (dashboard) => {
        this.homeworkDashboard.set(dashboard);
      }
    });
  }

  createHomeworkAssignment(dto: CreateHomeworkAssignment): Observable<HomeworkAssignment> {
    return this.http.post<HomeworkAssignment>(`${this.apiBaseUrl}/homework/assignments`, dto);
  }

  updateHomeworkAssignment(homeworkAssignmentId: number, dto: UpdateHomeworkAssignment): Observable<HomeworkAssignment> {
    return this.http.put<HomeworkAssignment>(`${this.apiBaseUrl}/homework/assignments/${homeworkAssignmentId}`, dto);
  }

  deleteHomeworkAssignment(homeworkAssignmentId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/homework/assignments/${homeworkAssignmentId}`);
  }

  updateHomeworkSubmission(dto: UpdateHomeworkSubmission): Observable<StudentHomeworkProgress> {
    return this.http.put<StudentHomeworkProgress>(`${this.apiBaseUrl}/homework/progress`, dto);
  }

  loadParentPortal(studentId: number | null): void {
    this.isLoadingParentPortal.set(true);
    this.parentPortalError.set(null);
    const query = studentId ? `?studentId=${studentId}` : '';

    this.http.get<ParentPortalDashboard>(`${this.apiBaseUrl}/parent-portal/dashboard${query}`).subscribe({
      next: (dashboard) => {
        this.parentPortalDashboard.set(dashboard);
        this.isLoadingParentPortal.set(false);
      },
      error: (error) => {
        this.parentPortalError.set(extractApiErrorMessage(error, 'Parent portal data could not be loaded for the selected student.'));
        this.isLoadingParentPortal.set(false);
      }
    });
  }

  loadTransportDashboard(): void {
    this.fetchTransportModule().subscribe({
      next: (payload) => {
        this.transportDashboard.set(payload.dashboard);
        this.transportRoutes.set(payload.routes);
        this.transportVehicles.set(payload.vehicles);
      },
      error: (err) => {
        console.error('Failed to load transport dashboard', err);
      }
    });
  }

  createTransportRoute(dto: CreateTransportRoute): Observable<TransportRoute> {
    return this.http.post<TransportRoute>(`${this.apiBaseUrl}/transport/routes`, dto);
  }

  updateTransportRoute(routeId: number, dto: UpdateTransportRoute): Observable<TransportRoute> {
    return this.http.put<TransportRoute>(`${this.apiBaseUrl}/transport/routes/${routeId}`, dto);
  }

  deleteTransportRoute(routeId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/transport/routes/${routeId}`);
  }

  createTransportVehicle(dto: CreateTransportVehicle): Observable<TransportVehicle> {
    return this.http.post<TransportVehicle>(`${this.apiBaseUrl}/transport/vehicles`, dto);
  }

  updateTransportVehicle(vehicleId: number, dto: UpdateTransportVehicle): Observable<TransportVehicle> {
    return this.http.put<TransportVehicle>(`${this.apiBaseUrl}/transport/vehicles/${vehicleId}`, dto);
  }

  deleteTransportVehicle(vehicleId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/transport/vehicles/${vehicleId}`);
  }

  createAdmissionApplication(dto: CreateAdmissionApplication): Observable<number> {
    return this.http.post<number>(`${this.apiBaseUrl}/admissions/applications`, dto);
  }

  updateAdmissionApplicationStatus(applicationId: number, status: string): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/admissions/applications/${applicationId}/status`, { status });
  }

  loadAdmissionsDashboard(): void {
    this.fetchAdmissionsModule().subscribe({
      next: (payload) => {
        this.admissionsDashboard.set(payload.dashboard);
        this.admissionApplications.set(payload.applications);
        this.admissionGuardians.set(payload.guardians);
      },
      error: (err) => {
        console.error('Failed to load admissions dashboard', err);
      }
    });
  }

  recordFeePayment(dto: RecordFeePayment): Observable<number> {
    return this.http.post<number>(`${this.apiBaseUrl}/fees/payments`, dto);
  }

  updateFeePayment(paymentId: number, dto: UpdateFeePayment): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/fees/payments/${paymentId}`, dto);
  }

  createFeeConcession(dto: CreateFeeConcession): Observable<number> {
    return this.http.post<number>(`${this.apiBaseUrl}/fees/concessions`, dto);
  }

  approveFeeConcession(concessionId: number, approvedBy: string): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/fees/concessions/${concessionId}/approve`, { approvedBy });
  }

  generateFeeReceipt(paymentId: number): Observable<FeeReceipt> {
    return this.http.get<FeeReceipt>(`${this.apiBaseUrl}/fees/receipts/${paymentId}`);
  }

  loadFeesDashboard(): void {
    this.fetchFeesModule().subscribe({
      next: (payload) => {
        this.feesDashboard.set(payload.dashboard);
        this.feeStructures.set(payload.structures);
        this.feePayments.set(payload.payments);
        this.feeConcessions.set(payload.concessions);
        this.feeReceipts.set(payload.receipts);
      },
      error: (err) => {
        console.error('Failed to load fees dashboard', err);
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

  private fetchTransportModule() {
    return forkJoin({
      dashboard: this.http.get<TransportDashboard>(`${this.apiBaseUrl}/transport/dashboard`),
      routes: this.http.get<TransportRoute[]>(`${this.apiBaseUrl}/transport/routes`),
      vehicles: this.http.get<TransportVehicle[]>(`${this.apiBaseUrl}/transport/vehicles`)
    });
  }

  private fetchAdmissionsModule() {
    return forkJoin({
      dashboard: this.http.get<AdmissionsDashboard>(`${this.apiBaseUrl}/admissions/dashboard`),
      applications: this.http.get<AdmissionApplication[]>(`${this.apiBaseUrl}/admissions/applications`),
      guardians: this.http.get<Guardian[]>(`${this.apiBaseUrl}/admissions/guardians`)
    });
  }

  private fetchFeesModule() {
    return forkJoin({
      dashboard: this.http.get<FeesDashboard>(`${this.apiBaseUrl}/fees/dashboard`),
      structures: this.http.get<FeeStructure[]>(`${this.apiBaseUrl}/fees/structures`),
      payments: this.http.get<FeePayment[]>(`${this.apiBaseUrl}/fees/payments`),
      concessions: this.http.get<FeeConcession[]>(`${this.apiBaseUrl}/fees/concessions`),
      receipts: this.http.get<FeeReceipt[]>(`${this.apiBaseUrl}/fees/receipts`)
    });
  }

  private buildWorkspaceRequest() {
    return forkJoin({
      admissionsDashboard: this.http.get<AdmissionsDashboard>(`${this.apiBaseUrl}/admissions/dashboard`),
      admissionApplications: this.http.get<AdmissionApplication[]>(`${this.apiBaseUrl}/admissions/applications`),
      admissionGuardians: this.http.get<Guardian[]>(`${this.apiBaseUrl}/admissions/guardians`),
      feesDashboard: this.http.get<FeesDashboard>(`${this.apiBaseUrl}/fees/dashboard`),
      feeStructures: this.http.get<FeeStructure[]>(`${this.apiBaseUrl}/fees/structures`),
      feePayments: this.http.get<FeePayment[]>(`${this.apiBaseUrl}/fees/payments`),
      feeConcessions: this.http.get<FeeConcession[]>(`${this.apiBaseUrl}/fees/concessions`),
      feeReceipts: this.http.get<FeeReceipt[]>(`${this.apiBaseUrl}/fees/receipts`),
      academicsDashboard: this.http.get<AcademicsDashboard>(`${this.apiBaseUrl}/academics/dashboard`),
      examinationsDashboard: this.http.get<ExaminationsDashboard>(`${this.apiBaseUrl}/examinations/dashboard`),
      homeworkDashboard: this.http.get<HomeworkDashboard>(`${this.apiBaseUrl}/homework/dashboard`),
      parentPortalDashboard: this.http.get<ParentPortalDashboard>(`${this.apiBaseUrl}/parent-portal/dashboard`),
      attendanceDashboard: this.http.get<AttendanceDashboard>(`${this.apiBaseUrl}/attendance/dashboard`),
      attendanceMonthlyReport: this.http.get<AttendanceMonthlyReport>(`${this.apiBaseUrl}/attendance/monthly-report`),
      attendanceEntryBoard: this.http.get<AttendanceEntryBoard>(`${this.apiBaseUrl}/attendance/entry-board`),
      classAttendanceRegister: this.http.get<ClassAttendanceRegister>(`${this.apiBaseUrl}/attendance/class-register`),
      attendanceLeaveRequests: this.http.get<AttendanceLeaveRequest[]>(`${this.apiBaseUrl}/attendance/leave-requests`),
      transportDashboard: this.http.get<TransportDashboard>(`${this.apiBaseUrl}/transport/dashboard`),
      transportRoutes: this.http.get<TransportRoute[]>(`${this.apiBaseUrl}/transport/routes`),
      transportVehicles: this.http.get<TransportVehicle[]>(`${this.apiBaseUrl}/transport/vehicles`),
      academicStructure: this.http.get<AcademicStructure>(`${this.apiBaseUrl}/academic-structure`),
      students: this.http.get<Student[]>(`${this.apiBaseUrl}/students`),
      studentProfiles: this.http.get<StudentProfileOverview[]>(`${this.apiBaseUrl}/students/profile-overview`),
      studentDocuments: this.http.get<StudentDocument[]>(`${this.apiBaseUrl}/students/documents`)
    });
  }

  private applyWorkspacePayload(payload: {
    admissionsDashboard: AdmissionsDashboard;
    admissionApplications: AdmissionApplication[];
    admissionGuardians: Guardian[];
    feesDashboard: FeesDashboard;
    feeStructures: FeeStructure[];
    feePayments: FeePayment[];
    feeConcessions: FeeConcession[];
    feeReceipts: FeeReceipt[];
    academicsDashboard: AcademicsDashboard;
    examinationsDashboard: ExaminationsDashboard;
    homeworkDashboard: HomeworkDashboard;
    parentPortalDashboard: ParentPortalDashboard;
    attendanceDashboard: AttendanceDashboard;
    attendanceMonthlyReport: AttendanceMonthlyReport;
    attendanceEntryBoard: AttendanceEntryBoard;
    classAttendanceRegister: ClassAttendanceRegister;
    attendanceLeaveRequests: AttendanceLeaveRequest[];
    transportDashboard: TransportDashboard;
    transportRoutes: TransportRoute[];
    transportVehicles: TransportVehicle[];
    academicStructure: AcademicStructure;
    students: Student[];
    studentProfiles: StudentProfileOverview[];
    studentDocuments: StudentDocument[];
  }): void {
    this.admissionsDashboard.set(payload.admissionsDashboard);
    this.admissionApplications.set(payload.admissionApplications);
    this.admissionGuardians.set(payload.admissionGuardians);
    this.feesDashboard.set(payload.feesDashboard);
    this.feeStructures.set(payload.feeStructures);
    this.feePayments.set(payload.feePayments);
    this.feeConcessions.set(payload.feeConcessions);
    this.feeReceipts.set(payload.feeReceipts);
    this.academicsDashboard.set(payload.academicsDashboard);
    this.examinationsDashboard.set(payload.examinationsDashboard);
    this.homeworkDashboard.set(payload.homeworkDashboard);
    this.parentPortalDashboard.set(payload.parentPortalDashboard);
    this.parentPortalError.set(null);
    this.isLoadingParentPortal.set(false);
    this.attendanceDashboard.set(payload.attendanceDashboard);
    this.attendanceMonthlyReport.set(payload.attendanceMonthlyReport);
    this.attendanceEntryBoard.set(payload.attendanceEntryBoard);
    this.classAttendanceRegister.set(payload.classAttendanceRegister);
    this.attendanceLeaveRequests.set(payload.attendanceLeaveRequests);
    this.transportDashboard.set(payload.transportDashboard);
    this.transportRoutes.set(payload.transportRoutes);
    this.transportVehicles.set(payload.transportVehicles);
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
