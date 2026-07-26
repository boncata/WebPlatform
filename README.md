# WebPlatform

WebPlatform is a project for building an online book-selling system, whose backend is built using C#, ASP.NET Core Web API, Entity Framework Core, and PostgreSQL, while the frontend
is developed with JavaScript, React, Vite and Axios.

The goal of the project is to create a professional backend architecture for managing books, supporting CRUD operations through a REST API, persistent database storage, automated testing, and later containerization and frontend integration.

---

# Tech Stack

## Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- xUnit
- Moq
- Swagger / OpenAPI

## Frontend
- JavaScript
- React
- Vite
- Axios

---

# Backend Requirements

The requirements and their installation procedure are planned to be automated,
at least to a degree, when the project is updated to run inside a container.

## .NET SDK
The project requires .NET 10. For more information, see the
[official documentation](https://dotnet.microsoft.com/en-us/download).

## Additional .NET Packages
The API requires Entity Framework CLI Tools. To run the tests, you need xUnit and Moq.
These requirements are outline in the ``.cproj`` file of each project.

The EF Core CLI tool (``dotnet ef``) is not bundled with the .NET SDK and must
be installed separately:

```Bash
dotnet tool install --global dotnet-ef
```

## PostgreSQL
After installing PostgreSQL, it needs to be configured with the following instructions.

### Database Configuration
Open PostgreSQL:

```Bash
sudo -u postgres psql
```

Create database (inside the PostgreSQL console):

```SQL
CREATE DATABASE webplatform;
```

Create dedicated application user (and use a different strong password):

```SQL
CREATE USER webplatform_user
WITH PASSWORD 'YourStrongPassword';
```

Grant database permissions:

```SQL
GRANT ALL PRIVILEGES
ON DATABASE webplatform
TO webplatform_user;
```

Connect to the database:

```SQL
\c webplatform
```

Grant schema permissions:

```SQL
GRANT ALL ON SCHEMA public TO webplatform_user;
ALTER SCHEMA public OWNER TO webplatform_user;
```

Then exit the PostgreSQL console.

### Database Migration
From the project root, run:

```Bash
dotnet ef database update --project backend/WebPlatform.Api
```

## Configuration
The connection string is **not** stored in ``appsettings.json`` — that file is
committed to source control and must never contain real credentials. Instead,
provide it via the ``ConnectionStrings__DefaultConnection`` environment
variable (the double underscore represents the ``:`` nesting separator that
.NET configuration uses internally, since most shells don't allow ``:`` in
variable names).

Before running the backend or its tests, export the connection string in
your shell, using the ``username``/``password`` you configured above:

```Bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=webplatform;Username=webplatform_user;Password=YourStrongPassword"
```

On Windows PowerShell, the equivalent is:

```PowerShell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=webplatform;Username=webplatform_user;Password=YourStrongPassword"
```

This only lasts for the current terminal session. To avoid re-exporting it
every time, add the ``export`` line to your shell's startup file (e.g.
``~/.bashrc`` or ``~/.zshrc``) — just make sure that file itself is never
committed anywhere shared.

---

# Frontend Requirements
The frontend is located in ``frontend``.

## Node.js

Install ``Node.js`` and ``npm``. This project is currently built using Node.js,
version 24 (LTS).

## Project dependencies
Navigate to the frontend directory ``frontend``. Install the required packages:

```Bash
npm install
```

---

# Run WebPlatform

## Backend (API)
Make sure ``ConnectionStrings__DefaultConnection`` is exported in your
current shell (see the Configuration section above), then, from the project
root, run:

```Bash
dotnet run --project backend/WebPlatform.Api
```

The API will start and Swagger UI will be available at:
``http://localhost:5130/swagger``

## Frontend
Start the development server:

```Bash
npm run dev
```

The app will be available at:
``http://localhost:5173``

---

# Run Tests

## Backend
The integration tests run against the real database configured above, so
``ConnectionStrings__DefaultConnection`` must be exported in your shell
first (see the Configuration section). From the project root, run:

```Bash
dotnet test
```

This runs all automated tests inside ``WebPlatform.Tests``.

## Frontend
From the ``frontend`` folder, run:

```Bash
npm run test
```

This runs all automated tests inside ``webplatform-ui``.