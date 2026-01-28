Before you begin, ensure you have the following installed:

•	.NET 8.0 SDK or later

•	SQL Server 2019+ (Express/Developer edition)

•	Visual Studio 2022 (recommended) or Visual Studio Code

•	SQL Server Management Studio (SSMS) (optional, for database management)

---

🚀 Installation \& Configuration

1\. Clone the Repository

git clone https://github.com/LilyStoner/FuNewsManagement.git

cd FuNewsManagement



2\. Database Setup

Option A: Using Entity Framework Migrations

cd Assigment1\_PRN232

dotnet ef database update



3\. Configure Connection Strings

Backend API (Assigment1\_PRN232/appsettings.json):

{

&nbsp; "ConnectionStrings": {

&nbsp;   "MyCnn": "Server=YOUR\_SERVER\_NAME;Database=FUNewsManagement;Trusted\_Connection=True;TrustServerCertificate=True;"

&nbsp; },

&nbsp; "JwtSettings": {

&nbsp;   "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123!",

&nbsp;   "Issuer": "FUNewsAPI",

&nbsp;   "Audience": "FUNewsClient",

&nbsp;   "ExpiryMinutes": 60

&nbsp; }

}

Frontend (Assignment01\_FE/appsettings.json):

{

&nbsp; "ApiSettings": {

&nbsp;   "BaseUrl": "https://localhost:7215"

&nbsp; },

&nbsp; "Logging": {

&nbsp;   "LogLevel": {

&nbsp;     "Default": "Information",

&nbsp;     "Microsoft.AspNetCore": "Warning"

&nbsp;   }

&nbsp; }

}

🎮 Running the Application

Method 1: Using Visual Studio 2022 (Recommended)

1\.	Open FuNewsManagement.sln in Visual Studio 2022

2\.	Right-click on the solution → Properties

3\.	Select Multiple Startup Projects

4\.	Set both projects to Start:

•	Assigment1\_PRN232\_BE (Backend API)

•	Assignment1\_PRN232\_FE (Frontend Web)

5\.	Press F5 or click Start

Application URLs:

•	Backend API: https://localhost:7215

•	Swagger UI: https://localhost:7215/swagger

•	Frontend Web: https://localhost:7001

Method 2: Using Command Line

Open two terminal windows:

Terminal 1 - Backend API:

cd Assigment1\_PRN232

dotnet run

Terminal 2 - Frontend:

cd Assignment01\_FE

dotnet run



📡 API Endpoints Overview

Base URL: https://localhost:7215

🔐 Authentication Endpoints

Method	Endpoint	Description	Auth Required

POST	/api/Auth/Login	User login with email/password	No

POST	/api/Auth/Logout	User logout	Yes

{

&nbsp; "email": "admin@funews.com",

&nbsp; "password": "admin123"

}

{

&nbsp; "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",

&nbsp; "account": {

&nbsp;   "accountId": 1,

&nbsp;   "accountName": "Admin User",

&nbsp;   "accountEmail": "admin@funews.com",

&nbsp;   "accountRole": 3

&nbsp; },

&nbsp; "expiresAt": "2024-12-25T10:00:00Z"

}

---

📰 News Articles (OData CRUD)

Method	Endpoint	Description	Auth	Role

GET	/odata/NewsArticles	Get all articles	No	All

GET	/odata/NewsArticles('{id}')	Get article by ID (full content)	No	All

POST	/odata/NewsArticles	Create new article	Yes	Staff/Lecturer

PUT	/odata/NewsArticles('{id}')	Update article	Yes	Staff/Lecturer

DELETE	/odata/NewsArticles('{id}')	Delete article	Yes	Staff/Lecturer

OData Query Examples:

\# Get published articles only

GET /odata/NewsArticles?$filter=NewsStatus eq true



\# Get articles with related entities

GET /odata/NewsArticles?$expand=Category,CreatedBy,Tags



\# Search by title

GET /odata/NewsArticles?$filter=contains(NewsTitle,'Technology')



\# Sort and paginate

GET /odata/NewsArticles?$orderby=CreatedDate desc\&$top=10\&$skip=0



\# Complex query

GET /odata/NewsArticles?$filter=NewsStatus eq true and CategoryId eq 1\&$expand=Category,Tags\&$orderby=CreatedDate desc



---

📰 News Articles Functions (Custom Operations)

Method	Endpoint	Description	PageSize

GET	/odata/NewsArticlesFunctions/Active	Get published articles (summary data)	7

GET	/odata/NewsArticlesFunctions/Search	Search with multiple filters	7

GET	/odata/NewsArticlesFunctions/ByAuthor?authorId={id}	Get articles by author	7

GET	/odata/NewsArticlesFunctions/ByCategory?categoryId={id}	Get articles by category	7

GET	/odata/NewsArticlesFunctions/Related?articleId={id}\&limit=5	Get related articles	7

---

📁 Categories (OData CRUD)

Method	Endpoint	Description	Auth	Role

GET	/odata/Categories	Get all categories	No	All

GET	/odata/Categories({id})	Get category by ID	No	All

POST	/odata/Categories	Create new category	Yes	Staff

PUT	/odata/Categories({id})	Update category	Yes	Staff

DELETE	/odata/Categories({id})	Delete category (if not in use)	Yes	Staff

Category Rules:

•	Cannot delete if ArticleCount > 0

•	Cannot change ParentCategoryId if category has articles

•	Name must be unique

---

📁 Categories Functions

Method	Endpoint	Description	PageSize

GET	/odata/CategoriesFunctions/Active	Active categories with article counts	50

GET	/odata/CategoriesFunctions/Search?name={name}\&description={desc}	Search categories	50

Article Count Implementation:

\# Backend expands NewsArticles and counts them

GET /odata/Categories?$expand=NewsArticles($select=NewsArticleId)



\# Returns:

{

&nbsp; "categoryId": 1,

&nbsp; "categoryName": "Technology",

&nbsp; "articleCount": 15  // Calculated from NewsArticles.Count

}

---

🏷️ Tags (OData CRUD)

Method	Endpoint	Description	Auth	Role

GET	/odata/Tags	Get all tags	No	All

GET	/odata/Tags({id})	Get tag by ID	No	All

POST	/odata/Tags	Create new tag	Yes	Staff

PUT	/odata/Tags({id})	Update tag (name \& note)	Yes	Staff

DELETE	/odata/Tags({id})	Delete tag (if not in use)	Yes	Staff

Create Tag Request:

{

&nbsp; "tagName": "AI",

&nbsp; "note": "Artificial Intelligence related articles"

}

Tag Rules:

•	Cannot delete if referenced in NewsTag table

•	Name must be unique

•	Note/description is optional but recommended

---

🏷️ Tags Functions

Method	Endpoint	Description

GET	/odata/TagsFunctions/Search?tagName={name}	Search tags by name or note

GET	/odata/TagsFunctions/ArticlesByTag?tagId={id}	Get all articles using specific tag (JOIN query)

---

👥 System Accounts (OData CRUD - Admin Only)

Method	Endpoint	Description	Auth	Role

GET	/odata/SystemAccounts	Get all user accounts	Yes	Admin

GET	/odata/SystemAccounts({id})	Get account by ID	Yes	Admin

POST	/odata/SystemAccounts	Create new account	Yes	Admin

PUT	/odata/SystemAccounts({id})	Update account	Yes	Admin

DELETE	/odata/SystemAccounts({id})	Delete account (if no articles)	Yes	Admin

Account Roles:

•	1 = Staff (Can manage articles, categories, tags)

•	2 = Lecturer (Can manage own articles)

•	3 = Admin (Full system access)

---

👥 System Accounts Functions

Method	Endpoint	Description	Auth	Role

GET	/odata/SystemAccountsFunctions/Search?name={name}\&email={email}\&role={role}	Search accounts	Yes	Admin

POST	/odata/SystemAccountsFunctions/ChangePassword	Change own password	Yes	All

---

📊 Reports Endpoints (Admin Only)

Method	Endpoint	Description

GET	/api/Reports/Dashboard	Get dashboard statistics with monthly trends

GET	/api/Reports/Category?startDate={date}\&endDate={date}	Category usage report for date range

GET	/api/Reports/Author?startDate={date}\&endDate={date}	Author productivity report

GET	/api/Reports/Monthly?year={year}	Monthly article statistics for specific year

GET	/api/Reports/TopAuthors?limit={limit}	Top N productive authors

GET	/api/Reports/TopCategories?limit={limit}	Top N used categories

GET	/api/Reports/TagUsage	Tag usage statistics

---

🔑 User Roles \& Test Credentials

Default Test Accounts

Role	Email	Password	Access Level

Admin	admin@funews.com	admin123	Full system access

Staff	staff@funews.com	staff123	Articles, categories, tags management

Lecturer	lecturer@funews.com	lecturer123	Own articles only

Detailed Permission Matrix

Feature	Admin	Staff	Lecturer	Public

Articles	View Published Articles	✅	✅	✅	✅

Create New Articles	✅	✅	✅	❌

Edit Own Articles	✅	✅	✅	❌

Delete Own Articles	✅	✅	✅	❌

View All Articles	✅	✅	❌	❌

Categories	View Categories	✅	✅	✅	✅

Manage Categories	✅	✅	❌	❌

Delete Categories	✅	✅	❌	❌

Tags	View Tags	✅	✅	✅	✅

Manage Tags	✅	✅	❌	❌

Delete Tags	✅	✅	❌	❌

Users	View All Users	✅	❌	❌	❌

Manage Users	✅	❌	❌	❌

Reports	View Dashboard	✅	❌	❌	❌

Generate Reports	✅	❌	❌	❌

Export to Excel	✅	✅	❌	❌

Other	Change Own Password	✅	✅	✅	❌

View Profile	✅	✅	✅	❌

---

📁 Project Structure

Backend API (Assigment1\_PRN232)

Assigment1\_PRN232/

├── Controllers/

│   ├── AuthController.cs                    # JWT authentication

│   ├── NewsArticlesController.cs            # OData CRUD for articles

│   ├── NewsArticlesFunctionsController.cs   # Custom article operations

│   ├── CategoriesController.cs              # OData CRUD for categories

│   ├── CategoriesFunctionsController.cs     # Custom category operations

│   ├── TagsController.cs                    # OData CRUD for tags

│   ├── TagsFunctionsController.cs           # Custom tag operations

│   ├── SystemAccountsController.cs          # OData CRUD for accounts

│   ├── SystemAccountsFunctionsController.cs # Custom account operations

│   └── ReportsController.cs                 # Reports and analytics

│

├── Services/

│   ├── INewsArticleService.cs              # Article service interface

│   ├── NewsArticleService.cs               # Article business logic

│   │   ├── GetActiveNewsArticlesSummaryAsync()    # Summary without content

│   │   ├── GetNewsArticleByIdAsync()              # Full content for details

│   │   └── GetRelatedNewsAsync()                  # Related articles

│   ├── ICategoryService.cs

│   ├── CategoryService.cs                  # Category business logic

│   ├── ITagService.cs

│   ├── TagService.cs                       # Tag business logic + JOIN queries

│   ├── IAccountService.cs

│   ├── AccountService.cs                   # Account management + password

│   ├── IAuthService.cs

│   ├── AuthService.cs                      # JWT token generation

│   ├── IReportService.cs

│   └── ReportService.cs                    # Report generation logic

│

├── Repositories/

│   ├── IRepository.cs                      # Generic repository interface

│   ├── Repository.cs                       # Generic repository implementation

│   ├── IUnitOfWork.cs                      # Unit of Work pattern interface

│   └── UnitOfWork.cs                       # Unit of Work implementation

│

├── Models/

│   ├── NewsArticle.cs                      # Article entity

│   ├── Category.cs                         # Category entity (hierarchical)

│   ├── Tag.cs                              # Tag entity

│   ├── SystemAccount.cs                    # User account entity

│   └── FunewsManagementContext.cs          # EF Core DbContext

│

├── DTOs/

│   ├── NewsArticleDto.cs                   # Article DTOs (Create, Update)

│   ├── CategoryDto.cs                      # Category DTOs

│   ├── TagDto.cs                           # Tag DTOs (Create, Update)

│   └── SystemAccountDto.cs                 # Account DTOs

│

└── Program.cs                               # API startup + OData configuration



Frontend Razor Pages (Assignment01\_FE)

Assignment01\_FE/

├── Pages/

│   ├── Admin/

│   │   ├── Dashboard.cshtml/.cs           # Admin dashboard with stats

│   │   ├── Reports.cshtml/.cs             # Reports with Excel export

│   │   └── Accounts/

│   │       ├── Index.cshtml/.cs          # User management

│   │       ├── Create.cshtml/.cs         # Create user

│   │       └── Edit.cshtml/.cs           # Edit user

│   │

│   ├── Staff/

│   │   ├── Dashboard.cshtml/.cs           # Staff dashboard

│   │   ├── Categories.cshtml/.cs          # Category CRUD + article counts

│   │   ├── Articles/

│   │   │   ├── Index.cshtml/.cs          # Article list with filters

│   │   │   ├── Create.cshtml/.cs         # Create article + tags

│   │   │   └── Edit.cshtml/.cs           # Edit article + tags

│   │   └── Tags/

│   │       ├── Index.cshtml/.cs          # Tag management + counts

│   │       └── Articles.cshtml/.cs       # Articles by tag (JOIN view)

│   │

│   ├── News/

│   │   ├── Active.cshtml/.cs             # Public news feed (summary data)

│   │   ├── Details.cshtml/.cs            # Article details (full content)

│   │   └── Search.cshtml/.cs             # Advanced search

│   │

│   ├── Shared/

│   │   ├── \_Layout.cshtml                # Main layout with navigation

│   │   ├── \_LoginLayout.cshtml           # Login page layout

│   │   └── \_PaginationPartial.cshtml     # Reusable pagination component

│   │

│   ├── Login.cshtml/.cs                   # Login page

│   ├── Logout.cshtml/.cs                  # Logout handler

│   ├── Profile.cshtml/.cs                 # User profile + password change

│   └── Index.cshtml                       # Home page (redirects to Active)

│

├── Services/

│   ├── IApiService.cs                     # API client interface

│   ├── ApiService.cs                      # HTTP client service with JWT

│   └── ExcelExportService.cs              # Excel export using EPPlus

│

├── Models/

│   └── ViewModels.cs                      # All view models and DTOs

│       ├── CategoryModel (with ArticleCount)

│       ├── TagModel (with ArticleCount)

│       ├── NewsArticleModel

│       ├── SystemAccountModel

│       ├── PaginationInfo

│       └── DashboardStatisticsModel

│

└── wwwroot/

&nbsp;   ├── css/

&nbsp;   │   ├── custom.css                     # Custom styles

&nbsp;   │   └── site.css                       # Site-wide styles

&nbsp;   ├── js/

&nbsp;   │   └── site.js                        # Client-side scripts

&nbsp;   └── images/                            # Static images and logos



---

🛠️ Technologies Used

Backend Stack

•	Framework: ASP.NET Core 8.0 Web API

•	API Protocol: OData v8.2 (enables flexible querying)

•	ORM: Entity Framework Core 8.0

•	Database: SQL Server 2019+

•	Authentication: JWT Bearer Tokens

•	Validation: Data Annotations + FluentValidation

•	Documentation: Swagger/OpenAPI

Frontend Stack

•	Framework: ASP.NET Core Razor Pages 8.0

•	UI Framework: Bootstrap 5.3

•	Icons: Font Awesome 6.0

•	HTTP Client: HttpClientFactory pattern

•	Session: Distributed Session State

•	Excel Export: EPPlus library

Development Tools

•	IDE: Visual Studio 2022 / VS Code

•	Version Control: Git \& GitHub

•	Package Manager: NuGet

•	Build Tool: MSBuild / dotnet CLI

Key Design Patterns

•	Repository Pattern: Generic Repository<T> for CRUD

•	Unit of Work Pattern: Transaction management

•	Service Layer Pattern: Business logic separation

•	DTO Pattern: Data transfer optimization

•	Dependency Injection: Built-in ASP.NET Core DI

---



🙏 Acknowledgments

•	FPT University for project guidelines and requirements

•	ASP.NET Core team for excellent framework and documentation

•	OData team for flexible query capabilities

•	Bootstrap team for responsive UI framework

•	Open-source community for libraries and tools

---

Last Updated: December 2024

Version: 1.0.0

Status: ✅ Production Ready

---

