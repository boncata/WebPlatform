# WebPlatform

WebPlatform is a project for building an online book-selling backend system using C#, ASP.NET Core Web API, Entity Framework Core, and PostgreSQL.

The goal of the project is to create a professional backend architecture for managing books, supporting CRUD operations through a REST API, persistent database storage, automated testing, and later containerization and frontend integration.

---

# Tech Stack

- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- xUnit
- Moq
- Swagger / OpenAPI

---

# Requirements

The requirements and their installation procedure are planned to be automated,
at least to a degree, when the project is updated to run inside a container.

## .NET SDK
The project requires .NET 10. For more information, see the
[official documentation](https://dotnet.microsoft.com/en-us/download).

## Additional .NET Packages
The API requires Entity Framework CLI Tools. To run the tests, you need xUnit and Moq.
These requirements are outline in the ``.cproj`` file of each project.

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
dotnet ef database update --project src/WebPlatform.Api
```

---

# Configuration
Update the "DefaultConfiguration" field in ``src/WebPlatform.Api/appsettings.json``
to reflect the ``username`` and ``password`` set in the previous section.

---

# Run the API
From the project root, run:

```Bash
dotnet run --project src/WebPlatform.Api
```

The API will start and Swagger UI will be available at:
``http://localhost:5130/swagger``

---

# Run Unit Tests
From the project root, run:

```Bash
dotnet test
```

This runs all automated tests inside ``WebPlatform.Tests``.