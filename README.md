# Chores Hub
Minimalist full-stack household chores management app with n-layered architecture.

<p align="center">
  <img src="./client/public/choreshub.gif" alt="choreshub">
</p>

## Table of Contents
* [Tech Stack](#tech-stack)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
* [Features](#features)

## Tech Stack

| Category | Technology |
| :--- | :--- |
| **Frontend** | ![React](https://img.shields.io/badge/React%2019-61DAFB?style=flat&logo=react&logoColor=black) ![Redux Toolkit](https://img.shields.io/badge/Redux%20Toolkit-764ABC?style=flat&logo=redux&logoColor=white) ![React Router](https://img.shields.io/badge/React%20Router-CA4245?style=flat&logo=react-router&logoColor=white) ![Vite](https://img.shields.io/badge/Vite-646CFF?style=flat&logo=vite&logoColor=white) |
| **Backend** | ![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=flat&logo=dotnet&logoColor=white) ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) ![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) ![JWT](https://img.shields.io/badge/JWT-black?style=flat&logo=JSON%20web%20tokens) |
| **Database** | ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)  |


## Getting Started
**Prerequisites:** Node.js, .NET 8 SDK, Entity Framework Core CLI tool and PostgreSQL.

**CLIENT**
- From `/client`.

   ```
   npm install
   ```
   ```
   npm start
   ```
- Vite dev server runs on port 3030 `(vite.config.js)`.

**SERVER**
- Entity Framework Core CLI tool required. From `/server`

  ```
  dotnet ef migrations add InitialCreate --project ChoresHub.Infrastructure --startup-project ChoresHub.WebAPI
  ```
  ```
  dotnet ef database update --project ChoresHub.Infrastructure --startup-project ChoresHub.WebAPI
  ```

- .NET 8 SDK required. Run with dotnet run in server/ChoresHub.WebAPI/.

   ```
   dotnet run
   ```


## Configuration

### CLIENT
- API URL is set via VITE_API_BASE_URL in `.env.development` and `.env.production`.

### Server
- Configure `server/ChoresHub.WebAPI/appsettings.Development.json` for local development.
- Configure `server/ChoresHub.WebAPI/appsettings.Production.json` for production.
- Required settings include:
  - `ConnectionStrings:ChoresHubDb`
  - `JwtSettings` (`Issuer`, `Audience`, `SecretKey`)
  - `CloudinarySettings` (`CloudName`, `ApiKey`, `ApiSecret`)
  - `EmailSettings` (`SmtpHost`, `SmtpPort`, `SenderName`, `SenderEmail`, `SenderPassword`)
  - `Cors:AllowedOrigins`
  - `ClientApp:BaseUrl`

## Features
- Register and log in
- Create tasks, notifications, and shopping lists
- Update profile (photo, name, email, password)
- Reset password
- Mark others' tasks as done
- Mark others' notifications as seen
- Mark others' shopping items as bought
- Delete account



