# Expense Tracking API - Azul School Project

This repository contains the source code for a RESTful API for personal expense tracking, developed as a practice project for the Web API with ASP.NET Core course at Azul School.

The API allows users to register, manage their money accounts, record transactions (income and expenses), and transfer funds between their accounts.

## ✨ Key Features

- **Secure Authentication:** Implementation of authentication based on JSON Web Tokens (JWT).
- **User Management:** Creation and management of user profiles.
- **Money Accounts:** Creation of multiple "sections" or money accounts per user.
- **Transaction Recording:** Recording of income and expenses associated with categories.
- **Internal Transfers:** Movement of funds between the user's own accounts.
- **Queries and Listings:** Endpoints for querying transactions by category and transfers.
- **API Documentation:** Swagger UI interface to explore and test endpoints interactively.

## 🛠️ Technologies Used

- **Framework:** .NET 8.0
- **Language:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 8
- **Authentication:** JWT (JSON Web Tokens)
- **Architecture:** RESTful API

---

## 🚀 Getting Started (Local Setup)

Follow these steps to clone and run the project on your local machine.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) installed and running.
- A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/).
- [Git](https://git-scm.com/downloads)

### 1. Clone the Repository# Expense Tracking API - Azul School Project

This repository contains the source code for a RESTful API for personal expense tracking, developed as a practice project for the Web API with ASP.NET Core course at Azul School.

The API allows users to register, manage their money accounts, record transactions (income and expenses), and transfer funds between their accounts.

## ✨ Key Features

- **Secure Authentication:** Implementation of authentication based on JSON Web Tokens (JWT).
- **User Management:** Creation and management of user profiles.
- **Money Accounts:** Creation of multiple "sections" or money accounts per user.
- **Transaction Recording:** Recording of income and expenses associated with categories.
- **Internal Transfers:** Movement of funds between the user's own accounts.
- **Queries and Listings:** Endpoints for querying transactions by category and transfers.
- **API Documentation:** Swagger UI interface to explore and test endpoints interactively.

## 🛠️ Technologies Used

- **Framework:** .NET 8.0
- **Language:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 8
- **Authentication:** JWT (JSON Web Tokens)
- **Architecture:** RESTful API

---

## 🚀 Getting Started (Local Setup)

Follow these steps to clone and run the project on your local machine.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) installed and running.
- A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/).
- [Git](https://git-scm.com/downloads)

### 1. Clone the Repository

```bash
git clone <REPOSITORY_URL>
cd AzulSchoolProject
```

### 2. Configure Application Secrets

For security, secret keys and the connection string are not stored in the repository. We use the `dotnet user-secrets`  tool to manage them locally.

**a. Initialize User Secrets:**

```bash
dotnet user-secrets init
```

**b. Add the Connection String to your PostgreSQL database:**

> **Note:** Ensure you have created an empty database in PostgreSQL for this project.

```bash
# Replace the values with your PostgreSQL configuration
dotnet user-secrets set "ConnectionStrings:ProjectServer" "Host=localhost;Port=5432;Database=YourDBName;Username=your_user;Password=your_password"
```

**c. Add the JWT Secret Key:**

The key must be long and random. You can generate one with the following command in PowerShell and use the result:

```powershell
# Command to generate a secure key in PowerShell
[Convert]::ToBase64String((1..32 | % { [byte](Get-Random -Maximum 256) }))
```

Use the generated key in the following command:

```bash
dotnet user-secrets set "Jwt:Key" "PASTE_YOUR_GENERATED_KEY_HERE"
```

### 3. Apply Database Migrations

This will create the database schema (tables, relationships, etc.) based on the application models.

```bash
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:7038` (or the port indicated in the console).

## 📖 API Usage

1.  **Explore Endpoints:** Navigate to `https://localhost:7038/swagger` to view the interactive API documentation.
2.  **Register:** Create a new user using the `POST /api/Users` endpoint.
3.  **Log In:** Use the `POST /api/Authentication/login` endpoint with the email and password of the user you created. The response will contain your JWT token.
4.  **Authorize your Requests:** In the top right of Swagger, click the "Authorize" button. In the dialog, enter `Bearer ` followed by your token (e.g., `Bearer eyJhbGciOi...`) and click "Authorize".
5.  **Use the Protected Endpoints:** You can now use the rest of the endpoints that require authentication. Swagger will automatically include the token in each request.