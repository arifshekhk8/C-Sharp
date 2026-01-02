# Library Management System (ASP.NET Core MVC)

A full-stack **Library Management System** built using **ASP.NET Core MVC**.  
This project supports user authentication, book management, loan requests, and an admin dashboard with role-based access.

---

## Features

### User Features
- User registration and login
- Browse available books
- Search books by title, author, or category
- Request book loans
- View loan history and status

### Admin Features

admin@library.com
Admin@123
- Admin dashboard with system overview
- Manage books (Create, Read, Update, Delete)
- Manage users (Edit / Delete)
- Manage loan requests
- Category-based book management
  - Newly Arrived
  - Most Popular
  - Normal
- Role-based authorization (Admin / User)

---

## Technologies Used

- **ASP.NET Core MVC**
- **C#**
- **Entity Framework Core**
- **SQLite**
- **Razor Views**
- **Bootstrap**
- **Git & GitHub**

---

## Project Structure

```text
LibraryManagementSystem
│
├── Areas/               # Admin area
├── Controllers/         # MVC controllers
├── Models/              # Entity models
├── ViewModels/          # View-specific models
├── DTOs/                # Data transfer objects
├── Data/                # Database context
├── Services/            # Business logic
├── Views/               # Razor views
├── wwwroot/             # Static files (CSS, JS)
├── Program.cs           # Application startup
├── appsettings.json     # Configuration
└── LibraryManagementSystem.csproj
