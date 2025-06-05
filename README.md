# Welcome to Team Isengards Tabletennis school-project!  

# Project description:
A modern web application for Ängby Table Tennis Club that provides comprehensive management of the club's activities.  
The system is developed with .NET 9 and follows modern web standards.

---

## Trello-board  

[Trello](https://trello.com/invite/b/68133273e650052b63ef675d/ATTI4a3fd60004a259d593032d6bf9eacb48029733F3/angby-pingis-isengard)

## Working agreement and team decision-board  

[Miro](https://miro.com/app/board/uXjVI7bk488=/)  

## Authors

**Fakhara Imran**   [Github](https://github.com/fakhara)  
**Rut Frisk**       [Github](https://github.com/ArrenCelion)  
**Jan Hamrin**      [Github](https://github.com/jaham88)  
**Karl Westergren** [Github](https://github.com/Brottarbengt)  


## Acknowledgements

- [**Unsplash**](https://unsplash.com/): Free to use pictures 
- [**Fontawesome**](https://fontawesome.com/): Free to use icons
- [**Googlefonts**](https://fonts.google.com/): Free to use fonts

---

## Developing Methods
- **SCRUM Framework**:
  - Weekly sprints with clear sprint goals
  - Daily stand-ups for progress tracking
  - Sprint planning and retrospectives
  - Backlog refinement sessions
  - Continuous integration and delivery

- **Project Management**:
  - Kanban board in Trello for task tracking
  - User stories and acceptance criteria
  - Task prioritization and estimation
  - Visual progress tracking
  - Team collaboration and transparency

- **Development Workflow**:
  - Feature branch development
  - Code reviews
  - Pair programming sessions
  - Regular team sync-ups
  - Continuous feedback loops

## Main Features
- **Match Management**: Registration and updating of matches with detailed statistics
- **Player Management**: Registration and updating of players
- **Player Statistics**: Comprehensive statistics and history for each player
- **User Management**: Secure login and role-based access control

## Technical Stack
- **Frontend**: Razor Pages and Javascript
- **Backend**: .NET 9, Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity

## Patterns and Principals

### Architecture Patterns
- **Clean Architecture**: Separation of concerns with distinct layers (Presentation, Business Logic, Data Access)
- **Repository Pattern**: Abstract data access layer for database operations
- **Dependency Injection**: Loose coupling and testability through IoC container

### Design Patterns
- **Razor Pages Pattern**: Page-based UI framework with built-in separation of concerns
- **DTO Pattern**: Data Transfer Objects for API communication
- **Adapter Pattern**: Using Mapster for object mapping
- **Service Pattern**: Business logic encapsulation in service classes
- **Factory Pattern**: Object creation and configuration

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
  - Example: `SetService` handles only set-related operations
  - Example: `PlayerService` manages player-specific functionality
  - Example: `MatchService` focuses solely on match management

- **Open/Closed**: Open for extension, closed for modification
  - Example: `PagedResultBase` abstract class allows extension through inheritance
  - Example: Service interfaces (`ISetService`, `IMatchService`, `IPlayerService`) enable new implementations without modifying existing code

- **Liskov Substitution**: Derived classes can substitute base classes
  - Example: `PagedResult<T>` extends `PagedResultBase` and can be used interchangeably
  - Example: Service implementations can be swapped without affecting the application

- **Interface Segregation**: Client-specific interfaces
  - Example: `ISetService` defines only set-related operations
  - Example: `IPlayerService` contains only player-specific methods
  - Example: `IMatchService` focuses on match-related functionality

- **Dependency Inversion**: High-level modules don't depend on low-level modules
  - Example: Services depend on interfaces (`ISetService`, `IMatchService`) rather than concrete implementations
  - Example: Dependency injection in `Program.cs` for service registration
  - Example: Constructor injection in services (`MatchService`, `PlayerService`, `SetService`)

### Best Practices
- **Async/Await**: Asynchronous programming for better performance
- **Error Handling**: Consistent error handling and logging
- **Security**: Authentication and authorization using ASP.NET Core Identity
- **Testing**: Unit tests and integration tests for critical functionality

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

