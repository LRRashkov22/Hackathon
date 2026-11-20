# PASSALONG

Community platform for giving away and swapping useful physical items — without prices or payments.

## What is implemented

PASSALONG currently has a usable MVP backed by Neon PostgreSQL.

- ASP.NET Core API with EF Core migrations, PostgreSQL, ASP.NET Identity and JWT authentication.
- React + TypeScript frontend with standalone Login, Register, Dashboard, Create, My Posts, Wanted Items and Community pages.
- Public listing discovery with search, categories, Give Away/Swap types and authenticated exclusion of the current user's listings.
- Create listings and wanted items; Swap listings require a description of what the owner wants in return.
- Personal My Posts view and basic matching for wanted items.
- Exchange lifecycle: create request, accept, reject/cancel, two-party handover confirmation and atomic transfer ownership.
- Swap request dialog lets the requester choose an available listing to offer in return.
- Community impact summary and reputation leaderboard.
- Transfer-history endpoint and API support for reviews/reputation.
- Seeded demo users, categories, listings and wanted items.

### Main API routes

- `POST /api/auth/register`, `POST /api/auth/login`
- `GET, POST /api/listings`, `GET /api/listings/mine`
- `GET /api/categories`
- `POST /api/wanted-items`, `GET /api/wanted-items/mine`, `GET /api/wanted-items/{id}/matches`
- `POST /api/exchange-requests`, `GET /api/exchange-requests/incoming`, `GET /api/exchange-requests/outgoing`
- `POST /api/exchange-requests/{id}/accept|reject|cancel|confirm-handover`
- `GET /api/impact/summary`, `GET /api/leaderboard`, `GET /api/items/{id}/history`

## Configuration

Set these values as environment variables before starting the API. Do not commit them.

```text
ConnectionStrings__DefaultConnection=Host=...;Database=neondb;Username=...;Password=...;SSL Mode=Require
Jwt__Key=<a-long-random-secret-at-least-32-characters>
Jwt__Issuer=passalong
Jwt__Audience=passalong
```

Start the API locally with `dotnet run --project src/Passalong.Api`.

Start the frontend with:

```text
cd frontend/passalong-client
npm install
npm run dev
```

Swagger is exposed by the API for endpoint exploration. The frontend defaults to `http://localhost:5173` and the API to `http://localhost:5082`.

### Demo flow

1. Register two accounts (or use `maria@passalong.local` / `Passalong123!`).
2. Account A publishes a Give Away listing or a Swap listing with the desired item described.
3. Account B requests it; for a Swap, B selects one of their own available listings as the offer.
4. Account A accepts in Dashboard.
5. Both accounts confirm the handover in Dashboard to complete the transfer.

## Remaining work

The following features are planned but not yet complete:

1. Listing details page, item passport timeline UI and relisting of transferred items.
2. Edit, archive and delete listing interfaces; advanced filters, sorting and pagination UI.
3. Wanted-item edit/delete UI and a fuller matching explanation.
4. Profile settings, public profiles and review/reputation UI.
5. Notification center, saved listings, badges and recently completed exchanges.
6. Admin dashboard and moderation tools.
7. Automated tests for exchange authorization, transfer concurrency, matching and reviews.
8. Final responsive QA, polished empty/loading states and deployment instructions.
