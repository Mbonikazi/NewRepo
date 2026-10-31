# ?? RaceDay Event Management System

## PROG6212 - Programming 2B | Final Portfolio of Evidence (Parts 1, 2 & 3)

**Student Name:** [Your Name]
**Student Number:** [Your Student Number]

## Project Description
RaceDay is a comprehensive event management system built using a 3-tier architecture. It supports Organisers creating and managing events, and Participants enrolling and viewing results. It features Azure Blob Storage integration for image uploads, and the front-end is fully containerized using Docker.

### Architecture:
1. **RaceDay.Api (Part 2):** A RESTful API built with .NET 8, EF Core Code-First, SQL Server, and Session-based authentication with role enforcement. It handles Azure Blob Storage uploads.
2. **RaceDay.Mvc (Part 3):** An ASP.NET Core MVC web front end that consumes the API. It uses session-based authentication, role-specific interfaces, and handles image uploads via the API.
3. **RaceDay.Contracts:** Shared DTOs between the API and MVC projects.

### User Roles
- **Organiser:** Can create events, upload event banner images, manage categories, view enrolments, and capture results.
- **Participant:** Can browse upcoming events, enrol in events, view their results, and manage their profile.

## Technologies Used
- .NET 8 Web API & MVC
- Entity Framework Core 8 (Code First)
- SQL Server / SSMS
- Azure Blob Storage
- Docker (Linux Containers)
- Swashbuckle (Swagger)
- xUnit
- GitHub Actions

## Project Structure
# ?? RaceDay Event Management System

## PROG6212 - Programming 2B | Final Portfolio of Evidence (Parts 1, 2 & 3)

**Student Name:** [Your Name]
**Student Number:** [Your Student Number]

## Project Description
RaceDay is a comprehensive event management system built using a 3-tier architecture. It supports Organisers creating and managing events, and Participants enrolling and viewing results. It features Azure Blob Storage integration for image uploads, and the front-end is fully containerized using Docker.

### Architecture:
1. **RaceDay.Api (Part 2):** A RESTful API built with .NET 8, EF Core Code-First, SQL Server, and Session-based authentication with role enforcement. It handles Azure Blob Storage uploads.
2. **RaceDay.Mvc (Part 3):** An ASP.NET Core MVC web front end that consumes the API. It uses session-based authentication, role-specific interfaces, and handles image uploads via the API.
3. **RaceDay.Contracts:** Shared DTOs between the API and MVC projects.

### User Roles
- **Organiser:** Can create events, upload event banner images, manage categories, view enrolments, and capture results.
- **Participant:** Can browse upcoming events, enrol in events, view their results, and manage their profile.

## Technologies Used
- .NET 8 Web API & MVC
- Entity Framework Core 8 (Code First)
- SQL Server / SSMS
- Azure Blob Storage
- Docker (Linux Containers)
- Swashbuckle (Swagger)
- xUnit
- GitHub Actions

## Project Structure
RaceDay/
??? .github/workflows/ci.yml
??? docs/ (Part 1 planning)
??? RaceDay.Api/ (Web API + Models + Data + Controllers + Blob Service)
??? RaceDay.Contracts/ (DTOs)
??? RaceDay.Api.Tests/ (xUnit tests)
??? RaceDay.Mvc/ (Part 3 MVC Web Front End + Dockerfile)
??? README.md


---

## ?? Azure Blob Storage Integration
Azure Blob Storage is used to store event banner images.
**Crucial Architecture Rule:** The MVC application NEVER connects to Azure directly. It sends files to the API, and the API handles the upload to Azure and saves the URL to the SQL database.

**Setup:**
1. Create an Azure Storage Account and a container named `raceday-images` (set to Blob anonymous access).
2. Add your Connection String to `RaceDay.Api/appsettings.json`.

---

## ?? How to Run the Application

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express)
- Docker Desktop (for containerization)
- Azure Storage Account (Connection string required in API appsettings)

### Database Setup
1. Ensure SQL Server (Express) is running.
2. Update `RaceDay.Api/appsettings.json` if needed.
3. Package Manager Console:


### 1. Run the API
Open a terminal in the solution root and run:
```bash
dotnet run --project RaceDay.Api --urls http://0.0.0.0:5000

docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc:part3 .
docker run -d --name raceday-mvc -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:5000/ raceday-mvc:part3

Open your browser and navigate to: http://localhost:8080

API Endpoints
Method	Endpoint	Role
POST	/api/auth/register	Public
POST	/api/auth/login	Public
POST	/api/auth/logout	Any
GET	/api/profile/me	Any
PUT	/api/profile/me	Any
GET	/api/events	Any
GET	/api/events/{id}	Any
GET	/api/events/mine	Organiser
POST	/api/events	Organiser
PUT	/api/events/{id}	Organiser (owner)
DELETE	/api/events/{id}	Organiser (owner)
POST	/api/events/{id}/banner	Organiser (owner)
GET	/api/events/{eventId}/categories	Any
POST	/api/events/{eventId}/categories	Organiser (owner)
PUT	/api/categories/{id}	Organiser (owner)
DELETE	/api/categories/{id}	Organiser (owner)
POST	/api/enrolments	Participant
GET	/api/enrolments/mine	Participant
GET	/api/enrolments/event/{eventId}	Organiser (owner)
PUT	/api/enrolments/{id}/status	Organiser (owner)
POST	/api/results	Organiser (owner)
GET	/api/results/mine	Participant
GET	/api/results/event/{eventId}	Organiser (owner)
Swagger
Navigate to /swagger after starting the API.
https://github.com/Mbonikazi/RaceDay/blob/d630c17f3dffcf8be87aa0172d003577fedadd82/Swagger%20image.jpeg
https://github.com/Mbonikazi/RaceDay/blob/64d2b593c81c2ccac4ea122caa87eb77a0d5f776/Swagger%20image2.jpeg
https://github.com/Mbonikazi/RaceDay/blob/2496ffe942d7622bfdb67a02e1a8ca3a1114f279/Screenshot_11-10-2026_15279_localhost.jpeg

?? Unit Testing
The test project RaceDay.Api.Tests uses xUnit with EF Core InMemory for isolated, dependency-free tests.

Test Coverage
Test File	What It Tests
PasswordHasherTests.cs	Hashing, verification, case sensitivity
AuthTests.cs	Registration, login, duplicate email detection, role assignment
RoleAndSessionTests.cs	Role enforcement, session keys, access rules
ModelRelationshipTests.cs	Entity relationships (1-to-many, 1-to-1), ownership rules
?? CI/CD – GitHub Actions
The repository includes a CI workflow at .github/workflows/ci.yml. It runs automatically on every push and pull request to master or main.

Workflow Steps
? Checkout repository

? Setup .NET 8 SDK

? Restore NuGet packages

? Build solution in Release mode

? Run all unit tests

Green Build
You can view the workflow history on the GitHub Actions tab.

CI Screenshot:
https://github.com/Mbonikazi/RaceDay/blob/42aa11453932c92f21703b006ae296fa48b179c9/Screenshot%202026-10-09%20140747.png

Video Presentation
Unlisted YouTube: [Insert Link to your Part 3 Video here]

text

### Steps after pasting:
1. Put your actual Name and Student Number at the top.
2. Add your YouTube link at the bottom when your video is done.
3. Press **`Ctrl + S`** to save the file.
4. Go to **Git Changes**, type `Combined Part 2 and Part 3 README`, click **Commit All**, and push the **Up Arrow** to GitHub. 

Your README is now perfect and covers every single requirement for the final submission!
