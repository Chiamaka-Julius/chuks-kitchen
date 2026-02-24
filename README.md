# Chuks Kitchen Backend Deliverable

## System Overview
This project implements a backend prototype for Chuks Kitchen food ordering platform using **EF Core + CQRS** with **Controller-based APIs**.

Core flow:
1. User signs up with email or phone.
2. System generates OTP for verification.
3. Verified user browses foods and adds items to cart.
4. User creates order from cart.
5. Order moves through status lifecycle:
   - Pending -> Confirmed -> Preparing -> OutForDelivery -> Completed
   - Cancelled is supported from active states.

## Architecture
- `Chuks-Kitchen.API`: Controllers and HTTP layer.
- `Chuks-Kitchen.Services`: Generic CQRS dispatcher + BL layer (`BL/Interfaces`, `BL/Implementations`).
- `Chuks-Kitchen.Models`: Entities and enums.
- `Chuks-Kitchen.Migrations  `: EF Core `AppDbContext` and DB initialization.

## Tech Stack
- .NET 8 Web API
- EF Core + SQLite
- CQRS pattern
- Swagger UI

## Run
```bash
dotnet run --project src/Chuks-Kitchen.API/Chuks-Kitchen.API.csproj
```

Swagger UI (Development):
- `/swagger`

## API Endpoints

### User APIs
- `POST /api/users/signup`
- `POST /api/users/verify`

### Food APIs
- `GET /api/foods?includeUnavailable=false`
- `POST /api/foods`
- `PUT /api/foods/{foodId}/availability`

### Cart APIs
- `POST /api/cart/items`
- `GET /api/cart/{userId}`
- `DELETE /api/cart/{userId}`

### Order APIs
- `POST /api/orders`
- `GET /api/orders/{orderId}`
- `PATCH /api/orders/{orderId}/status`
- `POST /api/orders/{orderId}/cancel`

## Migrations (Recommended for EF Core)
From repository root:

```bash
dotnet ef migrations add InitialCreate \
  --project "src/Chuks-Kitchen.Migrations  /Chuks-Kitchen.Migrations.csproj" \
  --startup-project src/Chuks-Kitchen.API/Chuks-Kitchen.API.csproj \
  --context Chuks_Kitchen.Migrations.Data.AppDbContext


dotnet ef database update \
  --project "src/Chuks-Kitchen.Migrations  /Chuks-Kitchen.Migrations.csproj" \
  --startup-project src/Chuks-Kitchen.API/Chuks-Kitchen.API.csproj \
  --context Chuks_Kitchen.Migrations.Data.AppDbContext
```

## Edge Cases Handled
- Abandoned signup (unverified user remains until completed).
- Invalid/expired OTP.
- Duplicate email/phone signup.
- Invalid referral code.
- Food unavailable when adding to cart.
- Food becomes unavailable before order placement.
- Empty cart order attempt.
- Invalid order status transitions.
- Order cancellation restrictions on terminal states.

## Assumptions
- OTP is simulated and returned in signup response for test/demo.
- Authentication/authorization is intentionally omitted by requirement scope.
- Payment integration is not implemented (logic-only requirement).
- Admin actions are simulated through API endpoints.
