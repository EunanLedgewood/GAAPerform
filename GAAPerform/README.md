# GAA Perform — Setup Guide

## Prerequisites

1. Visual Studio 2022 (Community is free — https://visualstudio.microsoft.com)
2. In the VS Installer, check: **.NET Multi-platform App UI development**
3. .NET 8 SDK (installed automatically with the workload)

---

## Project Setup Steps

### 1. Create the project in Visual Studio

- File → New → Project
- Search: **MAUI**
- Choose: **.NET MAUI App**
- Name: `GAAPerform`
- Framework: **.NET 8**
- Click Create

This gives you a boilerplate shell. You will replace/add the files below.

---

### 2. File structure — create these exactly

```
GAAPerform/
├── Models/
│   ├── TrainingDay.cs
│   ├── SessionLog.cs
│   ├── UserProfile.cs
│   └── PositionPlan.cs
├── ViewModels/
│   ├── WeekViewModel.cs
│   ├── ReadinessViewModel.cs
│   ├── LogViewModel.cs
│   └── ProfileViewModel.cs
├── Views/
│   ├── WeekPage.xaml + WeekPage.xaml.cs
│   ├── ReadinessPage.xaml + ReadinessPage.xaml.cs
│   ├── LogPage.xaml + LogPage.xaml.cs
│   └── ProfilePage.xaml + ProfilePage.xaml.cs
├── Services/
│   ├── DatabaseService.cs
│   └── TrainingPlanService.cs
├── Resources/
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
├── App.xaml + App.xaml.cs
├── AppShell.xaml + AppShell.xaml.cs
├── MauiProgram.cs
└── GAAPerform.csproj
```

---

### 3. Install NuGet packages

Right-click the project → **Manage NuGet Packages** → Browse and install:

| Package | Version |
|---|---|
| `CommunityToolkit.Mvvm` | 8.2.2 |
| `sqlite-net-pcl` | 1.9.172 |
| `SQLitePCLRaw.bundle_green` | 2.1.8 |

Or use the Package Manager Console:
```
Install-Package CommunityToolkit.Mvvm -Version 8.2.2
Install-Package sqlite-net-pcl -Version 1.9.172
Install-Package SQLitePCLRaw.bundle_green -Version 2.1.8
```

---

### 4. Add the XAML pages

When you add a new page:
- Right-click `Views/` folder → Add → New Item
- Choose **.NET MAUI ContentPage (XAML)**
- Name it e.g. `WeekPage`
- This creates `WeekPage.xaml` and `WeekPage.xaml.cs` together

Replace the contents of each with the code provided.

---

### 5. Add the AppShell route

The `AppShell.xaml` wires up the tab bar. The four tabs map to:
- `WeekPage` — This Week
- `ReadinessPage` — Readiness
- `LogPage` — Log
- `ProfilePage` — Profile

---

### 6. Run on Android Emulator

1. **Tools → Android → Android Device Manager**
2. Click **New** → choose Pixel 6 → API 33 → Create
3. Click **Start** on the device
4. In the toolbar dropdown, select your Android emulator
5. Press **F5** to build and deploy

First build takes ~5 minutes (downloading Android SDK pieces). Subsequent builds are fast.

---

### 7. iOS (needs a Mac)

If you have a Mac on the same WiFi:
1. On the Mac: install **Xcode** from the App Store
2. In VS on Windows: **Tools → iOS → Pair to Mac**
3. Follow the pairing wizard
4. Once paired, select an iOS Simulator in the toolbar
5. Press F5

If no Mac available: test entirely on Android for now. The codebase is 100% shared — iOS just needs the Mac for compilation.

---

### 8. XAML Hot Reload

While the emulator is running, edit any `.xaml` file and save. The UI updates live without a full rebuild. This is your main feedback loop for UI tweaks.

---

## Architecture Overview

```
Views (XAML pages)
  └── bind to ViewModels (ObservableObject)
        └── call Services
              ├── DatabaseService  → SQLite (local storage)
              └── TrainingPlanService → pure logic, no DB
```

**Pattern:** MVVM (Model-View-ViewModel)
- Views only do UI — no business logic
- ViewModels hold state with `[ObservableProperty]` (auto-generates INotifyPropertyChanged)
- Commands use `[RelayCommand]` (auto-generates ICommand)
- Services injected via constructor (registered in `MauiProgram.cs`)

---

## Next steps to build on this

1. **Session detail page** — tap a day card → navigate to full session with exercise list
2. **Match date picker** — let user set their next match date in Profile
3. **Notifications** — remind player to log after a session (MAUI has local notifications)
4. **Charts** — add `Microcharts.Maui` NuGet for readiness trend graph
5. **Subscription gate** — wrap premium features behind a `IsPremium` flag on UserProfile

---

## Common errors and fixes

| Error | Fix |
|---|---|
| `XamlParseException` on launch | Check all `x:Class` names match the namespace in code-behind |
| `Could not resolve type` for a View | Make sure it's registered in `MauiProgram.cs` |
| Android emulator very slow | Enable Hardware Acceleration in AVD settings (HAXM) |
| SQLite `no such table` | DatabaseService `InitAsync()` creates tables on first run — check it's being awaited |
| Build error on iOS target on Windows | Expected — iOS requires Mac. Build Android-only for now by removing `net8.0-ios` from TargetFrameworks |
