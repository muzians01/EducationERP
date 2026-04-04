# Database Notes

Use SQL Server for the operational database.

Current schema includes:

1. Campuses
2. AcademicYears
3. Classes
4. Sections
5. Guardians
6. AdmissionApplications
7. Students
8. StudentDocuments
9. FeeStructures
10. StudentFees
11. FeePayments
12. FeeConcessions
13. AttendanceRecords

## Apply migrations

```powershell
cd backend
dotnet dotnet-ef database update --project .\EducationERP.Infrastructure\ --startup-project .\EducationERP.Api\
```

Note: if you already applied the old GUID-based schema, recreate the database before applying this migration set. The current schema uses `INT IDENTITY(1,1)` primary keys.

The current migration set also includes local-time `CreatedAt` / `UpdatedAt` audit columns, student profile details on `Students`, seeded verification records in `StudentDocuments`, fee master/ledger/payment/concession seed data, and seeded attendance records.

## Generate SQL script

```powershell
cd backend
dotnet dotnet-ef migrations script --project .\EducationERP.Infrastructure\ --startup-project .\EducationERP.Api\
```

## Next schema slices

1. Subject and timetable masters
2. Document upload storage and approval workflow
3. Fee reminders and discount rules
4. Parent receipts and statement downloads
5. Attendance monthly reports and class registers

Connection string is configured in `backend/EducationERP.Api/appsettings.json`.
