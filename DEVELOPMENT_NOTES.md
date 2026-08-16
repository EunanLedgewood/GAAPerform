# GAA Perform — Development Notes

## Project Overview
A .NET MAUI Android app for GAA player training and fitness tracking. Built for club players aged 16-30, with coach/player role separation.

**Tech stack:** C# / .NET MAUI, SQLite (local), Firebase Auth, Firebase Realtime Database  
**Target platform:** Android (iOS can be added later)  
**Package name:** com.companyname.gaaperform

---

## Architecture

```
Views (XAML pages)
  └── bind to ViewModels (ObservableObject - CommunityToolkit.Mvvm)
        └── call Services
              ├── DatabaseService     → SQLite local storage
              ├── TrainingPlanService → week plan logic (no DB)
              ├── SessionLibraryService → exercise content (no DB)
              ├── FirebaseAuthService → Firebase email/password auth
              └── FirestoreService   → Firebase Realtime Database
```

**Pattern:** MVVM with dependency injection (registered in MauiProgram.cs)  
**Converters:** All shared converters live in `Views/Converters.cs`

---

## Tab Structure (5 tabs)

| Tab | Page | Notes |
|-----|------|-------|
| Training | WeekPage | Week plan driven by calendar events |
| History | HistoryPage | Completed sessions with exercises |
| Calendar | CalendarPage | Match and session scheduling |
| Report | ReadinessPage | Readiness score and weekly stats |
| Me | MePage | Stats + settings cog → SettingsPage |
| Squad | CoachSquadPage | Coach only — injected dynamically in AppShell.xaml.cs |

---

## Key Features

### Session Tracking
- Player taps a day → SessionDetailPage → Start Session → ActiveSessionPage
- Timer runs top right counting up
- Each exercise has sets with configurable field types
- Finish Session → comment for coach → saves to SQLite + marks day complete

### Exercise Field Types
Stored as integer in SQLite via `ExerciseFieldType` enum. **Never reorder enum values** — doing so will corrupt existing saved session data. Always add new types at the end.

| Index | Enum Value | Display | Fields |
|-------|-----------|---------|--------|
| 0 | WeightsAndReps | ⚖️ Weights + Reps | Weight (kg), Reps |
| 1 | TimeAndDifficulty | ⏱️ Time + Difficulty | Time, Difficulty (1-10) |
| 2 | RepsOnly | 🔄 Reps Only | Reps |
| 3 | Custom | ✏️ Custom | User-defined label 1, label 2 |

**Adding a new field type — checklist:**
- [ ] Add enum value to `ExerciseFieldType` in `CompletedSession.cs` (at the end)
- [ ] Add `Is{TypeName}` property to `CompletedExercise`
- [ ] Add string to Picker items in `ActiveSessionPage.xaml`
- [ ] Add column headers XAML block with `IsVisible="{Binding Is{TypeName}}"`
- [ ] Add entry fields XAML block for the new type
- [ ] Add case to `OnFieldTypeSelected` switch in `ActiveSessionPage.xaml.cs`
- [ ] Add case to `OnPickerLoaded` switch in `ActiveSessionPage.xaml.cs`
- [ ] Add case to `ExerciseFieldTypeToLabelConverter` in `Converters.cs`
- [ ] Add fields to `ExerciseSet` model if needed

### Session Completion Tracking
Day completion is stored in `Preferences` using the key `completed_{yyyy-MM-dd}`.  
**Do not use the database for this** — SQLite schema changes caused issues. Preferences persists across app restarts.

### Coach → Player Session Assignment
1. Coach adds player by email in Squad tab
2. Coach assigns session with title, date, type, notes
3. Saved to Firebase Realtime Database under `assignedSessions/{safeEmail}/`
4. Player's CalendarViewModel calls `SyncCoachAssignedSessionsAsync` on load
5. Synced sessions appear in player's local SQLite calendar

### Firebase Database Structure
```
users/
  {userId}/
    email: string
    role: "Player" | "Coach"
    createdAt: ISO string

assignedSessions/
  {playerEmail_safe}/     (@ replaced with _at_, . replaced with _)
    {pushId}/
      coachId, playerEmail, title, date, eventType, notes, isCoachAssigned

coachPlayers/
  {coachId_safe}/
    {pushId}/
      playerEmail, addedAt
```

---

## Database Schema (SQLite)

### Tables
- `SessionLog` — feeling/soreness scores per session
- `UserProfile` — local position, season mode, next match date
- `CalendarEvent` — local events (matches, training, coach-assigned)
- `CompletedSession` — finished sessions with SetsJson (serialised JSON)
- `CompletedDays` — raw SQL table, tracks completed day strings

### Important: CalendarEvent EventType
`EventType` is stored as `EventTypeInt` (integer column) with `[SQLite.Ignore]` on the enum property. This was added to fix a cast exception. Do not change this pattern.

---

## Preferences Keys

| Key | Type | Purpose |
|-----|------|---------|
| `has_onboarded` | bool | Skip onboarding on subsequent launches |
| `is_logged_in` | bool | Skip login screen if already authenticated |
| `user_role` | string | "Player" or "Coach" — controls Squad tab visibility |
| `user_email` | string | Displayed in Me tab and Settings |
| `dark_mode` | bool | App theme preference |
| `completed_{yyyy-MM-dd}` | bool | Day completion tracking |

---

## Firebase Configuration
Firebase config is stored in `appsettings.json` which is **gitignored**.  
Never commit API keys. If a key is accidentally committed, rotate it immediately in Google Cloud Console → APIs & Services → Credentials.

The file lives at project root and is included as a MAUI raw asset:
```json
{
  "Firebase": {
    "ApiKey": "YOUR_KEY_HERE",
    "AuthDomain": "gaaperform.firebaseapp.com",
    "ProjectId": "gaaperform",
    "DatabaseUrl": "https://gaaperform-default-rtdb.europe-west1.firebasedatabase.app/"
  }
}
```

---

## Known Issues / Tech Debt

- **Password reset emails go to spam** — Firebase free tier uses `noreply@firebase.google.com`. Fix: set up custom SMTP domain in Firebase Auth templates when a domain is purchased.
- **Dark mode partial** — toggle switches theme but custom hex colours in XAML don't respond. Fix: migrate `Colors.xaml` to use `AppThemeBinding` for all colours.
- **Session completion not persisting to cloud** — day completion uses local Preferences only. If user reinstalls, completion history is lost. Fix: sync to Firebase.
- **Profile tab removed** — position and season mode now only editable via onboarding. Fix: add back as section in SettingsPage.
- **iOS not configured** — csproj targets Android only. iOS requires Mac pairing and Apple Developer account.

---

## To Do / Planned Features

### Near term
- [ ] Exercise video links (YouTube) per exercise in SessionLibraryService
- [ ] Push notifications — remind player to log after a session
- [ ] App icon and splash screen (currently default MAUI purple)
- [ ] Real device testing (currently emulator only)

### Medium term
- [ ] Team chat (Firebase Realtime Database)
- [ ] Coach can see player session completion and comments
- [ ] Weekly report emailed to player (Firebase Cloud Functions)
- [ ] GAAPerform Pro subscription (RevenueCat)

### Later
- [ ] iOS support
- [ ] Web dashboard for coaches
- [ ] Custom exercise video upload by coach

---

## Git Branch Convention
```
feature/{name}   — new features
fix/{name}       — bug fixes
chore/{name}     — maintenance, dependency updates
```

Always branch from `main`. Merge via PR (or direct merge for solo work). Never commit directly to `main`.

---

## Running the App

**Requirements:**
- Visual Studio 2022 with .NET MAUI workload
- Android emulator (Pixel 7, API 36) or physical Android device
- `appsettings.json` in project root (not in repo — get from project owner)

**First run:**
1. `dotnet restore`
2. Select Android emulator in toolbar
3. F5 to build and deploy

**If build fails:**
- Delete `bin/` and `obj/` folders
- Run `dotnet restore` in Developer PowerShell
- Rebuild

---

*Last updated: August 2026*
