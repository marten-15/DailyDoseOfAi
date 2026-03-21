# DailyDoseOfAi

DailyDoseOfAi is a .NET 10 solution for managing and viewing bookmarked AI resources.

## Solution Structure

- `AzenetOne.Bookmarks`: Console app entry point.
- `AzenetOne.Bookmarks.Core`: Core domain, EF Core data layer, and bookmark services.
- `AzenetOne.Bookmarks.Web`: Blazor web UI for browsing and managing bookmarks.
- `AzenetOne.Bookmarks.Tests`: Unit test project.

## Tech Stack

- .NET 10
- C#
- Entity Framework Core
- Blazor
- xUnit

## Getting Started

### Prerequisites

- .NET 10 SDK

### Restore

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run Web App

```bash
dotnet run --project AzenetOne.Bookmarks.Web
```

### Run Tests

```bash
dotnet test
```
