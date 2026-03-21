## Plan: Bookmark CRUD Across Console and Blazor

Implement bookmark domain + management logic in a new shared class library, integrate it into both console and Blazor apps, and use SQL Server with EF Core for persistence. The web app will provide full Create/Update/Delete + list UI, while the console app will seed and display bookmarks to prove shared-domain reuse. Tests will target manager behavior and core CRUD workflows.

**Steps**
1. Phase 1 - Create shared architecture baseline
2. Add a new class library project (AzenetOne.Bookmarks.Core) targeting net10.0, then add it to the solution and reference it from the console app, web app, and test project. This step blocks all code integration work.
3. Add EF Core packages for SQL Server provider and design-time tooling to the new core project and web project where needed for migrations/runtime wiring. Depends on step 2.
4. Define the domain entity and persistence contract in the core project: Bookmark with Id, Title, Url, Description, Tags, plus validation constraints and optional helper methods for normalization. Depends on step 2.
5. Define BookmarkDbContext in the core project with DbSet<Bookmark> and model configuration (max lengths, required fields, URL constraints, tags storage strategy). Depends on steps 3-4.

6. Phase 2 - Implement BookmarkManager business logic
7. Add IBookmarkManager contract in core for CRUD operations (create, list, get by id, update, delete) and input validation behavior. Depends on step 4.
8. Implement BookmarkManager using BookmarkDbContext with async EF Core operations, explicit not-found handling, and save semantics for create/update/delete. Depends on steps 5 and 7.
9. Add optional data seeding helper for development bootstrap so console and web can show records without manual DB prep. Parallel with step 8.

10. Phase 3 - Integrate into Blazor web app
11. Update web startup to register BookmarkDbContext (SQL Server connection string), BookmarkManager service, and related DI dependencies. Depends on steps 5 and 8.
12. Update appsettings (and development settings) with SQL Server connection configuration and a dedicated connection string key for bookmarks DB. Depends on step 11.
13. Add a dedicated bookmarks page route with interactive form + table UI:
14. List existing bookmarks.
15. Create bookmark form (Title, Url, Description, Tags).
16. Edit existing bookmark values.
17. Delete bookmark with confirmation.
18. Show validation errors and operation feedback states.
19. Depends on step 11.
20. Update navigation menu to expose Bookmarks page entry and preserve existing layout/routing conventions. Depends on step 13.
21. Add startup-time DB initialization strategy (at minimum EnsureCreated or migrations-based approach selected for dev flow). Depends on steps 11-12.

22. Phase 4 - Integrate into console app
23. Replace placeholder console Program with Generic Host setup that registers BookmarkDbContext + BookmarkManager against the same SQL Server connection strategy (or explicit local dev override), then run a short demo flow that creates and displays bookmarks. Depends on steps 8 and 12.
24. Keep console behavior deterministic for repeat runs (for example: add known sample entries only if missing). Depends on step 23.

25. Phase 5 - Testing and verification
26. Replace placeholder unit test file with focused BookmarkManager tests in test project, covering create/list/get/update/delete and expected failure paths (invalid input, not found). Depends on step 8.
27. If needed, use isolated test DB strategy (LocalDB test database per run or provider abstraction) to keep tests reliable. Depends on step 26.
28. Run full test suite and confirm web + console startup flows execute without runtime DI or DB errors. Depends on steps 20, 24, 26.

**Relevant files**
- AzenetOne.Bookmarks/AzenetOne.Bookmarks.csproj - add reference to shared core project.
- AzenetOne.Bookmarks/Program.cs - replace Hello World with host-based create/display bookmark demo.
- AzenetOne.Bookmarks.Web/AzenetOne.Bookmarks.Web.csproj - add core project reference and EF runtime dependencies.
- AzenetOne.Bookmarks.Web/Program.cs - register DbContext and manager in DI, configure DB initialization.
- AzenetOne.Bookmarks.Web/appsettings.json - add SQL Server connection string.
- AzenetOne.Bookmarks.Web/appsettings.Development.json - add development connection override.
- AzenetOne.Bookmarks.Web/Components/Layout/NavMenu.razor - add bookmarks navigation entry.
- AzenetOne.Bookmarks.Web/Components/Pages/Home.razor - optional link/summary entry point, only if desired.
- AzenetOne.Bookmarks.Tests/AzenetOne.Bookmarks.Tests.csproj - update references if core split requires direct reference.
- AzenetOne.Bookmarks.Tests/UnitTest1.cs - replace with meaningful BookmarkManager tests.
- AzenetOne.Bookmarks.slnx - include new core project and updated project references.

**Verification**
1. Build solution and confirm all projects restore and compile.
2. Run tests in AzenetOne.Bookmarks.Tests and verify CRUD behavior assertions pass.
3. Launch web app, navigate to Bookmarks page, and manually verify create/update/delete/list against SQL Server persistence.
4. Restart web app and confirm data persists in SQL Server.
5. Run console app and verify it can create and display bookmarks using shared manager/domain logic.
6. Confirm navigation entry exists and routes correctly from sidebar.

**Decisions**
- Include: full CRUD in Blazor UI, SQL Server + EF Core, shared core class library, and unit tests.
- Bookmark v1 schema: Id, Title, Url, Description, Tags.
- Exclude for this iteration: authentication/authorization, advanced tagging taxonomy, search/filter/sort UX enhancements, import/export, and API endpoints.
- Recommended tags storage for v1: single delimited string column with manager-level normalization, unless relational tag tables are requested in a future phase.

**Further Considerations**
1. Migrations strategy recommendation: use EF Core migrations (preferred) rather than EnsureCreated for long-term schema evolution.
2. SQL Server local development recommendation: use LocalDB or Docker SQL Server profile and document one default connection string for both console and web.
3. If data volume grows, split BookmarkManager read operations into paged query methods before adding UI filtering.
