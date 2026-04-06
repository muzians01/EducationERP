import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes } from './app.routes';

describe('App', () => {
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter(routes)]
    }).compileComponents();

    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    httpTesting.expectOne('http://localhost:5093/api/admissions/dashboard').flush({
      totalApplications: 1,
      newApplications: 1,
      approvedApplications: 0,
      waitlistedApplications: 0,
      totalRegistrationFees: 1500,
      recentApplications: [],
      guardians: []
    });
    httpTesting.expectOne('http://localhost:5093/api/fees/dashboard').flush({
      totalExpectedAmount: 60000,
      totalConcessionAmount: 6000,
      netReceivableAmount: 54000,
      totalCollectedAmount: 24000,
      outstandingAmount: 30000,
      overdueCount: 0,
      outstandingFees: [],
      recentPayments: [],
      recentReceipts: []
    });
    httpTesting.expectOne('http://localhost:5093/api/academics/dashboard').flush({
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      subjectCount: 3,
      weeklyPeriodsPlanned: 2,
      subjects: [],
      weeklyTimetable: []
    });
    httpTesting.expectOne('http://localhost:5093/api/examinations/dashboard').flush({
      selectedExamTermId: 1,
      selectedExamTermName: 'Term 1 Assessment',
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      examTerms: [],
      schedule: [],
      reportCards: []
    });
    httpTesting.expectOne('http://localhost:5093/api/homework/dashboard').flush({
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      activeAssignments: 1,
      pendingSubmissions: 1,
      assignments: [],
      progress: []
    });
    httpTesting.expectOne('http://localhost:5093/api/parent-portal/dashboard').flush({
      studentId: 1,
      studentName: 'Ishita Verma',
      admissionNumber: 'STU-2026-001',
      className: 'Grade 1',
      sectionName: 'B',
      guardianName: 'Anjali Verma',
      guardianPhoneNumber: '9876500001',
      attendancePercentage: 83.5,
      outstandingFees: 12000,
      currentExamTerm: 'Term 1 Assessment',
      latestExamPercentage: 88.5,
      announcements: [],
      upcomingHomework: [],
      outstandingFeeItems: [],
      examResults: [],
      todayTimetable: [],
      recentAttendance: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/dashboard').flush({
      attendanceDate: '2026-04-03',
      totalStudentsMarked: 1,
      presentCount: 0,
      absentCount: 1,
      lateCount: 0,
      todayRecords: [],
      studentSummaries: [],
      classSummaries: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/monthly-report').flush({
      monthLabel: 'April 2026',
      workingDays: 3,
      studentsCovered: 1,
      overallAttendancePercentage: 50,
      classSummaries: [],
      studentsNeedingAttention: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/entry-board').flush({
      attendanceDate: '2026-04-03',
      classId: 1,
      className: 'Grade 1',
      sectionId: 2,
      sectionName: 'B',
      studentsOnRoll: 1,
      studentsMarked: 1,
      students: [],
      upcomingHolidays: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/class-register').flush({
      monthLabel: 'April 2026',
      classId: 1,
      className: 'Grade 1',
      sectionId: 2,
      sectionName: 'B',
      workingDayLabels: [],
      rows: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/leave-requests').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/academic-structure').flush({
      academicYears: [],
      classes: [{ id: 1, campusId: 1, code: 'G1', name: 'Grade 1', displayOrder: 1 }],
      sections: [{ id: 2, schoolClassId: 1, schoolClassName: 'Grade 1', name: 'B', capacity: 35, roomNumber: 'G1-B01' }]
    });
    httpTesting.expectOne('http://localhost:5093/api/students').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/students/profile-overview').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/students/documents').flush([]);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(App);
    httpTesting.expectOne('http://localhost:5093/api/admissions/dashboard').flush({
      totalApplications: 1,
      newApplications: 1,
      approvedApplications: 0,
      waitlistedApplications: 0,
      totalRegistrationFees: 1500,
      recentApplications: [],
      guardians: []
    });
    httpTesting.expectOne('http://localhost:5093/api/fees/dashboard').flush({
      totalExpectedAmount: 60000,
      totalConcessionAmount: 6000,
      netReceivableAmount: 54000,
      totalCollectedAmount: 24000,
      outstandingAmount: 30000,
      overdueCount: 0,
      outstandingFees: [],
      recentPayments: [],
      recentReceipts: []
    });
    httpTesting.expectOne('http://localhost:5093/api/academics/dashboard').flush({
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      subjectCount: 3,
      weeklyPeriodsPlanned: 2,
      subjects: [],
      weeklyTimetable: []
    });
    httpTesting.expectOne('http://localhost:5093/api/examinations/dashboard').flush({
      selectedExamTermId: 1,
      selectedExamTermName: 'Term 1 Assessment',
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      examTerms: [],
      schedule: [],
      reportCards: []
    });
    httpTesting.expectOne('http://localhost:5093/api/homework/dashboard').flush({
      selectedClassId: 1,
      selectedClassName: 'Grade 1',
      selectedSectionId: 2,
      selectedSectionName: 'B',
      activeAssignments: 1,
      pendingSubmissions: 1,
      assignments: [],
      progress: []
    });
    httpTesting.expectOne('http://localhost:5093/api/parent-portal/dashboard').flush({
      studentId: 1,
      studentName: 'Ishita Verma',
      admissionNumber: 'STU-2026-001',
      className: 'Grade 1',
      sectionName: 'B',
      guardianName: 'Anjali Verma',
      guardianPhoneNumber: '9876500001',
      attendancePercentage: 83.5,
      outstandingFees: 12000,
      currentExamTerm: 'Term 1 Assessment',
      latestExamPercentage: 88.5,
      announcements: [],
      upcomingHomework: [],
      outstandingFeeItems: [],
      examResults: [],
      todayTimetable: [],
      recentAttendance: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/dashboard').flush({
      attendanceDate: '2026-04-03',
      totalStudentsMarked: 1,
      presentCount: 0,
      absentCount: 1,
      lateCount: 0,
      todayRecords: [],
      studentSummaries: [],
      classSummaries: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/monthly-report').flush({
      monthLabel: 'April 2026',
      workingDays: 3,
      studentsCovered: 1,
      overallAttendancePercentage: 50,
      classSummaries: [],
      studentsNeedingAttention: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/entry-board').flush({
      attendanceDate: '2026-04-03',
      classId: 1,
      className: 'Grade 1',
      sectionId: 2,
      sectionName: 'B',
      studentsOnRoll: 1,
      studentsMarked: 1,
      students: [],
      upcomingHolidays: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/class-register').flush({
      monthLabel: 'April 2026',
      classId: 1,
      className: 'Grade 1',
      sectionId: 2,
      sectionName: 'B',
      workingDayLabels: [],
      rows: []
    });
    httpTesting.expectOne('http://localhost:5093/api/attendance/leave-requests').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/academic-structure').flush({
      academicYears: [],
      classes: [{ id: 1, campusId: 1, code: 'G1', name: 'Grade 1', displayOrder: 1 }],
      sections: [{ id: 2, schoolClassId: 1, schoolClassName: 'Grade 1', name: 'B', capacity: 35, roomNumber: 'G1-B01' }]
    });
    httpTesting.expectOne('http://localhost:5093/api/students').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/students/profile-overview').flush([]);
    httpTesting.expectOne('http://localhost:5093/api/students/documents').flush([]);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Education ERP Workspace');
  });
});
