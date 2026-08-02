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
- **Authentication:** JWT (JSON Web Tokens) with Refresh Token and Antiforgery Tokens
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

For security, secret keys, configuration settings, and database connection strings are not stored in the repository. We use the `dotnet user-secrets` tool to manage them locally.

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

The key must be long and random. You can generate one with the following command in PowerShell:

```powershell
# Command to generate a secure key in PowerShell
[Convert]::ToBase64String((1..32 | % { [byte](Get-Random -Maximum 256) }))
```

Use the generated key in the following command:

```bash
dotnet user-secrets set "Jwt:Key" "PASTE_YOUR_GENERATED_KEY_HERE"
```

**d. Add the Refresh Token Secret Key:**

The hashing method for the refresh token requires a dedicated secret key. Generate a random key (using the same PowerShell command as above) and set it with:

```bash
dotnet user-secrets set "RefreshTokenSettings:SecretKey" "PASTE_YOUR_GENERATED_KEY_HERE"
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

---

## 🛡️ Token Security and Management

The API implements a robust security schema based on secure cookies and antiforgery tokens to mitigate risks such as XSS and CSRF:

1. **JWT (JSON Web Token):** 
   - Used for user authentication and authorization.
   - Has a lifetime of **1 hour** (`AuthExpiresMinutes` in configuration).
   - Transmitted via a cookie named `AuthToken` configured as `HttpOnly`, `Secure`, and `SameSite=None`. This prevents malicious JavaScript scripts from accessing the token (mitigating XSS attacks).

2. **Refresh Token:**
   - Used to renew the JWT once it expires, without forcing the user to log in again.
   - Has a lifetime of **45 days** (`RefreshExpiresDays` in configuration).
   - Transmitted as an `HttpOnly` cookie (`RefreshToken`).
   - For enhanced security, the refresh token is stored in the database as a **hash** (encrypted). The hashing process requires a secret key configured under `RefreshTokenSettings:SecretKey`.

3. **XSRF-TOKEN (Antiforgery Token):**
   - Protects state-changing endpoints (POST, PUT, DELETE) against Cross-Site Request Forgery (CSRF/XSRF) attacks.
   - Uses the *Double-Submit Cookie* pattern:
     - The server sets an antiforgery cookie named `XSRF-TOKEN` (not `HttpOnly`, so the frontend client can read it or handle it securely).
     - The API also returns the request token in the response body (`xsrfToken`) after a successful login or refresh.
     - The frontend client **must store this token** and send it in the `X-CSRF-TOKEN` HTTP header on every state-changing request.

---

## 📖 API Usage & Swagger Testing

Follow these steps to interact with the protected endpoints using the Swagger UI interface:

1. **Explore Endpoints:** Navigate to `https://localhost:7038/swagger` in your browser.
2. **User Registration:** Create a new user using the `POST /api/Users` endpoint.
3. **Log In:** Make a request to `POST /api/Authentication/login` with your credentials. 
   - The browser will automatically receive and store the `AuthToken` and `RefreshToken` cookies as `HttpOnly`.
   - The JSON response will contain the XSRF token:
     ```json
     {
       "message": "Autenticación exitosa.",
       "xsrfToken": "YOUR_XSRF_TOKEN"
     }
     ```
4. **Authorize Requests in Swagger:**
   - In the top-right corner of the Swagger page, click the **Authorize** button.
   - In the input field for the **XSRF** definition, paste only the `xsrfToken` value you copied from the response body.
   - Click **Authorize** and then **Close**.
5. **Perform Protected Requests:**
   - From this point on, Swagger will include the `X-CSRF-TOKEN` header with the provided value in every request.
   - Thanks to the custom script injected into Swagger UI, the browser will automatically send the `AuthToken` authentication cookie with every request.
6. **Token Renewal:**
   - If your JWT expires after an hour, make a request to `POST /api/Authentication/refresh`.
   - The API will automatically validate your refresh token cookie, rotate the token in the database, and return new cookies along with a new `xsrfToken` which you should update in Swagger's **Authorize** modal to continue your tests.