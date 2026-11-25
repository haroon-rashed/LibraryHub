# LibraryHub

A small library lending system built as three .NET 10 projects: a Web API backend, an MVC frontend and a shared Common library. A librarian can manage books and members, lend books, return them and see overdue loans with late fees.

## Solution structure

| Project | Type | Purpose |
|---|---|---|
| `LibraryHub.Common` | Class library | DTOs, `ApiResponse<T>` and `PagedResult<T>` wrappers, `LoanStatus` enum, loan rule constants |
| `LibraryHub.Api` | ASP.NET Core Web API | Entities, `DbContext`, migrations, services, controllers, exception middleware |
| `LibraryHub.Web` | ASP.NET Core MVC | Controllers, ViewModels, Razor views, one typed API client |

`Web` and `Api` never reference each other. Both reference only `Common`. The Web project gets all its data by calling the API over HTTP.

## Prerequisites

- .NET 10 SDK
- MySQL Server 8 (running locally)
- EF Core command line tool, version 10: `dotnet tool install --global dotnet-ef --version 10.*`

## Setup

1. **Clone the repository** and open `LibraryHub.sln` in Visual Studio (or use the command line).

2. **Set the database connection.** `LibraryHub.Api/appsettings.json` contains a placeholder connection string. Create `LibraryHub.Api/appsettings.Development.json` (it is git-ignored) with your real MySQL password:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=LibraryHubDb;User=root;Password=your_password;"
     }
   }
   ```

3. **Create the database** with the migrations. Run from the solution folder:

   ```bash
   dotnet ef database update --project LibraryHub.Api
   ```

   This creates `LibraryHubDb` and seeds 10 books, 5 members and 2 loans (one of them overdue).

4. **Check the API address in the Web project.** `LibraryHub.Web/appsettings.json` holds the API base URL (`ApiSettings:BaseUrl`, default `https://localhost:7123/`). It must match the HTTPS address in `LibraryHub.Api/Properties/launchSettings.json`.

## Running

Start the **API first, then the Web project**.

- In Visual Studio: right-click the solution → Configure Startup Projects → Multiple startup projects → set Api, then Web, to Start, and press F5.
- Or with the command line, in two terminals:

  ```bash
  dotnet run --project LibraryHub.Api --launch-profile https
  dotnet run --project LibraryHub.Web --launch-profile https
  ```

| What | URL |
|---|---|
| API Swagger UI | `https://localhost:7123/swagger` |
| Web app | the HTTPS address in `LibraryHub.Web/Properties/launchSettings.json` |

## Features

**Web pages:** Dashboard (`/`), Books (`/Books`), Book form (`/Books/Create`, `/Books/Edit/{id}`), Members (`/Members`), Member form (`/Members/Create`, `/Members/Edit/{id}`), Loans (`/Loans`, filters All / Active / Overdue), Borrow a book (`/Loans/Create`), Error page (`/Home/Error`).

**API endpoints** (all return `ApiResponse<T>`):

| Method | Route | Status codes |
|---|---|---|
| GET | `/api/books?search=&page=&pageSize=&availableOnly=` | 200 |
| GET / POST | `/api/books`, `/api/books/{id}` | 200, 201, 400, 404, 409 |
| PUT / DELETE | `/api/books/{id}` | 200, 204, 400, 404, 409 |
| GET | `/api/members?search=&page=&pageSize=&activeOnly=` | 200 |
| GET / POST | `/api/members`, `/api/members/{id}` | 200, 201, 400, 404, 409 |
| PUT / DELETE | `/api/members/{id}` | 200, 204, 400, 404, 409 |
| GET | `/api/loans?status=all\|active\|overdue` | 200, 400 |
| POST | `/api/loans` (borrow) | 201, 400, 404, 409 |
| POST | `/api/loans/{id}/return` | 200, 404, 409 |
| GET | `/api/dashboard` | 200 |

**Business rules** (enforced in the API services, constants in `LibraryHub.Common/Constants/LoanRules.cs`):

1. A member can have at most 3 active loans.
2. A book can be borrowed only when copies are available. Borrowing lowers available copies by 1 and returning raises it by 1.
3. An inactive member cannot borrow.
4. The loan period is 14 days.
5. On return, the late fee is 10 per overdue day (0 if on time) and is stored on the loan.
6. A member cannot hold two active loans of the same book.
7. A book or member with active loans cannot be deleted.
8. ISBN and email must be unique.
9. Returning an already returned loan is an error.

A failed rule returns a clear message with the right status code (409 for broken rules, 404 for missing items, 400 for invalid input), never a 500.

## Technical notes

- Services (`IBookService`, `IMemberService`, `ILoanService`, `IDashboardService`) use EF Core's `DbContext` directly and are registered through dependency injection. Controllers stay thin.
- Services return a `ServiceResult<T>`, and a base controller turns it into the right HTTP response.
- One global exception middleware returns an `ApiResponse` with status 500 and no stack trace. Details are written to the log.
- `ILogger` logs borrows, returns, rejected borrows and unhandled errors.
- The Web project calls the API through one typed client, `LibraryApiClient`, registered with `IHttpClientFactory`.
- Validation happens in layers: data annotations on DTOs and ViewModels, business rules in the services, and the API's messages shown in the UI.

## Assumptions

- The database is MySQL (via `MySql.EntityFrameworkCore`), confirmed with the project reviewer.
- Ids are integers.
- Dates are stored in UTC. A loan is overdue when its due date is earlier than the current time, and the late fee counts whole calendar days.
- "Active loans" means loans without a return date, so overdue loans are included in the active count on the dashboard.
- Deleting a book or member that has no active loans also deletes its returned-loan history.
- When a book is edited, the number of copies currently on loan is kept, and total copies cannot be set below it.
- Duplicate ISBN or email returns 409 Conflict with a message. Email and ISBN comparison follows the database collation (case-insensitive by default in MySQL).
- The UI shows 5 items per page. The API accepts a page size between 1 and 100.
- The seeded overdue loan has a fixed due date (15 Nov 2026). The second seeded loan will also become overdue once its due date (4 Dec 2026) passes.

## Out of scope

Login and roles, JavaScript frameworks, Docker, deployment and email notifications.