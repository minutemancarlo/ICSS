Overview:
The Integrated Class Scheduling System is a feature-rich platform designed to streamline the academic scheduling process for educational institutions. This system, built using Blazor .NET Core WASM PWA, ensures seamless online and offline functionality, robust user management, and conflict-free scheduling. It leverages modern web technologies to offer an installable, responsive, and user-friendly interface.

Key Features
1. Authentication and User Management
- Google Login:
  - Integrate Google OAuth for secure and convenient login.
  - Support for role-based access control for different user types (Admin, Faculty, Students, etc.).
- Registration:
  - Allow users to register via email with password or Google login.
- Email Confirmation:
  - Implement email-based account verification to ensure authenticity.
  - Send confirmation links with a time-limited validity.
- User Roles:
  - Students: Access class schedules and grades.
  - Admin: Manage users, courses, schedules, and system configurations.
  - Faculty: View and manage teaching schedules and workloads.
  - Chairpersons: Oversee department-level schedules and approve changes.
  - EndUsers: Limited access for viewing schedules and notifications.
2. Offline Viewing
Implement Progressive Web Application (PWA) capabilities to:
- Enable offline access to schedules and essential data.
- Sync changes automatically when the device reconnects to the internet.
3. Installable Web UI (WebAssembly)
Provide an installable web application experience using Blazor WASM PWA:
- Mobile-friendly and cross-platform compatibility.
- Local caching for faster load times and offline support.
4. Academic Data Management
- Courses:
  - Add and manage courses with details such as course code, description, and prerequisites.
- Semesters:
  - Add 1st and 2nd semesters for each school year.
  - Assign specific start and end dates for each semester.
- Sections:
  - Manage sections for 1st to 4th year students.
  - Designate section names (e.g., 1A, 1B, up to 1E) for each year level.
- Subjects:
  - Add subject details including:
    - Subject Code
    - Description
    - Units
    - Load Units
    - Schedule
    - Days (e.g., MWF, TTh)
    - Room Assignment
    - Assigned Instructor
5. Advanced Scheduling
- Automatic Scheduling:
  - Develop an algorithm to create conflict-free schedules based on:
    - Room availability.
    - Faculty workload limits.
    - Student section requirements.
  - Handle overlapping time slots and shared resources effectively.
- Faculty Workload:
  - Generate individual workload reports for faculty members.
  - Include details of subjects, units, and schedules assigned to each instructor.
Technical Specifications
- Frontend:
  - Blazor .NET Core WebAssembly for dynamic and responsive UI.
  - MudBlazor for consistent styling and design.
- Backend:
  - ASP.NET Core Web API for data processing and business logic.
  - SQL Server for relational data.
- Third-Party Integrations:
  - Auth0 Authentication for secure login.
  - SMTP services (e.g., SendGrid) for email confirmations.
        
Implementation Timeline
1. Phase 1: Authentication & User Management
   - Implement Google Login, registration, and email confirmation.
   - Role-based access control setup.
2. Phase 2: Core Academic Management
   - Develop features for courses, semesters, sections, and subjects.
3. Phase 3: Scheduling & Workload Management
   - Build conflict-free scheduling algorithms and faculty workload reports.
4. Phase 4: PWA Features
   - Enable offline viewing and installable web UI.
5. Phase 5: Testing & Deployment
   - Comprehensive testing (unit, integration, and user testing).
   - Deploy to production.
     
Conclusion
This Integrated Class Scheduling System aims to simplify academic scheduling and management for educational institutions while ensuring efficiency, scalability, and user convenience. With its modern features and robust architecture, the platform will serve as a reliable tool for administrators, faculty, and students alike.
