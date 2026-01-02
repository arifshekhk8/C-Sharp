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


Step 1: Prerequisites

Make sure the following software is installed on your system:
	•	.NET SDK (version 8.0 or later)
	•	Git
	•	SQLite

Check .NET installation:
dotnet --version

Step 2: Clone the Repository
git clone https://github.com/arifshekhk8/C-Sharp.git
cd C-Sharp

Step 3: Restore Dependencies
dotnet restore

Step 4: Apply Database Migrations
dotnet ef database update

Step 5: Run the Application
dotnet run
