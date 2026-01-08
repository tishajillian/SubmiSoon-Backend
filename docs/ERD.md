# SubmiSoon Database Schema (ERD)

> **Last Updated:** January 4, 2026  
> **Database:** SQL Server  
> **ORM:** Entity Framework Core 8.0

---

## 📊 Entity-Relationship Diagram

This document describes the complete database schema for SubmiSoon, including all tables, columns, relationships, and constraints.

---

## 🎨 Visual Overview (Text-Based)

```
┌─────────────┐
│   Faculty   │
└──────┬──────┘
       │ 1:N
       ▼
┌──────────────┐      ┌──────────────┐
│ProgramStudy  │◄──┬──│   Students   │
└──────┬───────┘   │  └──────┬───────┘
       │ 1:N       │         │ 1:1
       │           │         ▼
       │           │  ┌─────────────┐      ┌──────────────────┐
       │           │  │    Users    │◄─────│  RefreshTokens   │
       │           │  └──────┬──────┘      └──────────────────┘
       │           │         │ 1:1               N:1
       │           │         ▼
       │           └──┌──────────────┐
       │              │  Lecturers   │
       └──────────────┤              │
           1:N        └──────┬───────┘
                             │ 1:N
                             ▼
                      ┌─────────────┐      ┌──────────────────┐
                      │    Class    │◄─────│ StudentEnrollment│
                      └──────┬──────┘      └──────────────────┘
                             │ 1:N               N:N bridge
                             ▼
                      ┌──────────────┐
                      │  Assessment  │◄─────────┐
                      └──────┬───────┘          │
                             │ 1:N              │
                             ▼                  │
                      ┌──────────────┐          │
                      │  Questions   │          │
                      └──────┬───────┘          │
                             │ 1:N              │ N:1
                             ▼                  │
                      ┌──────────────┐          │
                      │  McqOptions  │          │
                      └──────────────┘          │
                                                │
┌──────────────┐      ┌────────────────────┐   │
│ UserAssessment│─────►│     Answers       │   │
└───────┬──────┘ 1:N  └────────┬───────────┘   │
        │                      │                │
        │ N:1                  │ N:1            │
        ▼                      ▼                │
  ┌──────────┐          ┌──────────┐           │
  │  Users   │          │  Files   │───────────┘
  └──────────┘          └──────────┘
                             │ N:1
                             ▼
                        ┌──────────┐
                        │  Users   │
                        └──────────┘

  ┌──────────────────┐      ┌──────────────┐
  │  AcademicTerms   │◄─────│    Class     │
  └──────────────────┘ 1:N  └──────┬───────┘
                                   │ N:1
                                   ▼
                            ┌──────────────┐
                            │    Course    │
                            └──────────────┘
```

---

## 📋 Tables

### 1. users
**Purpose:** Base table for all system users (students and lecturers).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `UserId` | INT | PK, AUTO_INCREMENT | Unique user identifier |
| `Name` | VARCHAR | NOT NULL | Full name |
| `Email` | VARCHAR | UNIQUE, NOT NULL | Email address (used for login) |
| `PasswordHash` | VARCHAR | NOT NULL | Argon2id hash (format: salt.hash) |
| `Role` | ENUM | NOT NULL | 'student' or 'lecturer' |

**Indexes:**
- Primary Key: `UserId`
- Unique Index: `Email`

**C# Entity:**
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
}

public enum UserRole
{
    student,
    lecturer
}
```

**Relationships:**
- 1:1 → Students (via UserId)
- 1:1 → Lecturers (via UserId)
- 1:N → RefreshTokens
- 1:N → UserAssessments
- 1:N → Files

---

### 2. faculty
**Purpose:** Academic faculties within the university.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `FacultyId` | INT | PK, AUTO_INCREMENT | Unique faculty identifier |
| `Name` | VARCHAR | UNIQUE, NOT NULL | Faculty name |

**Indexes:**
- Primary Key: `FacultyId`
- Unique Index: `Name`

**C# Entity:**
```csharp
public class Faculty
{
    public int FacultyId { get; set; }
    public string Name { get; set; } = null!;
    
    // Navigation
    public ICollection<ProgramStudy> ProgramStudies { get; set; } = new List<ProgramStudy>();
}
```

**Relationships:**
- 1:N → ProgramStudy

**Example Data:**
- Faculty of Engineering
- Faculty of Computer Science
- Faculty of Business

---

### 3. program_study
**Purpose:** Study programs within faculties.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `ProgramStudyId` | INT | PK, AUTO_INCREMENT | Unique program identifier |
| `FacultyId` | INT | FK → faculty, NOT NULL | Parent faculty |
| `Name` | VARCHAR | UNIQUE, NOT NULL | Program name |

**Indexes:**
- Primary Key: `ProgramStudyId`
- Foreign Key: `FacultyId` → faculty.FacultyId
- Unique Index: `Name`

**C# Entity:**
```csharp
public class ProgramStudy
{
    public int ProgramStudyId { get; set; }
    public int FacultyId { get; set; }
    public string Name { get; set; } = null!;
    
    // Navigation
    public Faculty Faculty { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
}
```

**Relationships:**
- N:1 → Faculty
- 1:N → Students
- 1:N → Lecturers

---

### 4. students
**Purpose:** Extended information for student users.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `UserId` | INT | PK, FK → users, NOT NULL | References Users table (1:1) |
| `StudentId` | VARCHAR | UNIQUE, NOT NULL | Student ID number |
| `EnrollmentYear` | YEAR | NOT NULL | Year of enrollment |
| `ProgramStudyId` | INT | FK → program_study, NOT NULL | Student's program |

**Indexes:**
- Primary Key: `UserId`
- Foreign Key: `UserId` → users.UserId (1:1)
- Foreign Key: `ProgramStudyId` → program_study.ProgramStudyId
- Unique Index: `StudentId`

**C# Entity:**
```csharp
public class Student
{
    public int UserId { get; set; }
    public string StudentId { get; set; } = null!;
    public int EnrollmentYear { get; set; }
    public int ProgramStudyId { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ProgramStudy ProgramStudy { get; set; } = null!;
    public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
}
```

**Relationships:**
- 1:1 → Users
- N:1 → ProgramStudy
- 1:N → StudentEnrollment

---

### 5. lecturers
**Purpose:** Extended information for lecturer users.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `UserId` | INT | PK, FK → users, NOT NULL | References Users table (1:1) |
| `LecturerId` | VARCHAR | UNIQUE, NOT NULL | Lecturer ID number |
| `ProgramStudyId` | INT | FK → program_study, NOT NULL | Lecturer's primary program |

**Indexes:**
- Primary Key: `UserId`
- Foreign Key: `UserId` → users.UserId (1:1)
- Foreign Key: `ProgramStudyId` → program_study.ProgramStudyId
- Unique Index: `LecturerId`

**C# Entity:**
```csharp
public class Lecturer
{
    public int UserId { get; set; }
    public string LecturerId { get; set; } = null!;
    public int ProgramStudyId { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ProgramStudy ProgramStudy { get; set; } = null!;
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
```

**Relationships:**
- 1:1 → Users
- N:1 → ProgramStudy
- 1:N → Class

---

### 6. refresh_tokens
**Purpose:** Store refresh tokens for JWT authentication.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `RefreshTokenId` | INT | PK, AUTO_INCREMENT | Unique token identifier |
| `UserId` | INT | FK → users, NOT NULL | Token owner |
| `TokenHash` | VARCHAR | NOT NULL | Hashed refresh token |
| `ExpiredAt` | TIMESTAMP | NOT NULL | Expiration date |
| `RevokedAt` | TIMESTAMP | NULL | Revocation date (null if active) |
| `CreatedAt` | TIMESTAMP | NOT NULL | Creation timestamp |

**Indexes:**
- Primary Key: `RefreshTokenId`
- Foreign Key: `UserId` → users.UserId
- Index: `TokenHash` (for fast lookup)
- Index: `ExpiredAt` (for cleanup queries)

**C# Entity:**
```csharp
public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiredAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
}
```

**Relationships:**
- N:1 → Users

**Usage:**
- Generate on login/refresh
- Validate on token refresh
- Revoke on logout
- Cleanup expired tokens periodically

---

### 7. academic_terms
**Purpose:** Academic semesters (e.g., 2025 Odd, 2025 Even).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `AcademicTermId` | INT | PK, AUTO_INCREMENT | Unique term identifier |
| `Year` | INT | NOT NULL | Year (e.g., 2025) |
| `Semester` | ENUM | NOT NULL | 'odd' or 'even' |
| `StartDate` | DATE | NULL | Term start date (optional) |
| `EndDate` | DATE | NULL | Term end date (optional) |

**Indexes:**
- Primary Key: `AcademicTermId`
- Unique Index: (Year, Semester) - prevents duplicate terms

**C# Entity:**
```csharp
public class AcademicTerm
{
    public int AcademicTermId { get; set; }
    public int Year { get; set; }
    public SemesterType Semester { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Navigation
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}

public enum SemesterType
{
    odd,
    even
}
```

**Relationships:**
- 1:N → Class

**Example Data:**
- 2025, odd, 2025-08-01, 2025-12-31
- 2025, even, 2026-01-01, 2026-06-30

---

### 8. course
**Purpose:** Course catalog (e.g., "Database Systems", "Web Programming").

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `CourseId` | INT | PK, AUTO_INCREMENT | Unique course identifier |
| `CourseCode` | VARCHAR | NOT NULL | Course code (e.g., "CS101") |
| `Name` | VARCHAR | UNIQUE, NOT NULL | Course name |

**Indexes:**
- Primary Key: `CourseId`
- Unique Index: `Name`
- Index: `CourseCode`

**C# Entity:**
```csharp
public class Course
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    // Navigation
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
```

**Relationships:**
- 1:N → Class

---

### 9. class
**Purpose:** Specific class instances (e.g., "CS101-A, Fall 2025, Prof. Smith").

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `ClassId` | INT | PK, AUTO_INCREMENT | Unique class identifier |
| `ClassCode` | VARCHAR | NOT NULL | Class code (e.g., "LC01") |
| `CourseId` | INT | FK → course, NOT NULL | Course being taught |
| `LecturerId` | INT | FK → lecturers, NOT NULL | Class instructor |
| `AcademicTermId` | INT | FK → academic_terms, NOT NULL | Academic term |

**Indexes:**
- Primary Key: `ClassId`
- Foreign Key: `CourseId` → course.CourseId
- Foreign Key: `LecturerId` → lecturers.UserId
- Foreign Key: `AcademicTermId` → academic_terms.AcademicTermId

**C# Entity:**
```csharp
public class Class
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = null!;
    public int CourseId { get; set; }
    public int LecturerId { get; set; }
    public int AcademicTermId { get; set; }
    
    // Navigation
    public Course Course { get; set; } = null!;
    public Lecturer Lecturer { get; set; } = null!;
    public AcademicTerm AcademicTerm { get; set; } = null!;
    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
}
```

**Relationships:**
- N:1 → Course
- N:1 → Lecturer
- N:1 → AcademicTerm
- 1:N → Assessment
- 1:N → StudentEnrollment

---

### 10. student_enrollment
**Purpose:** Many-to-many relationship between students and classes.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `StudentEnrollmentId` | INT | PK, AUTO_INCREMENT | Unique enrollment identifier |
| `StudentId` | INT | FK → students, NOT NULL | Enrolled student |
| `ClassId` | INT | FK → class, NOT NULL | Enrolled class |
| `EnrolledAt` | TIMESTAMP | NOT NULL | Enrollment timestamp |
| `Status` | ENUM | NULL | 'active', 'dropped', 'completed' |

**Indexes:**
- Primary Key: `StudentEnrollmentId`
- Foreign Key: `StudentId` → students.UserId
- Foreign Key: `ClassId` → class.ClassId
- Unique Index: (StudentId, ClassId) - student can't enroll twice in same class

**C# Entity:**
```csharp
public class StudentEnrollment
{
    public int StudentEnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus? Status { get; set; }
    
    // Navigation
    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
}

public enum EnrollmentStatus
{
    active,
    dropped,
    completed
}
```

**Relationships:**
- N:1 → Student
- N:1 → Class

---

### 11. assessments
**Purpose:** Assignments/tests created by lecturers for classes.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `AssessmentId` | INT | PK, AUTO_INCREMENT | Unique assessment identifier |
| `ClassId` | INT | FK → class, NOT NULL | Associated class |
| `Title` | VARCHAR | NOT NULL | Assessment title |
| `StartDate` | TIMESTAMP | NOT NULL | When students can start |
| `EndDate` | TIMESTAMP | NOT NULL | Submission deadline |
| `CreatedAt` | TIMESTAMP | NOT NULL | Creation timestamp |
| `UpdatedAt` | TIMESTAMP | NULL | Last update timestamp |

**Indexes:**
- Primary Key: `AssessmentId`
- Foreign Key: `ClassId` → class.ClassId
- Index: `EndDate` (for deadline queries)

**C# Entity:**
```csharp
public class Assessment
{
    public int AssessmentId { get; set; }
    public int ClassId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public Class Class { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<UserAssessment> UserAssessments { get; set; } = new List<UserAssessment>();
    public ICollection<File> Files { get; set; } = new List<File>();
}
```

**Relationships:**
- N:1 → Class
- 1:N → Questions
- 1:N → UserAssessments
- 1:N → Files

---

### 12. questions
**Purpose:** Individual questions within assessments.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `QuestionId` | INT | PK, AUTO_INCREMENT | Unique question identifier |
| `AssessmentId` | INT | FK → assessments, NOT NULL | Parent assessment |
| `Type` | ENUM | NOT NULL | 'essay', 'mcq', 'file' |
| `Content` | TEXT | NOT NULL | Question text |
| `CreatedAt` | TIMESTAMP | NOT NULL | Creation timestamp |
| `UpdatedAt` | TIMESTAMP | NULL | Last update timestamp |

**Indexes:**
- Primary Key: `QuestionId`
- Foreign Key: `AssessmentId` → assessments.AssessmentId

**C# Entity:**
```csharp
public class Question
{
    public int QuestionId { get; set; }
    public int AssessmentId { get; set; }
    public QuestionType Type { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public Assessment Assessment { get; set; } = null!;
    public ICollection<McqOption> McqOptions { get; set; } = new List<McqOption>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

public enum QuestionType
{
    essay,
    mcq,
    file
}
```

**Relationships:**
- N:1 → Assessment
- 1:N → McqOptions (if type = 'mcq')
- 1:N → Answers

---

### 13. mcq_options
**Purpose:** Multiple choice options for MCQ questions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `OptionId` | INT | PK, AUTO_INCREMENT | Unique option identifier |
| `QuestionId` | INT | FK → questions, NOT NULL | Parent question |
| `OptionLabel` | CHAR(1) | NOT NULL | 'A', 'B', 'C', 'D' |
| `OptionText` | VARCHAR | NOT NULL | Option text |
| `IsCorrect` | BOOLEAN | NOT NULL | True if correct answer |

**Indexes:**
- Primary Key: `OptionId`
- Foreign Key: `QuestionId` → questions.QuestionId
- Unique Index: (OptionLabel, OptionText)

**C# Entity:**
```csharp
public class McqOption
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public char OptionLabel { get; set; }
    public string OptionText { get; set; } = null!;
    public bool IsCorrect { get; set; }
    
    // Navigation
    public Question Question { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
```

**Relationships:**
- N:1 → Question
- 1:N → Answers

**Example Data:**
```
QuestionId: 102, OptionLabel: 'A', OptionText: 'Paris', IsCorrect: false
QuestionId: 102, OptionLabel: 'B', OptionText: 'London', IsCorrect: true
QuestionId: 102, OptionLabel: 'C', OptionText: 'Berlin', IsCorrect: false
QuestionId: 102, OptionLabel: 'D', OptionText: 'Rome', IsCorrect: false
```

---

### 14. user_assessments
**Purpose:** Track student submissions for assessments.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `UserAssessmentId` | INT | PK, AUTO_INCREMENT | Unique submission identifier |
| `UserId` | INT | FK → users, NOT NULL | Student who submitted |
| `AssessmentId` | INT | FK → assessments, NOT NULL | Assessment being submitted |
| `Status` | ENUM | NOT NULL | 'draft', 'on_review', 'completed' |
| `Score` | INT | NULL | Final score (null if not graded) |
| `CreatedAt` | TIMESTAMP | NOT NULL | First save timestamp |
| `UpdatedAt` | TIMESTAMP | NULL | Last update timestamp |

**Indexes:**
- Primary Key: `UserAssessmentId`
- Foreign Key: `UserId` → users.UserId
- Foreign Key: `AssessmentId` → assessments.AssessmentId
- Index: `Status` (for filtering by status)
- Index: (UserId, Status) (for user's incomplete/complete lists)

**C# Entity:**
```csharp
public class UserAssessment
{
    public int UserAssessmentId { get; set; }
    public int UserId { get; set; }
    public int AssessmentId { get; set; }
    public AssessmentStatus Status { get; set; }
    public int? Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public Assessment Assessment { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

public enum AssessmentStatus
{
    draft,
    on_review,
    completed
}
```

**Relationships:**
- N:1 → User
- N:1 → Assessment
- 1:N → Answers

**Status Flow:**
- `draft` → Student saving answers (not submitted)
- `on_review` → Student submitted, lecturer reviewing
- `completed` → Lecturer graded, score assigned

---

### 15. files
**Purpose:** Store metadata for uploaded files.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `FileId` | INT | PK, AUTO_INCREMENT | Unique file identifier |
| `UserId` | INT | FK → users, NOT NULL | User who uploaded (auditing) |
| `AssessmentId` | INT | FK → assessments, NOT NULL | Associated assessment |
| `OriginalFilename` | VARCHAR | NOT NULL | User's filename |
| `StoredFilename` | VARCHAR | NOT NULL | Server filename (GUID) |
| `FilePath` | VARCHAR | NOT NULL | Full server path |
| `FileExtension` | VARCHAR | NOT NULL | File extension (.pdf, .jpg) |
| `FileSize` | INT | NOT NULL | Size in bytes |
| `CreatedAt` | TIMESTAMP | NOT NULL | Upload timestamp |
| `UpdatedAt` | TIMESTAMP | NULL | Update timestamp |

**Indexes:**
- Primary Key: `FileId`
- Foreign Key: `UserId` → users.UserId
- Foreign Key: `AssessmentId` → assessments.AssessmentId
- Index: `StoredFilename` (for retrieval)

**C# Entity:**
```csharp
public class File
{
    public int FileId { get; set; }
    public int UserId { get; set; }
    public int AssessmentId { get; set; }
    public string OriginalFilename { get; set; } = null!;
    public string StoredFilename { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
    public int FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public Assessment Assessment { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
```

**Relationships:**
- N:1 → User
- N:1 → Assessment
- 1:N → Answers

**File Validation:**
- Allowed extensions: .doc, .docx, .pdf, .jpg, .jpeg, .png
- Max size: 2MB (2,097,152 bytes)

---

### 16. answers
**Purpose:** Store student answers to questions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `AnswerId` | INT | PK, AUTO_INCREMENT | Unique answer identifier |
| `UserAssessmentId` | INT | FK → user_assessments, NOT NULL | Parent submission |
| `QuestionId` | INT | FK → questions, NOT NULL | Question being answered |
| `AnswerText` | TEXT | NULL | Essay answer text |
| `SelectedOptionId` | INT | FK → mcq_options, NULL | MCQ selected option |
| `FileId` | INT | FK → files, NULL | Uploaded file reference |
| `CreatedAt` | TIMESTAMP | NOT NULL | First save timestamp |
| `UpdatedAt` | TIMESTAMP | NULL | Last update timestamp |

**Indexes:**
- Primary Key: `AnswerId`
- Foreign Key: `UserAssessmentId` → user_assessments.UserAssessmentId
- Foreign Key: `QuestionId` → questions.QuestionId
- Foreign Key: `SelectedOptionId` → mcq_options.OptionId
- Foreign Key: `FileId` → files.FileId
- Unique Index: (UserAssessmentId, QuestionId) - one answer per question per submission

**C# Entity:**
```csharp
public class Answer
{
    public int AnswerId { get; set; }
    public int UserAssessmentId { get; set; }
    public int QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public int? SelectedOptionId { get; set; }
    public int? FileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public UserAssessment UserAssessment { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public McqOption? McqOption { get; set; }
    public File? File { get; set; }
}
```

**Relationships:**
- N:1 → UserAssessment
- N:1 → Question
- N:1 → McqOption (nullable)
- N:1 → File (nullable)

**Data Constraints:**
- Essay questions: Only `AnswerText` populated
- MCQ questions: Only `SelectedOptionId` populated
- File questions: Only `FileId` populated

---

## 🔗 Relationship Summary

### One-to-One (1:1)
- Users ↔ Students
- Users ↔ Lecturers

### One-to-Many (1:N)
- Faculty → ProgramStudy
- ProgramStudy → Students
- ProgramStudy → Lecturers
- Users → RefreshTokens
- Users → UserAssessments
- Users → Files
- Lecturer → Class
- AcademicTerm → Class
- Course → Class
- Class → Assessment
- Class → StudentEnrollment
- Assessment → Questions
- Assessment → UserAssessments
- Assessment → Files
- Question → McqOptions
- Question → Answers
- UserAssessment → Answers
- McqOption → Answers
- File → Answers

### Many-to-Many (N:N)
- Students ↔ Classes (via StudentEnrollment)

---

## 📏 Data Constraints & Business Rules

### User Constraints
- Email must be unique across all users
- Password must be hashed with Argon2id
- Role must be either 'student' or 'lecturer'

### Student Constraints
- StudentId must be unique
- Must have valid ProgramStudyId
- EnrollmentYear must be 4-digit year

### Assessment Constraints
- EndDate must be after StartDate
- Title is required

### File Constraints
- Extensions: .doc, .docx, .pdf, .jpg, .jpeg, .png only
- Size: Max 2MB (2,097,152 bytes)
- StoredFilename must be unique (use GUID)

### Answer Constraints
- Only one of (AnswerText, SelectedOptionId, FileId) should be populated
- Answer type must match Question type

### Enrollment Constraints
- Student cannot enroll in same class twice (unique constraint)

---

## 🎯 Indexes for Performance

### Essential Indexes

**users**
- `Email` (UNIQUE) - for login queries
- `Role` - for filtering by role

**students**
- `StudentId` (UNIQUE)
- `ProgramStudyId` - for joins

**assessments**
- `ClassId` - for class assessments
- `EndDate` - for deadline filtering

**user_assessments**
- `(UserId, Status)` - for incomplete/complete lists
- `Status` - for filtering

**answers**
- `(UserAssessmentId, QuestionId)` (UNIQUE) - prevent duplicate answers

**files**
- `StoredFilename` - for file retrieval

---

## 🛠️ Migration Strategy

### Current State (January 4, 2026)
✅ **Implemented:**
- Users table with Role enum

📋 **Pending:**
- All other tables (Faculty → File)

### Recommended Migration Order

1. **Phase 1: Core Entities**
   ```bash
   dotnet ef migrations add AddFacultyAndProgramStudy
   dotnet ef migrations add AddStudentsAndLecturers
   ```

2. **Phase 2: Academic Structure**
   ```bash
   dotnet ef migrations add AddAcademicTermsAndCourses
   dotnet ef migrations add AddClasses
   dotnet ef migrations add AddStudentEnrollment
   ```

3. **Phase 3: Assessment System**
   ```bash
   dotnet ef migrations add AddAssessments
   dotnet ef migrations add AddQuestionsAndOptions
   dotnet ef migrations add AddUserAssessments
   dotnet ef migrations add AddAnswers
   ```

4. **Phase 4: File System**
   ```bash
   dotnet ef migrations add AddFiles
   ```

5. **Phase 5: Authentication**
   ```bash
   dotnet ef migrations add AddRefreshTokens
   ```

### Or: Single Migration
```bash
dotnet ef migrations add AddAllEntities
```

---

## 📊 Sample Data

### Users
```
UserId | Name          | Email                      | Role
-------|---------------|----------------------------|----------
1      | Tisha Jillian | tisha.jillian@sunib.ac.id | student
2      | Jane Smith    | jane.smith@sunib.edu       | lecturer
```

### Faculty
```
FacultyId | Name
----------|-------------------
1         | Computer Science
2         | Engineering
```

### ProgramStudy
```
ProgramStudyId | FacultyId | Name
---------------|-----------|---------------------
1              | 1         | Software Engineering
2              | 1         | Information Systems
```

### Students
```
UserId | StudentId | EnrollmentYear | ProgramStudyId
-------|-----------|----------------|----------------
1      | 2025001   | 2025           | 1
```

### AcademicTerms
```
AcademicTermId | Year | Semester | StartDate  | EndDate
---------------|------|----------|------------|------------
1              | 2025 | odd      | 2025-08-01 | 2025-12-31
```

---

## 🔐 Security Considerations

### Password Storage
- Never store plaintext passwords
- Use Argon2id (implemented in PasswordHasher.cs)
- Format: `{salt}.{hash}` (Base64 encoded)

### File Security
- Store files outside wwwroot or with restricted access
- Generate unique server-side filenames (GUID)
- Validate user owns file before serving
- Track UserId for auditing

### Token Security
- Store refresh tokens hashed
- Include expiration and revocation timestamps
- Cleanup expired tokens periodically

---

## 📝 Notes for Developers

### Adding New Entity
1. Create model class in `/Models`
2. Add DbSet to `AppDbContext`
3. Configure relationships in `OnModelCreating`
4. Create migration
5. Update this document (ERD.md)

### Relationship Conventions
- Use navigation properties for related entities
- Use collections (`ICollection<T>`) for 1:N relationships
- Configure relationships with Fluent API if complex

### Enum Usage
- Use C# enums for status fields
- EF Core stores as strings by default
- Easy to extend (add new values)

---

**Database Design Philosophy:** Normalized structure for data integrity, with strategic denormalization for performance where needed (leaderboard queries, etc.).
