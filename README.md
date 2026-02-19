# Pets Hotel (Windows + SQLite)

This repository contains a C# Windows Forms app to manage dog stays in a pets hotel.

## Main functionality

- **Check in** a dog with:
  - Dog name
  - Owner name
  - Owner mobile number
  - Arrival date
  - Departure date
  - Food plan (times/day + portion size in grams)
- **Check out** a dog:
  - Select active stay
  - Enter daily tariff
  - Close account based on tariff × number of stay days

All data is stored locally in **SQLite** (`petshotel.db`) next to the executable.

## Tech stack

- .NET 8 Windows Forms
- `Microsoft.Data.Sqlite`

## Build and publish a single EXE

On a machine with .NET 8 SDK installed:

```bash
dotnet restore
dotnet build
```

Publish as a self-contained **single .exe** for Windows x64:

```bash
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true
```

The generated executable will be under:

- `bin/Release/net8.0-windows/win-x64/publish/`

## Notes

- The app creates the SQLite table (`DogStay`) automatically on first launch.
- A minimum of 1 day is charged even when arrival and departure dates are the same.
