export interface AcademicStructure {
  academicYears: AcademicYear[];
  classes: SchoolClass[];
  sections: Section[];
}

export interface AcademicsDashboard {
  selectedClassId: number;
  selectedClassName: string;
  selectedSectionId: number;
  selectedSectionName: string;
  subjectCount: number;
  weeklyPeriodsPlanned: number;
  subjects: Subject[];
  weeklyTimetable: TimetableDay[];
}

export interface ExaminationsDashboard {
  selectedExamTermId: number;
  selectedExamTermName: string;
  selectedClassId: number;
  selectedClassName: string;
  selectedSectionId: number;
  selectedSectionName: string;
  examTerms: ExamTerm[];
  schedule: ExamSchedule[];
  reportCards: StudentReportCard[];
}

export interface ParentPortalDashboard {
  studentId: number;
  studentName: string;
  admissionNumber: string;
  className: string;
  sectionName: string;
  guardianName: string;
  guardianPhoneNumber: string;
  attendancePercentage: number;
  outstandingFees: number;
  currentExamTerm: string;
  latestExamPercentage: number;
  announcements: ParentPortalAnnouncement[];
  upcomingHomework: ParentPortalHomework[];
  outstandingFeeItems: ParentPortalFeeItem[];
  examResults: ParentPortalExamResult[];
  todayTimetable: ParentPortalTimetableItem[];
  recentAttendance: ParentPortalAttendanceEntry[];
}

export interface HomeworkDashboard {
  selectedClassId: number;
  selectedClassName: string;
  selectedSectionId: number;
  selectedSectionName: string;
  activeAssignments: number;
  pendingSubmissions: number;
  assignments: HomeworkAssignment[];
  progress: StudentHomeworkProgress[];
}

export interface HomeworkAssignment {
  id: number;
  classId: number;
  className: string;
  sectionId: number;
  sectionName: string;
  subjectId: number;
  subjectName: string;
  assignedOn: string;
  dueOn: string;
  title: string;
  instructions: string;
  assignedBy: string;
}

export interface StudentHomeworkProgress {
  homeworkAssignmentId: number;
  studentId: number;
  studentName: string;
  admissionNumber: string;
  homeworkTitle: string;
  dueOn: string;
  status: string;
  submittedOn: string | null;
  remarks: string | null;
}

export interface ParentPortalAnnouncement {
  title: string;
  message: string;
  publishDate: string;
}

export interface ParentPortalHomework {
  subjectName: string;
  title: string;
  dueOn: string;
  status: string;
  instructions: string;
}

export interface ParentPortalFeeItem {
  feeName: string;
  dueOn: string;
  balanceAmount: number;
  status: string;
}

export interface ParentPortalExamResult {
  examTermName: string;
  subjectName: string;
  marksObtained: number;
  maxMarks: number;
  grade: string;
  resultStatus: string;
}

export interface ParentPortalTimetableItem {
  dayOfWeek: string;
  periodNumber: number;
  subjectName: string;
  startTime: string;
  endTime: string;
  teacherName: string;
}

export interface ParentPortalAttendanceEntry {
  attendanceDate: string;
  status: string;
  remarks: string | null;
}

export interface ExamTerm {
  id: number;
  name: string;
  examType: string;
  startDate: string;
  endDate: string;
  status: string;
}

export interface ExamSchedule {
  id: number;
  examTermId: number;
  examTermName: string;
  classId: number;
  className: string;
  sectionId: number;
  sectionName: string;
  subjectId: number;
  subjectName: string;
  examDate: string;
  startTime: string;
  durationMinutes: number;
  maxMarks: number;
  passMarks: number;
}

export interface StudentReportCard {
  studentId: number;
  studentName: string;
  admissionNumber: string;
  className: string;
  sectionName: string;
  totalMarks: number;
  marksObtained: number;
  percentage: number;
  resultStatus: string;
  subjectResults: ExamSubjectResult[];
}

export interface ExamSubjectResult {
  subjectId: number;
  subjectName: string;
  maxMarks: number;
  passMarks: number;
  marksObtained: number;
  grade: string;
  resultStatus: string;
}

export interface Subject {
  id: number;
  code: string;
  name: string;
  category: string;
  weeklyPeriods: number;
}

export interface TimetableDay {
  dayOfWeek: string;
  periods: TimetablePeriod[];
}

export interface TimetablePeriod {
  id: number;
  classId: number;
  className: string;
  sectionId: number;
  sectionName: string;
  subjectId: number;
  subjectName: string;
  subjectCode: string;
  dayOfWeek: string;
  periodNumber: number;
  startTime: string;
  endTime: string;
  teacherName: string;
  roomNumber: string;
}

export interface AcademicYear {
  id: number;
  campusId: number;
  name: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface SchoolClass {
  id: number;
  campusId: number;
  code: string;
  name: string;
  displayOrder: number;
}

export interface Section {
  id: number;
  schoolClassId: number;
  schoolClassName: string;
  name: string;
  capacity: number;
  roomNumber: string;
}

export interface AdmissionsDashboard {
  totalApplications: number;
  newApplications: number;
  approvedApplications: number;
  waitlistedApplications: number;
  totalRegistrationFees: number;
  recentApplications: AdmissionApplication[];
  guardians: Guardian[];
}

export interface AdmissionApplication {
  id: number;
  applicationNumber: string;
  studentName: string;
  status: string;
  appliedOn: string;
  campusName: string;
  academicYearName: string;
  className: string;
  sectionName: string;
  guardianName: string;
  guardianPhoneNumber: string;
  registrationFee: number;
}

export interface Guardian {
  id: number;
  fullName: string;
  relationship: string;
  phoneNumber: string;
  email: string;
  occupation: string;
  campusName: string;
}

export interface Student {
  id: number;
  admissionNumber: string;
  studentName: string;
  campusName: string;
  academicYearName: string;
  className: string;
  sectionName: string;
  guardianName: string;
  enrolledOn: string;
  status: string;
}

export interface StudentProfileOverview {
  id: number;
  admissionNumber: string;
  studentName: string;
  className: string;
  sectionName: string;
  guardianName: string;
  primaryContactNumber: string;
  address: string;
  gender: string;
  bloodGroup: string | null;
  medicalNotes: string | null;
  profileCompletionPercentage: number;
  pendingDocumentCount: number;
}

export interface StudentDocument {
  id: number;
  studentId: number;
  studentName: string;
  admissionNumber: string;
  documentType: string;
  status: string;
  submittedOn: string;
  verifiedOn: string | null;
  remarks: string | null;
}

export interface FeeStructure {
  id: number;
  feeCode: string;
  feeName: string;
  campusName: string;
  academicYearName: string;
  className: string;
  billingCycle: string;
  amount: number;
}

export interface StudentFee {
  id: number;
  studentId: number;
  studentName: string;
  admissionNumber: string;
  feeName: string;
  billingCycle: string;
  dueOn: string;
  amount: number;
  concessionAmount: number;
  amountPaid: number;
  balanceAmount: number;
  status: string;
}

export interface FeePayment {
  id: number;
  studentName: string;
  admissionNumber: string;
  feeName: string;
  paymentReference: string;
  paidOn: string;
  amount: number;
  paymentMethod: string;
  status: string;
}

export interface FeesDashboard {
  totalExpectedAmount: number;
  totalConcessionAmount: number;
  netReceivableAmount: number;
  totalCollectedAmount: number;
  outstandingAmount: number;
  overdueCount: number;
  outstandingFees: StudentFee[];
  recentPayments: FeePayment[];
  recentReceipts: FeeReceipt[];
}

export interface FeeReceipt {
  id: number;
  receiptNumber: string;
  studentName: string;
  admissionNumber: string;
  feeName: string;
  paidOn: string;
  amount: number;
  paymentMethod: string;
  status: string;
}

export interface AttendanceRecord {
  id: number;
  studentName: string;
  admissionNumber: string;
  className: string;
  sectionName: string;
  attendanceDate: string;
  status: string;
  markedOn: string;
  remarks: string | null;
}

export interface StudentAttendanceSummary {
  studentId: number;
  studentName: string;
  admissionNumber: string;
  className: string;
  sectionName: string;
  presentDays: number;
  absentDays: number;
  lateDays: number;
  attendancePercentage: number;
}

export interface AttendanceDashboard {
  attendanceDate: string;
  totalStudentsMarked: number;
  presentCount: number;
  absentCount: number;
  lateCount: number;
  todayRecords: AttendanceRecord[];
  studentSummaries: StudentAttendanceSummary[];
  classSummaries: ClassAttendanceSummary[];
}

export interface ClassAttendanceSummary {
  className: string;
  sectionName: string;
  studentsMarked: number;
  presentCount: number;
  absentCount: number;
  lateCount: number;
  attendancePercentage: number;
}

export interface AttendanceMonthlyReport {
  monthLabel: string;
  workingDays: number;
  studentsCovered: number;
  overallAttendancePercentage: number;
  classSummaries: ClassAttendanceSummary[];
  studentsNeedingAttention: StudentAttendanceSummary[];
}

export interface AttendanceEntryStudent {
  studentId: number;
  studentName: string;
  admissionNumber: string;
  status: string;
  hasApprovedLeave: boolean;
  leaveType: string | null;
  remarks: string | null;
}

export interface AttendanceEntryDraft {
  studentId: number;
  status: string;
  remarks: string | null;
}

export interface AttendanceHoliday {
  id: number;
  holidayDate: string;
  title: string;
  category: string;
}

export interface AttendanceEntryBoard {
  attendanceDate: string;
  classId: number;
  className: string;
  sectionId: number;
  sectionName: string;
  studentsOnRoll: number;
  studentsMarked: number;
  students: AttendanceEntryStudent[];
  upcomingHolidays: AttendanceHoliday[];
}

export interface AttendanceLeaveRequest {
  id: number;
  studentId: number;
  studentName: string;
  admissionNumber: string;
  className: string;
  sectionName: string;
  leaveDate: string;
  leaveType: string;
  reason: string;
  status: string;
}

export interface ClassRegisterRow {
  studentId: number;
  studentName: string;
  admissionNumber: string;
  dailyStatus: Record<string, string>;
  presentDays: number;
  absentDays: number;
  lateDays: number;
}

export interface ClassAttendanceRegister {
  monthLabel: string;
  classId: number;
  className: string;
  sectionId: number;
  sectionName: string;
  workingDayLabels: string[];
  rows: ClassRegisterRow[];
}
