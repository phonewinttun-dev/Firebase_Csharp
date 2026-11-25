# Firebase_Csharp

A .NET 10 sample application demonstrating Firebase Cloud Messaging (FCM) push notifications using an ASP.NET Core Web API backend and a Blazor WebAssembly frontend styled with Tailwind CSS.

## Project Structure

- **`Firebase_Csharp.Api`** — ASP.NET Core Web API (`net10.0`) using `FirebaseAdmin` to send FCM notifications, with OpenAPI, Swagger UI, and Scalar API reference.
- **`Firebase_Csharp.Client`** — Blazor WebAssembly (`net10.0`) frontend with Tailwind CSS v4 and JS interop for Firebase Cloud Messaging.
- **`Firebase_Csharp.Domain`** — Domain services (`INotificationService`, `NotificationService`) and request/response DTOs.
- **`Firebase_Csharp.Shared`** — Shared primitives (`Result<T>`).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js & npm](https://nodejs.org/) (used by `Firebase_Csharp.Client` to build Tailwind CSS)
- A Firebase project with Cloud Messaging enabled and a service account key JSON file

## Setup

1. **Firebase Service Account Key**
   Place your Firebase service account key file at:
   ```
   Firebase_Csharp.Api/serviceAccountKey.json
   ```

2. **Client Dependencies (Tailwind CSS)**
   ```bash
   cd Firebase_Csharp.Client
   npm install
   ```

## Running the Application

Run the API and Client in separate terminals:

**API** (`https://localhost:7137` / `http://localhost:5206`):
```bash
dotnet run --project Firebase_Csharp.Api --launch-profile https
```

**Client** (`https://localhost:7213` / `http://localhost:5158`):
```bash
dotnet run --project Firebase_Csharp.Client --launch-profile https
```

## API Endpoints

- `POST /api/Notification/send-to-device` — Send a push notification to a specific device registration token.
- `POST /api/Notification/send-to-topic` — Send a push notification to a subscribed topic.

In `Development` mode, interactive API documentation is available at:
- Scalar: `https://localhost:7137/scalar`
- Swagger UI: `https://localhost:7137/swagger`
