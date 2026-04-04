# Education ERP Architecture

## Vision

This solution is designed as a modular school ERP that starts on the web and can later expose the same backend to Android and iOS clients.

## Core stack

- Backend: ASP.NET Core Web API on `.NET 10`
- Frontend: Angular
- Database: SQL Server with Entity Framework Core
- Mobile later: same API reused by native or cross-platform apps

## Suggested modules

- Admissions and enrolment
- Student information system
- Attendance
- Timetable and academics
- Fees and finance
- Examinations and report cards
- Staff and HR
- Transport
- Inventory and assets
- Parent and student portal

## Backend structure

- `EducationERP.Api`: HTTP endpoints and host configuration
- `EducationERP.Application`: contracts and use-case level abstractions
- `EducationERP.Domain`: core entities and business rules
- `EducationERP.Infrastructure`: EF Core, SQL Server, and service implementations

## Delivery path

1. Build a strong multi-campus foundation first.
2. Implement admissions, student master, class/section, and academic year.
3. Add attendance, timetable, and fee management.
4. Add parent, teacher, and student experiences.
5. Reuse the API for Android and iOS apps.
