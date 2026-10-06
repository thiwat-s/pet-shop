# Pet Shop Management System

A full-stack assessment project for managing pet inventory. It provides a JWT-protected ASP.NET Core API and a React/TypeScript dashboard for authenticating, viewing, creating, editing, deleting, and refreshing pets.

## Stack

- Backend: .NET 8, ASP.NET Core, Dapper
- Frontend: React, TypeScript, Vite
- Persistence: JSON files under `backend/PetShop.Api/Data`

## Structure

```
backend/PetShop.Api/    API, domain models, services, repositories, JSON data
frontend/               React application
```

The application uses repository interfaces so the active JSON persistence implementation can be replaced later. Dapper is included through `DapperPetRepository`, which is the SQL implementation seam for a future relational database. JSON files are deliberately not treated as relational data or queried with Dapper.

## Prerequisites

- .NET SDK 8
- Node.js 20 or later

## Run the backend

```powershell
dotnet restore .\PetShop.sln
dotnet run --project .\backend\PetShop.Api --urls http://localhost:5000
```

On first start, the API creates `Data/users.json` and `Data/pets.json` with sample records. The JSON repository uses a process-level write lock and temporary-file replacement for pet writes.

## Run the frontend

```powershell
cd frontend
npm install
npm run dev
```

Open the URL printed by Vite. The development API endpoint is `http://localhost:5000/api`.

## Environment configuration

The frontend selects its endpoint through Vite modes:

- `.env.development`: local API
- `.env.uat`: UAT placeholder endpoint; run `npm run dev:uat`
- `.env.production`: production placeholder endpoint; run `npm run dev:prod` or `npm run build`

Set `VITE_API_BASE_URL` to the deployed API URL before deploying UAT or production. No component hardcodes an API URL.

## API

| Method | Endpoint | Purpose |
| --- | --- | --- |
| POST | `/api/auth/login` | Sign in and receive a JWT |
| GET | `/api/pets` | List pets |
| GET | `/api/pets/{id}` | Get one pet |
| POST | `/api/pets` | Create a pet |
| PUT | `/api/pets/{id}` | Update a pet |
| DELETE | `/api/pets/{id}` | Delete a pet |
| GET | `/health` | Health check |

All Pet endpoints require `Authorization: Bearer <token>`. Invalid input returns 400, absent data returns 404, and unauthenticated requests return 401.

## Demo account

Username: `admin`  
Password: `PetShop123!`

The seeded password is PBKDF2-SHA256 hashed with a unique salt; the configured JWT key is deliberately an assessment-only value and must be replaced in a real deployment.

## Limitations

JSON files are appropriate only for this assessment’s mock persistence and are not a multi-instance production datastore. CORS is permissive to simplify local integration; deployment should restrict allowed origins. JWT configuration should be supplied securely outside source control in a production environment.
