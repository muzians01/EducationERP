# Education ERP

Starter repository for a school-focused Education ERP built with:

- ASP.NET Core Web API on `.NET 10`
- Angular for the web frontend
- SQL Server for the database

## Repository layout

- `backend/`: API, application, domain, and infrastructure projects
- `frontend/`: Angular web application
- `database/`: database notes and future scripts
- `docs/`: architecture and planning notes

## Run the backend

```powershell
cd backend
dotnet run --project .\EducationERP.Api\
```

The API starts on `http://localhost:5093` and exposes:

- `GET /`
- `GET /health`
- `GET /api/campuses`
- `GET /api/academic-years`
- `GET /api/classes`
- `GET /api/sections`
- `GET /api/academic-structure`
- `GET /api/admissions/applications`
- `GET /api/admissions/guardians`
- `GET /api/admissions/dashboard`
- `GET /api/students`
- `GET /api/students/profile-overview`
- `GET /api/students/documents`
- `GET /api/fees/structures`
- `GET /api/fees/student-balances`
- `GET /api/fees/payments`
- `GET /api/fees/concessions`
- `GET /api/fees/receipts`
- `GET /api/fees/dashboard`
- `GET /api/attendance/records`
- `GET /api/attendance/student-summary`
- `GET /api/attendance/class-summary`
- `GET /api/attendance/monthly-report`
- `GET /api/attendance/dashboard`
- `POST /api/setup/database/migrate`
- `GET /swagger`

## Run the frontend

```powershell
cd frontend
npm.cmd start
```

The Angular app starts on `http://localhost:4200`.

The frontend now has separate routed module screens:

- `/` overview
- `/admissions`
- `/fees`
- `/attendance`

Those views read live data from:

- `http://localhost:5093/api/admissions/dashboard`
- `http://localhost:5093/api/fees/dashboard`
- `http://localhost:5093/api/attendance/dashboard`
- `http://localhost:5093/api/attendance/monthly-report`
- `http://localhost:5093/api/students`
- `http://localhost:5093/api/students/profile-overview`
- `http://localhost:5093/api/students/documents`

## Test the backend

```powershell
cd backend
dotnet test .\EducationERP.Api.Tests\
```

For manual API testing, start the backend and open `http://localhost:5093/swagger`.

## Update the database

```powershell
cd backend
dotnet dotnet-ef database update --project .\EducationERP.Infrastructure\ --startup-project .\EducationERP.Api\
```

If you already created the older GUID-based database locally, drop that database first and then run the update command again. The schema now uses `INT IDENTITY(1,1)` primary keys throughout.

The latest migrations add student profile fields, a `StudentDocuments` table for verification tracking, completed fee tables for structures, balances, concessions, receipts, and attendance records for students.

## Next recommended features

1. Subject and timetable planning
2. Parent portal views
3. Student document upload workflows
4. Fee reminders and receipt download workflows
5. Attendance entry workflows and class registers
