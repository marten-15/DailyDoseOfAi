# Favorites (Bookmarks Manager) — Requirements Document

**Date:** 2026-03-22
**Version:** 1.0
**Target stack:** .NET 10, Blazor Web App, EF Core 10, PostgreSQL
**Hosting:** On-prem Windows Server 2019
**Metadata fetch:** Server-side

---

## Overview

The **Favorites** app is a personal bookmarks manager that replaces reliance on browser-native favorites menus. It provides a dedicated, searchable, and organized store for saving, tagging, and retrieving links. Users can add a URL and have the server automatically retrieve page metadata (title, description, favicon), organize bookmarks with flat folders and tags, and import or export their existing collections.

---

## 1. Scope and Phasing

### 1.1 Phase 1 — MVP

- User registration, login, and per-user data isolation (ASP.NET Core Identity)
- Favorites CRUD with server-side metadata fetch
- Flat folders (no nesting)
- Tags with many-to-many assignment
- Global search, filtering, and sorting
- Import from browser HTML export; export to HTML and JSON
- Responsive UI (mobile and desktop)
- Deployment on Windows Server 2019 with PostgreSQL

### 1.2 Phase 2 — Sharing (planned)

- Shared collections with public / unlisted / private-by-invite visibility modes
- Read-only shared views with revocable links
- Collaboration role extension (optional, post-Phase 2)

### 1.3 Deferred / Out of Scope

- **Folder nesting:** deferred; flat folders only in MVP. May be revisited after Phase 2.
- Full offline-first or native mobile apps
- AI-generated summaries
- Enterprise admin features beyond basic user management

---

## 2. Functional Requirements — Phase 1 MVP

### 2.1 Authentication and Authorization

| ID | Requirement |
|----|-------------|
| FR-001 | Users can register with email and password, log in, and log out. |
| FR-002 | Users can request a password reset via email. Email confirmation is configurable. |
| FR-003 | The system enforces per-user data isolation: every data query is scoped to the authenticated user. |
| FR-004 | An administrator role may manage user accounts (activate, deactivate, delete). Scope to be confirmed for on-prem deployment. |

### 2.2 Favorites CRUD

| ID | Requirement |
|----|-------------|
| FR-010 | A user can create a Favorite by providing a URL (required field). |
| FR-011 | A Favorite stores the following fields: URL (required), Title (optional), Notes (optional), FolderId (optional), Pinned (boolean, default false), Status (Normal / ReadLater / Archived), CreatedAt, UpdatedAt, DeletedAt (nullable, soft delete). |
| FR-012 | A user can edit any field of an existing Favorite. |
| FR-013 | A user can soft-delete a Favorite. Deleted items appear in a Trash view and can be restored or permanently deleted. |
| FR-014 | A user can open the bookmarked URL in a new tab directly from the list view. |
| FR-015 | When adding a Favorite, if the same URL already exists for that user, the system presents the following options: **Open existing**, **Update existing (merge tags)**, or **Create duplicate**. |

### 2.3 Server-side Metadata Fetch

| ID | Requirement |
|----|-------------|
| FR-020 | After a Favorite is created, the server fetches the following metadata for the URL: page title (HTML `<title>` or OpenGraph `og:title`), favicon URL (best-effort), and description (OpenGraph `og:description` or meta description). |
| FR-021 | Metadata fetch is performed asynchronously and must not block saving the Favorite. The Favorite is persisted immediately; metadata is updated when the fetch completes. |
| FR-022 | Metadata fetch must apply a configurable timeout (default: 5 seconds). If the fetch fails or times out, the Favorite is retained with only the URL and any user-supplied fields. |
| FR-023 | Users can manually override Title and Description regardless of what metadata fetch returned. |

### 2.4 Tags

| ID | Requirement |
|----|-------------|
| FR-030 | A user can create, rename, and delete tags. Deleting a tag removes it from all associated Favorites. |
| FR-031 | A user can assign or unassign one or more tags to any Favorite. |
| FR-032 | When editing tags on a Favorite, the UI provides suggestions from the user's existing tag list as the user types. |

### 2.5 Folders (Flat Only)

| ID | Requirement |
|----|-------------|
| FR-040 | A user can create, rename, and delete folders. Folders are flat; nesting is not supported in Phase 1. |
| FR-041 | A user can assign a Favorite to a folder or move it to a different folder, or leave it unfiled. |
| FR-042 | When a folder is deleted, all Favorites previously in that folder become unfiled (no cascading delete of Favorites). |

### 2.6 Search, Filter, and Sort

| ID | Requirement |
|----|-------------|
| FR-050 | Global search queries the title, URL, notes, and tag names of Favorites belonging to the authenticated user. |
| FR-051 | Favorites list can be filtered by: folder, tag(s), pinned status, Status field, and date range (CreatedAt). |
| FR-052 | Favorites list can be sorted by: newest (default), oldest, title A–Z, and pinned first. |
| FR-053 | List results are paginated or rendered with virtual scrolling to support large collections. |

### 2.7 Import and Export

| ID | Requirement |
|----|-------------|
| FR-060 | A user can import bookmarks from a standard browser HTML export file (Netscape Bookmark Format). |
| FR-061 | The import flow shows a preview with a summary of how many items will be added before the user confirms. |
| FR-062 | During import, the user selects a duplicate-handling strategy: skip duplicates, merge tags into existing entries, or allow duplicates. |
| FR-063 | A user can export their Favorites to: HTML bookmarks format (for re-import into browsers) and JSON (full application backup). |

### 2.8 Logging and Audit

| ID | Requirement |
|----|-------------|
| FR-070 | The system logs the following events using structured logging: authentication failures, import and export actions, metadata fetch failures, and unhandled application errors. Logs must not contain passwords or other sensitive personal data. |
| FR-071 | (Optional) The system records a "last opened" timestamp on a Favorite when the user navigates to its URL from the app (click tracking). |

---

## 3. Phase 2 Requirements — Sharing

| ID | Requirement |
|----|-------------|
| FR2-001 | A user can create a named **Shared Collection** containing selected Favorites. |
| FR2-002 | Shared Collections support three visibility modes: **Public** (accessible via link; may be indexed), **Unlisted** (accessible only via link; not indexed), and **Private** (accessible only to explicitly invited users). |
| FR2-003 | The shared view is read-only for all viewers; viewers cannot add, edit, or delete items. |
| FR2-004 | The owner can revoke a shared link at any time, instantly preventing further access. |
| FR2-005 | The owner can add or remove Favorites from a Shared Collection after it has been published. |

---

## 4. Non-Functional Requirements

### 4.1 Hosting and Operations

| ID | Requirement |
|----|-------------|
| NFR-101 | The application runs on Windows Server 2019 using ASP.NET Core hosting under IIS (with the ASP.NET Core Module) or as a Windows Service. |
| NFR-102 | The application connects to PostgreSQL. Connection strings are stored outside the compiled binary using environment variables or a secrets management approach; TLS on the database connection is required where the network path is not already secured. |
| NFR-103 | Deployment documentation must cover: database creation, EF Core migration execution, IIS/Windows Service configuration, HTTPS certificate setup, and backup and restore procedures for PostgreSQL. |
| NFR-104 | EF Core migrations are the authoritative mechanism for all schema changes. Migrations must be reviewed before being applied to a production database. |

### 4.2 Security

| ID | Requirement |
|----|-------------|
| NFR-201 | HTTPS is enforced for all requests; HTTP is redirected to HTTPS. |
| NFR-202 | CSRF protection is applied to all state-changing operations (Blazor antiforgery defaults plus explicit validation where required). |
| NFR-203 | Every database query that accesses user data filters by the authenticated user's ID. Returning data belonging to another user is a critical defect. |
| NFR-204 | Authentication endpoints apply rate limiting and account lockout after a configurable number of failed attempts. |
| NFR-205 | User-supplied content (notes, titles) is sanitized or encoded before being rendered to prevent XSS. |

### 4.3 Performance

| ID | Requirement |
|----|-------------|
| NFR-301 | The Favorites list page loads and renders within approximately 1 second for a collection of up to 2,000 Favorites on the target hardware. |
| NFR-302 | Search results are debounced (300 ms default) and return within approximately 300 ms for typical collection sizes (up to 10,000 Favorites). |
| NFR-303 | Server-side metadata fetch is non-blocking and does not degrade list-view or search response times. |
| NFR-304 | Paginated or virtualized lists are required when a user's collection exceeds a configurable threshold (default: 100 items per page). |

### 4.4 Reliability and Maintainability

| ID | Requirement |
|----|-------------|
| NFR-401 | Centralized structured logging (e.g., using Serilog or Microsoft.Extensions.Logging with a file or database sink) is configured from startup. |
| NFR-402 | Unit tests cover domain and service logic. Integration tests cover key data access and API flows. |
| NFR-403 | The solution structure separates concerns: Web (Blazor UI), Application (use cases / services), Domain (entities and rules), Infrastructure (EF Core, Identity, external integrations). |

---

## 5. Data Model (Conceptual)

### 5.1 Phase 1 Entities

| Entity | Key Fields |
|--------|-----------|
| **ApplicationUser** | Id, Email, PasswordHash, … (ASP.NET Core Identity) |
| **Favorite** | Id, UserId, Url, Title, Notes, FolderId (nullable), Pinned, Status, CreatedAt, UpdatedAt, DeletedAt (nullable) |
| **Tag** | Id, UserId, Name |
| **FavoriteTag** | FavoriteId, TagId (join table) |
| **Folder** | Id, UserId, Name, CreatedAt |

### 5.2 Phase 2 Additions

| Entity | Key Fields |
|--------|-----------|
| **SharedCollection** | Id, OwnerUserId, Name, Visibility, Slug/Token, CreatedAt, RevokedAt (nullable) |
| **SharedCollectionItem** | SharedCollectionId, FavoriteId |

### 5.3 Notable Database Constraints and Indexes

- `(UserId, Url)` — index to support fast duplicate detection on add; uniqueness policy is configurable (allow or prevent duplicates per user).
- `(UserId, CreatedAt)` — index to support default sort.
- Full-text search index on `(Title, Notes)` — may be added in a later iteration; initial implementation uses `ILIKE` queries.
- EF Core 10 migrations are the sole mechanism for applying schema changes.

---

## 6. UI/UX Requirements

### 6.1 Screens (Phase 1)

| Screen | Purpose |
|--------|---------|
| Login / Register / Forgot Password | Account access and recovery |
| Favorites List (main dashboard) | Browse, search, filter, and sort all Favorites |
| Add / Edit Favorite | Create or modify a single Favorite; shows metadata fetch progress |
| Trash | View and restore or permanently delete soft-deleted Favorites |
| Tag Management | Create, rename, and delete tags |
| Folder Management | Create, rename, and delete folders |
| Import Wizard | Upload browser HTML export; preview, configure, and confirm import |
| Export | Download Favorites as HTML or JSON |
| Settings | User preferences, account data export, account deletion |

### 6.2 Interaction Rules

| ID | Rule |
|----|------|
| UX-001 | A search bar is always visible at the top of the Favorites List screen. |
| UX-002 | An "Add Favorite" action is prominently accessible from every screen (e.g., fixed button or top-navigation item). |
| UX-003 | Adding a Favorite requires only a URL at minimum; the form saves immediately and shows metadata loading progress. |
| UX-004 | When a URL is pasted into the URL field, the system pre-fills available metadata fields automatically after a brief delay. |
| UX-005 | Folder and tag filters are shown in a sidebar or collapsible panel adjacent to the main list. |
| UX-006 | The layout is responsive and usable on mobile-width screens; primary actions have adequately sized tap targets. |
| UX-007 | Bulk selection (select multiple Favorites for tag/move/delete) is a Phase 2 enhancement unless prioritized in MVP. |

---

## 7. MVP Acceptance Criteria

- [ ] A new user can register, log in, and log out successfully.
- [ ] A logged-in user can add a Favorite with only a URL; the title is populated server-side when the page is reachable.
- [ ] A logged-in user cannot view, edit, or delete another user's Favorites.
- [ ] A user can create flat folders, assign Favorites to them, and rename or delete folders without losing Favorites.
- [ ] A user can create tags, assign them to Favorites, and filter the list by tag.
- [ ] Global search returns Favorites matching title, URL, notes, or tag name, and respects folder and status filters.
- [ ] A user can import a standard browser HTML bookmark export; duplicate handling behaves according to the selected strategy.
- [ ] A user can export all Favorites to both HTML and JSON formats and the files are valid.
- [ ] The application deploys on Windows Server 2019, connects to a PostgreSQL database, and serves pages over HTTPS.
- [ ] Deployment steps are documented and reproducible from the provided guide.

---

## 8. Open Questions

| # | Question | Status |
|---|----------|--------|
| OQ-1 | Should the administrator role (FR-004) be included in MVP or deferred? | Open |
| OQ-2 | Should "last opened" click tracking (FR-071) be included in MVP or deferred? | Open |
| OQ-3 | Preferred HTTPS certificate approach for on-prem IIS: self-signed, internal CA, or Let's Encrypt? | Open |
