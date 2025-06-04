# Welcome to Team Isengards Tabletennis school-project!  

# Project description:
A modern web application for Ängby Table Tennis Club that provides comprehensive management of the club's activities.  
The system is developed with .NET 9 and follows modern web standards.

## Main Features
- **Match Management**: Registration and updating of matches with detailed statistics
- **Player Statistics**: Comprehensive statistics and history for each player
- **Tournament Management**: Create and administer tournaments
- **User Management**: Secure login and role-based access control
- **Reporting**: Generate and export statistics and reports

## Technical Stack
- **Backend**: .NET 9, Entity Framework Core
- **Frontend**: React with TypeScript
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity
- **API**: RESTful API with Swagger documentation

---

## Trello-board  

[Trello](https://trello.com/b/ZAZXhoa2/angby-pingis-isengard)  

## Working agreement and team decision-board  

[Miro](https://miro.com/app/board/uXjVI7bk488=/)  

---

## Installation instructions:

#### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or later
- SQL Server (local or Azure)
- Git

#### Installation Steps

1. Clone the project
```bash
git clone https://github.com/[your-repo]/Isengard-Tabetennis.git
cd Isengard-Tabetennis
```

2. Open the solution file
- Open `Isengard-Tabletennis.sln` in Visual Studio

3. Restore NuGet packages
- Right-click on the solution in Solution Explorer
- Select "Restore NuGet Packages"

4. Configure the database
- Update the connection string in `appsettings.json` with your database settings
- Run the following commands in Package Manager Console:
```powershell
Update-Database
```

5. Start the project
- Press F5 or click "Start" in Visual Studio
- Alternatively, run from terminal:
```bash
dotnet run
```

#### Troubleshooting
- If you encounter issues with NuGet packages, try cleaning the solution and restoring packages again
- Verify that you have the correct version of .NET SDK installed
- Ensure SQL Server is running and accessible

---

