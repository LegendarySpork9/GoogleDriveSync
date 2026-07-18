# Google Drive Sync - Infrastructure Document

## Overview

Google Drive Sync is a Windows Forms desktop application for bidirectional file synchronisation between a local directory and a Google Drive folder. It compares files across both locations, detects changes in content, path, and metadata, and allows the user to selectively sync files upstream (to Google Drive) or downstream (to local). It also supports automatic synchronisation on a timer.

- **Author:** Hunter Industries / Toby Hunter
- **Version:** N/A (legacy .csproj format)
- **Repository:** https://github.com/LegendarySpork9/GoogleDriveSync

## Technology Stack

| Component | Technology | Version |
|---|---|---|
| Framework | .NET Framework | 4.7.2 |
| Language | C# | - |
| Application Type | Windows Forms (WinExe) | - |
| Google Drive API | Google.Apis.Drive.v3 | 1.69.0.3740 |
| Google Auth | Google.Apis.Auth | 1.69.0 |
| Logging | log4net | 3.0.4 |
| JSON Serialisation | Newtonsoft.Json | 13.0.3 |
| Configuration | System.Configuration.ConfigurationManager | 9.0.4 |
| Build System | MSBuild + NuGet | - |
| Testing | MSTest | 3.6.4 |
| Test SDK | Microsoft.NET.Test.Sdk | 17.12.0 |
| Mocking | Moq | 4.20.72 |
| Code Coverage | Microsoft.Testing.Extensions.CodeCoverage | 17.12.6 |
| Test Reporting | Microsoft.Testing.Extensions.TrxReport | 1.4.3 |

## Solution Structure

```
GoogleDriveSync/
+-- Google Drive Sync/                  # Main Windows Forms application
|   +-- Abstractions/                   # Interface definitions
|   +-- Content/                        # Static assets (icons, images)
|   +-- Converters/                     # MIME type, path, and value converters
|   +-- Forms/                          # Windows Forms UI
|   +-- Functions/                      # File comparison, folder, and utility functions
|   +-- Implementations/               # Interface implementations (wrappers)
|   +-- Models/                         # Data models
|   +-- Properties/                     # Assembly info, resources, settings
|   +-- Services/                       # Business logic services
+-- Google Drive Sync.Tests/            # Unit test project (.NET 4.7.2)
|   +-- Converters/                     # Converter tests
|   +-- Functions/                      # Function tests
|   +-- Services/                       # Service tests
+-- .github/workflows/                  # CI/CD pipeline definitions
```

## Application Architecture

### Application Type

The application is a **.NET Framework 4.7.2 Windows Forms** desktop application. It presents a single-form UI where the user can compare files between Google Drive and a local directory, review detected changes, and selectively synchronise files in either direction.

### Dependency Injection

External dependencies are wrapped behind interfaces to support testability. Services are instantiated manually.

| Abstraction | Implementation | Purpose |
|---|---|---|
| `ILoggerService` | `LoggerServiceWrapper` | Application logging via log4net |
| `IFileSystem` | `FileSystemWrapper` | File and directory operations, ZIP streams |
| `IFileMetadata` | `FileMetadataProvider` | File creation time, modification time, and hidden attribute |
| `IClock` | `SystemClockProvider` | UTC time and default date operations |
| `ICredentialProvider` | `GoogleCredentialProvider` | Google OAuth 2.0 credential acquisition |
| `IGoogleDriveClient` | `GoogleDriveClientWrapper` | Google Drive API file and folder operations |
| `IUserNotifier` | `MessageBoxWrapper` | Warning message display to the user |

### Services

| Service | Responsibility |
|---|---|
| `ApplicationService` | Top-level sync orchestrator: check updates, sync changes, progress reporting |
| `GoogleAPIService` | Google Drive folder traversal, file CRUD, OAuth initialisation, folder store caching |
| `DocumentService` | Local file system traversal, file metadata extraction, file attribute management |
| `LoggerService` | Internal log4net adapter |

### Converters

| Converter | Responsibility |
|---|---|
| `GoogleDriveConverter` | Maps file extensions to MIME types and constructs file paths |
| `LocalDriveConverter` | Extracts file names from full paths |
| `ProgressBarValueConverter` | Calculates progress bar increment from task count |
| `StandardValues` | Log level constants (Debug, Error, Info, Warn) |

### Functions

| Function | Responsibility |
|---|---|
| `FileFunction` | Compares Google Drive and local file lists to detect changes; checks file lock status |
| `FolderFunction` | Ensures local directories exist before download |
| `GoogleDriveFunction` | String parsing for comma-separated ID/path pairs and folder hierarchies |
| `LoggerFunction` | Formats Google Drive file metadata for log output |

## User Interface

### MainSyncPage Form

The application has a single Windows Form (`MainSyncPage`) with a fixed size of 1265x535 pixels.

#### Controls

| Control | Type | Purpose |
|---|---|---|
| DGVFileInformation | DataGridView | Main file list with columns: File Name, File Type, File Path, Date Created, Last Modified, Has Changes, Merge Type (dropdown), Update (checkbox) |
| TBCDocumentChanges | TabControl | Detail panel showing selected file properties and change details |
| DGVChanges | DataGridView | Change detail grid with columns: Field, Old Value, New Value, Stream |
| BTNCompare | Button | Triggers file comparison between Google Drive and local |
| BTNSync | Button | Syncs selected files (shows count: "Sync N File(s)") |
| PBUp | PictureBox | Sets all changed files to "Up Stream" sync direction |
| PBDown | PictureBox | Sets all changed files to "Down Stream" sync direction |
| PRBLoading | ProgressBar | Shows sync/compare progress (0-100) |
| PBLoading | PictureBox | Status indicator (loading spinner, tick, or cross) |
| CBAutoSync | CheckBox | Enables automatic synchronisation on a 15-second timer |
| TMAutoSync | Timer | Fires every 15 seconds when auto-sync is enabled |

#### File Detail Panel

When a file row is selected, the detail panel displays:

| Field | Description |
|---|---|
| Id | Google Drive file ID |
| Name | File name (without extension) |
| Type | File extension |
| Path Ids | Google Drive folder ID hierarchy |
| GD Path | Google Drive folder path |
| L Path | Local file path |
| Hidden | Whether the file is hidden |
| Created | Creation timestamp |
| Modified | Last modification timestamp |

### Sync Workflow

#### Manual Sync

1. User clicks **Compare** to scan both Google Drive and local directories
2. File list populates with detected changes flagged
3. For each changed file, user selects **Up Stream** or **Down Stream** direction and ticks **Update**
4. Bulk direction buttons (up/down arrows) set all changed files at once
5. User clicks **Sync N File(s)** to execute the synchronisation

#### Auto Sync

1. User ticks **Auto Sync** checkbox
2. Every 15 seconds, the application automatically:
   - Compares files between Google Drive and local
   - Skips locked files (in use by another process)
   - Skips files with only path changes (syncs content changes only)
   - Uploads locally-modified files and downloads Google-modified files
3. User unticks the checkbox to stop

## Synchronisation Logic

### Change Detection

The `FileFunction.CompareForChanges` method matches files by name and type across both sources:

| Change Type | Detection | Stream Direction |
|---|---|---|
| Not Uploaded | File exists locally but not on Google Drive | Down (upload candidate) |
| Not Downloaded | File exists on Google Drive but not locally | Up (download candidate) |
| Path Changed | File exists in both but folder paths differ | Based on modification time |
| Content Changed | File exists in both but modification times differ | Up if Google is newer, Down if local is newer |

### Sync Operations

| Operation | Trigger | Action |
|---|---|---|
| Create File | File not on Google Drive | Upload via Google Drive API |
| Update File | Content changed | Re-upload file content and update metadata |
| Move File | Path changed | Move file to correct folder on Google Drive |
| Download File | File not local or Google is newer | Download from Google Drive, set timestamps, unblock, set attributes |
| Delete File | File moved locally | Delete old copy from Google Drive |

### Post-Download Processing

After downloading a file from Google Drive:

1. **Unblock** — Removes the `Zone.Identifier` alternate data stream (ADS) to prevent Windows security warnings
2. **Set Hidden** — Applies the Hidden file attribute if the file was marked as hidden
3. **Set Timestamps** — Sets the local file's creation and modification times to match Google Drive metadata

## Supported MIME Types

| Extension | MIME Type |
|---|---|
| `.txt` | `text/plain` |
| `.pdf` | `application/pdf` |
| `.doc` | `application/msword` |
| `.docx` | `application/vnd.openxmlformats-officedocument.wordprocessingml.document` |
| `.xls` | `application/vnd.ms-excel` |
| `.xlsx` | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| `.png` | `image/png` |
| `.jpeg` | `image/jpeg` |
| Default | `application/octet-stream` |

## Authentication

### Google OAuth 2.0

- **Scope:** `DriveService.Scope.Drive` (full Google Drive access)
- **Credential Source:** OAuth client credentials JSON file (path configured via `CredentialsLocation`)
- **Flow:** `GoogleWebAuthorizationBroker.AuthorizeAsync` launches a browser-based consent flow
- **Token Storage:** Handled by the Google API client library

## Configuration

### App.config Structure

```xml
<configuration>
  <appSettings>
    <add key="CredentialsLocation" value="<path to Google OAuth credentials JSON>" />
    <add key="GoogleDriveFolder" value="<target Google Drive folder ID>" />
    <add key="LocalDirectory" value="<local sync directory path>" />
    <add key="IgnoreFolders" value="<comma-separated folder names to exclude>" />
    <add key="IgnoreFiles" value="<comma-separated file names to exclude>" />
  </appSettings>
</configuration>
```

| Setting | Required | Purpose |
|---|---|---|
| `CredentialsLocation` | Yes | Path to the Google OAuth 2.0 client credentials JSON file |
| `GoogleDriveFolder` | Yes | Google Drive folder ID to synchronise with |
| `LocalDirectory` | Yes | Local directory path to synchronise with |
| `IgnoreFolders` | No | Comma-separated folder names to exclude from sync |
| `IgnoreFiles` | No | Comma-separated file names to exclude from sync |

### Built-in Exclusions

Office temporary files matching the pattern `~$*` are automatically excluded from synchronisation regardless of the `IgnoreFiles` setting.

## File Identity Tracking

Files are tracked using a composite identity scheme:

| Source | Id Format | Path Format |
|---|---|---|
| Google Drive only | Google file ID | Google Drive folder path |
| Local only | Local file path | Local folder path |
| Both sources | `{GoogleId},{LocalPath}` | `{GooglePath},{LocalPath}` |

The comma-separated format is parsed at sync time to extract the relevant identifiers for each operation direction.

## Logging

- **Framework:** log4net 3.0.4
- **Configuration:** Embedded in App.config

### Appenders

| Appender | Type | File | Purpose |
|---|---|---|---|
| LogAppender | RollingFile | `Logs\GD Sync.log` | Application operation logs (INFO+) |

### Log File Settings

- **Max File Size:** 10 MB
- **Backup Count:** 10 rolling files
- **Format:** `{ISO8601 Timestamp} {LEVEL} - {Message}`
- **Lock Model:** MinimalLock (concurrent access safe)
- **Minimum Level:** INFO

## CI/CD

### GitHub Actions Workflows

All workflows run on `windows-latest`.

| Workflow | Trigger | Steps |
|---|---|---|
| **CI on Commit** (`Commit.yml`) | Push to any branch | Checkout, Setup MSBuild, Setup NuGet, Restore (`nuget restore`), Build (`msbuild /p:Configuration=Release`) |
| **CI on Pull Request** (`Pull Request.yml`) | PR to any branch | Checkout, Setup MSBuild, Setup NuGet, Restore, Build, Run Tests (`dotnet test`) |
| **Check for Linked Issue** (`PR Linked Issue.yml`) | PR opened/edited/reopened/synchronised | Verifies PR has linked GitHub issues via description, comments, or Development section |

### Build Configuration

- **Build Tool:** MSBuild (via `microsoft/setup-msbuild@v2`)
- **Package Manager:** NuGet (via `NuGet/setup-nuget@v2`)
- **Configuration:** Release
- **Test Runner:** `dotnet test` (MSTest with method-level parallelisation)

## Hosting Requirements

### Runtime Prerequisites

- .NET Framework 4.7.2 Runtime
- Windows (required for Windows Forms UI and file attribute management)

### Network Requirements

- Outbound HTTPS to Google APIs (`accounts.google.com`, `www.googleapis.com`) for OAuth and Drive API

### File System Requirements

- Read/write access to the configured local sync directory
- Read/write access to the `Logs/` directory
- Read access to the Google OAuth credentials JSON file

### Static Assets

| File | Purpose |
|---|---|
| `favicon.ico` | Application icon |
| `LoadingSpinner.gif` | Loading indicator animation |
| `Tick.png` | Success status indicator |
| `Cross.png` | Error status indicator |
| `Up Arrow.png` | Upload direction button |
| `Down Arrow.png` | Download direction button |
