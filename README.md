# SubmiSoon

ASP.NET Core 8 Web API for managing student assessments, submissions, and leaderboards.

---

## 🚀 Quick Start

### Prerequisites

**Option A: Visual Studio 2022** (Recommended)
- Download [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/) (free)
- Install workloads: "ASP.NET and web development" + "Data storage and processing"
- Includes: .NET 8 SDK + SQL Server LocalDB

**Option B: .NET CLI**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (free)

**Optional Tools:**
- [SSMS](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) - Database management
- [Postman](https://www.postman.com/downloads/) - API testing

### Installation

**1. Clone the repository**
```bash
git clone <repository-url>
cd SubmiSoonProject
```

**2. Configure Secrets** (First-time setup only)

Before running the app, you need to set up configuration secrets:

```powershell
# Copy the template file
Copy-Item SubmiSoonProject\appsettings.json.template SubmiSoonProject\appsettings.json

# Generate secrets using PowerShell
# JWT Secret (64 characters)
-join ((0..31) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })

# URL Signing Secret (128 characters)
-join ((0..63) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })
```

Open `SubmiSoonProject\appsettings.json` and replace the placeholder values with your generated secrets.

**Detailed instructions:** See [docs/SETUP_SECRETS.md](docs/SETUP_SECRETS.md)

**3. Run the application**

Using Visual Studio:

Using Visual Studio:
- Open `SubmiSoonProject.sln`
- Press `F5` to run
- Swagger UI opens automatically

Using CLI:
```bash
dotnet run --project SubmiSoonProject
```

**3. Configure Secrets** (First-time setup only)

Before running the app, you need to set up configuration secrets:

```powershell
# Copy the template file
Copy-Item SubmiSoonProject\appsettings.json.template SubmiSoonProject\appsettings.json

# Generate secrets using PowerShell
# JWT Secret (64 characters)
-join ((0..31) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })

# URL Signing Secret (128 characters)
-join ((0..63) | ForEach-Object { '{0:x2}' -f (Get-Random -Minimum 0 -Maximum 256) })
```

Open `SubmiSoonProject\appsettings.json` and replace the placeholder values with your generated secrets.

**Detailed instructions:** See [docs/SETUP_SECRETS.md](docs/SETUP_SECRETS.md)

**4. Access the API**
- Swagger UI: `https://localhost:7123/swagger`
- API Base: `https://localhost:7123`

The app will automatically:
- Create the database
- Run migrations
- Seed sample data (77 users, 15 assessments, 80 questions)

---

## 🔑 Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Student | tisha.jillian@sunib.ac.id | password2k25 |
| Student | alice.johnson@sunib.ac.id | password123 |
| Lecturer | jane.smith@sunib.edu | password123 |

---

## 📚 Key Features

- **JWT Authentication** - Secure login with Bearer tokens
- **Multiple Question Types** - MCQ, essay, file uploads
- **Draft & Submit** - Save progress, submit when ready
- **Auto-grading** - Instant MCQ scoring
- **Leaderboard** - Track student rankings
- **Secure Files** - HMAC-signed URLs for downloads

---

## 🔌 Main Endpoints

### Authentication
```bash
POST /login              # Get JWT token
POST /register           # Create account
```

### Assessments
```bash
GET  /assessments/incomplete           # List active assessments
GET  /assessments/incomplete/{id}      # Get assessment details
PUT  /assessments/incomplete/{id}      # Save draft
POST /assessments/incomplete/{id}/submit  # Submit for grading
GET  /assessments                      # List completed assessments
GET  /assessments/{id}                 # View results
```

### Leaderboard
```bash
GET /leaderboard         # All students
GET /leaderboard/my-rank # Your rank
```

### Files
```bash
GET /api/files/{fileId}  # Download/preview files
```

---

## 🧪 Testing the API

### 1. Using Swagger (Easiest)

1. Open `https://localhost:7123/swagger`
2. **Login** first:
   - Execute `POST /login` with test account
   - Copy the `accessToken` from response
3. Click **"Authorize"** button (top-right)
   - Enter token (Swagger adds "Bearer" prefix automatically)
   - Click "Authorize" then "Close"
4. Try other endpoints

### 2. Using Postman

Import collection: `docs/SubmiSoon-API-Tests.postman_collection.json`

**Test Data Reference:** See `docs/POSTMAN_TEST_DATA_REFERENCE.md` for:
- Valid assessment IDs to use
- Question IDs for testing
- MCQ options and correct answers
- Complete test scenarios

---

## 🛠️ Development

### Database Management

**Reset database:**
```bash
dotnet ef database drop --force --project SubmiSoonProject
dotnet run --project SubmiSoonProject
```

**Add migration (after model changes):**
```bash
dotnet ef migrations add MigrationName --project SubmiSoonProject
```

**Run with hot reload:**
```bash
dotnet watch run --project SubmiSoonProject
```

### Configuration

**Important:** `appsettings.json` is excluded from Git for security reasons. You must:
1. Copy `SubmiSoonProject/appsettings.json.template` to `SubmiSoonProject/appsettings.json`
2. Generate and configure your own secrets (see [docs/SETUP_SECRETS.md](docs/SETUP_SECRETS.md))
3. **Never commit `appsettings.json` with real secrets**

**To customize database connection:**

Edit your local `SubmiSoonProject/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SubmiSoonDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "ExpiresInMinutes": 60
  }
}
```

---

## 📖 Documentation

| File | Description |
|------|-------------|
| `docs/SETUP_SECRETS.md` | Configuration secrets setup guide |
| `docs/APICONTRACT_v1.3.md` | Complete API specification |
| `docs/POSTMAN_TEST_DATA_REFERENCE.md` | Valid test IDs and scenarios |
| `docs/ERD.md` | Database schema diagram |

---

## 🏗️ Tech Stack

- **Framework:** ASP.NET Core 8
- **Database:** SQL Server + Entity Framework Core
- **Auth:** JWT Bearer tokens
- **Security:** HMAC-SHA256 signed URLs for files
- **API Docs:** Swagger/OpenAPI
- **Architecture:** Repository Pattern + Unit of Work

---

## ⚠️ Troubleshooting

**Can't connect to database?**
```bash
# Check SQL Server is running
sc query MSSQLSERVER

# Test connection
sqlcmd -S localhost -E -Q "SELECT 1"
```

**Port already in use?**
- Change ports in `Properties/launchSettings.json`

**JWT token expired?**
- Tokens last 60 minutes - login again

**File uploads failing?**
- Max file size: 2MB per file
- Allowed types: .pdf, .doc, .docx, .zip, .jpg, .png
- Total request limit: 10MB

---

## 📝 Sample Data

The database seeds with:
- **77 users** (2 lecturers, 75 students)
- **15 assessments** (quizzes, assignments, projects)
- **80 questions** (41 MCQ, 23 essay, 16 file upload)
- **~325 submissions** (varied completion rates)
- **3 courses** (CS101, CS201, CS301)

## 📧 Support

**Issues?** Contact:
- tishajillian16@gmail.com
- tisha.jillian@binus.ac.id

---

**Built by Tisha Jillian for BINUS IT Division**
