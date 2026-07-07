# Regulatory Compliance System

A web-based Regulatory Compliance Management System built with **ASP.NET Core MVC** during a BT Group internship/training project.

The application enables organizations to manage regulatory compliance documents, monitor expiry dates, maintain document version history, and automatically notify responsible users before important documents expire.

---

## Features

### Authentication & Authorization

- Cookie-based Authentication
- Role-based Authorization
- Administrator and Management roles
- Secure login and logout

---

### Document Management

- Create regulatory documents
- Edit document metadata
- Renew existing documents
- Version history
- File upload and download
- Soft delete support

Supported document types include:

- Commercial Registrations
- Activity Licenses
- Certificates
- Other regulatory documents

---

### Dashboard

Interactive dashboard displaying:

- Valid documents
- Expiring documents
- Expired documents

Visualization includes:

- ApexCharts Doughnut Chart
- ApexCharts Bar Chart
- KPI Summary Cards

---

### Notification System

Automatic expiry notifications powered by Hangfire.

Features:

- Daily scheduled notification generation
- Duplicate notification prevention
- Individual notifications for each recipient
- Notification bell with unread count
- Notification history
- Mark individual notifications as read
- Mark all notifications as read

---

### Email Notifications

Email delivery is implemented using **MailKit**.

Notification flow:

```
Hangfire
      ↓
NotificationService
      ↓
Database Notification
      ↓
Attempt Email Delivery
```

Email failures never prevent notification creation.

---

### Audit Logging

Field-level audit logging records:

- Document creation
- Renewals
- Metadata changes

Notifications are intentionally excluded from auditing.

---

## Architecture

The application follows a clean **three-layer architecture**.

```
Web
    ↓
Core
    ↑
Infrastructure
```

### RegulatoryComplianceApplication (Web)

- MVC Controllers
- Razor Views
- ViewModels
- Authentication
- Dependency Injection

### RegulatoryComplianceApplication.Core

- Entities
- Interfaces
- Business Contracts

### RegulatoryComplianceApplication.Infrastructure

- Entity Framework Core
- SQL Server
- Services
- Hangfire
- Email
- File Storage

Controllers never access `AppDbContext` directly.

All business logic resides inside services.

---

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Hangfire
- ApexCharts
- Bootstrap 5
- Bootstrap Icons
- MailKit
- Cookie Authentication

---

## Database

Main entities:

- Users
- Roles
- Documents
- Document Types
- Document Versions
- Responsible Users
- Notifications
- Audit Logs

---

## Notification Workflow

```
Daily Hangfire Job
        ↓
Get Expiring Documents
        ↓
Create Notifications
        ↓
Prevent Duplicates
        ↓
Save Notifications
        ↓
Attempt Email Delivery
```

Only unread notifications prevent duplicate reminders.

Once a notification is marked as read, a future reminder may be generated again.

---

## Security

Implemented:

- Cookie Authentication
- Role-based Authorization
- Anti-forgery Tokens
- Service-based Architecture
- SQL Server using Entity Framework Core

---

## Project Structure

```
RegulatoryComplianceApplication

├── RegulatoryComplianceApplication
│   ├── Controllers
│   ├── Jobs
│   ├── ViewComponents
│   ├── ViewModels
│   ├── Views
│   └── wwwroot
│
├── RegulatoryComplianceApplication.Core
│   ├── Entities
│   └── Interfaces
│
└── RegulatoryComplianceApplication.Infrastructure
    ├── Configuration
    ├── Data
    ├── Services
    └── Migrations
```

---

## Current Features

- Authentication
- Authorization
- Dashboard
- ApexCharts Dashboard
- Document Creation
- Document Renewal
- Version History
- File Upload
- File Download
- Notification System
- Notification Bell
- Hangfire Background Jobs
- Email Service
- Audit Logging
- GitHub CI/CD

---

## Future Improvements

Possible future enhancements include:

- Password Reset
- User Registration
- Dashboard Filtering
- Notification Dropdown
- Administrator-only Hangfire Dashboard
- Configurable Notification Threshold
- SignalR Real-Time Notifications
- Improved Reporting

---

## Screenshots

<img width="1912" height="904" alt="image" src="https://github.com/user-attachments/assets/7a79083f-ccbb-4f7a-9631-e78edb6a1f89" />
<img width="1919" height="900" alt="image" src="https://github.com/user-attachments/assets/51e4a694-d883-4bda-b863-fa8ccb178b84" />
<img width="1919" height="899" alt="image" src="https://github.com/user-attachments/assets/0cdad4b0-93bd-4d68-90f4-6d10589ebc25" />
<img width="1919" height="895" alt="image" src="https://github.com/user-attachments/assets/6321c457-3f84-4963-87d5-5b52378a4427" />


---

## Author

**Abdulrazaq AlSayed Ahmad**

Computer Science Student

King Fahd University of Petroleum and Minerals (KFUPM)

Developed as part of a BT Group Internship/Training Project.

---
