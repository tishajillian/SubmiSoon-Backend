# Postman Test Data Reference Guide

**Last Updated:** January 8, 2026  
**Purpose:** Reference guide for valid IDs and test data to use in Postman collection tests

---

## 📌 Base Configuration

- **Base URL:** `http://localhost:5002`
- **Database:** Auto-seeded on first run (see `DbSeeder.cs`)

---

## 👥 Test Users

### Students

| Name | Email | Password | User ID | Student ID | Enrollment Year | Program |
|------|-------|----------|---------|------------|-----------------|---------|
| Tisha Jillian | tisha.jillian@sunib.ac.id | password2k25 | 3 | 20230001 | 2023 | Software Engineering |
| Alice Johnson | alice.johnson@sunib.ac.id | password123 | 4 | 20240002 | 2024 | Information Systems |
| Benjamin Chen | benjamin.chen@sunib.ac.id | password123 | 5 | 20250003 | 2025 | Computer Engineering |

**Pattern:** User IDs start at 3 (after 2 lecturers), Student IDs format: `{EnrollmentYear}{SequentialNumber:D4}`

### Lecturers

| Name | Email | Password | User ID | Lecturer ID | Program |
|------|-------|----------|---------|-------------|---------|
| Dr. Jane Smith | jane.smith@sunib.edu | password123 | 1 | LEC001 | Software Engineering |
| Prof. Bob Williams | bob@sunib.ac.id | password123 | 2 | LEC002 | Information Systems |

---

## 📚 Classes & Courses

| Class ID | Class Code | Course Code | Course Name | Lecturer | Academic Term |
|----------|------------|-------------|-------------|----------|---------------|
| 1 | CS101-A | CS101 | Introduction to Programming | Dr. Jane Smith | 2025-odd |
| 2 | CS201-B | CS201 | Data Structures and Algorithms | Dr. Jane Smith | 2025-odd |
| 3 | CS301-C | CS301 | Database Systems | Prof. Bob Williams | 2025-odd |

---

## 📝 Assessments by Status

### ✅ Expired Assessments (Past Deadline)

| Assessment ID | Class | Title | Start Date | End Date | Status |
|---------------|-------|-------|------------|----------|--------|
| 1 | CS101-A | Week 1 Quiz: Programming Basics | Now - 30 days | Now - 23 days | Expired |
| 2 | CS101-A | Assignment 1: Hello World Program | Now - 25 days | Now - 18 days | Expired |
| 3 | CS101-A | Week 3 Quiz: Variables and Data Types | Now - 20 days | Now - 13 days | Expired |
| 6 | CS201-B | Quiz 1: Arrays and Linked Lists | Now - 28 days | Now - 21 days | Expired |
| 7 | CS201-B | Assignment 1: Implement Stack and Queue | Now - 22 days | Now - 15 days | Expired |
| 8 | CS201-B | Quiz 2: Trees and Graphs | Now - 14 days | Now - 7 days | Expired |
| 11 | CS301-C | Quiz 1: SQL Fundamentals | Now - 27 days | Now - 20 days | Expired |
| 12 | CS301-C | Assignment 1: Database Design | Now - 19 days | Now - 12 days | Expired |
| 13 | CS301-C | Quiz 2: Normalization and Indexing | Now - 11 days | Now - 4 days | Expired |

**Use Case:** Testing `ASSESSMENT_EXPIRED` error responses

---

### ⏳ Active Assessments (Ongoing - Can Submit)

| Assessment ID | Class | Title | Start Date | End Date | Status |
|---------------|-------|-------|------------|----------|--------|
| 4 | CS101-A | Midterm Project: Calculator Application | Now - 10 days | **Now + 5 days** | Active |
| 9 | CS201-B | Midterm Exam: Sorting and Searching | Now - 5 days | **Now + 2 days** | Active |
| 14 | CS301-C | Midterm Exam: Advanced SQL Queries | Now - 3 days | **Now + 4 days** | Active |

**Use Case:** Testing save draft and submit functionality  
**Recommended for Postman:** Use Assessment ID **4**, **9**, or **14**

---

### 🚀 Upcoming Assessments (Not Yet Started)

| Assessment ID | Class | Title | Start Date | End Date | Status |
|---------------|-------|-------|------------|----------|--------|
| 5 | CS101-A | Final Project: Student Management System | **Now + 3 days** | Now + 21 days | Not Started |
| 10 | CS201-B | Final Project: Algorithm Visualization | **Now + 1 day** | Now + 18 days | Not Started |
| 15 | CS301-C | Final Project: E-Commerce Database | **Now + 2 days** | Now + 20 days | Not Started |

**Use Case:** Testing `ASSESSMENT_NOT_STARTED` or access denied scenarios

---

## 📋 Questions by Assessment

### Assessment 4: Midterm Project - Calculator Application (ACTIVE - Best for Testing)

| Question ID | Type | Content |
|-------------|------|---------|
| 14 | file | Upload your complete Calculator source code (.cs files) |
| 15 | file | Upload test cases demonstrating all calculator operations |
| 16 | file | Upload a demo video of your calculator application |
| 17 | essay | Describe the architecture of your calculator and explain your design decisions. |
| 18 | essay | What challenges did you face and how did you overcome them? |

**Total Questions:** 5 (3 file + 2 essay)

---

### Assessment 9: Midterm Exam - Sorting and Searching (ACTIVE)

| Question ID | Type | Content |
|-------------|------|---------|
| 38 | mcq | What is the time complexity of Bubble Sort in the worst case? |
| 39 | mcq | Which sorting algorithm uses divide and conquer? |
| 40 | mcq | What is the time complexity of binary search? |
| 41 | mcq | Which sorting algorithm is most efficient for nearly sorted arrays? |
| 42 | mcq | What does 'stable sort' mean? |
| 43 | mcq | Which search algorithm works on unsorted arrays? |
| 44 | essay | Explain Quick Sort algorithm with an example. |
| 45 | essay | Compare the performance of different sorting algorithms. |

**Total Questions:** 8 (6 MCQ + 2 essay)

---

## 🎯 MCQ Options Reference

### Question 38 (Bubble Sort Time Complexity)
| Option ID | Label | Text | Is Correct |
|-----------|-------|------|------------|
| 149 | A | O(n) | ❌ |
| 150 | B | O(n log n) | ❌ |
| 151 | C | O(n²) | ✅ |
| 152 | D | O(log n) | ❌ |

### Question 39 (Divide and Conquer Sorting)
| Option ID | Label | Text | Is Correct |
|-----------|-------|------|------------|
| 153 | A | Bubble Sort | ❌ |
| 154 | B | Merge Sort | ✅ |
| 155 | C | Selection Sort | ❌ |
| 156 | D | Insertion Sort | ❌ |

### Question 40 (Binary Search Complexity)
| Option ID | Label | Text | Is Correct |
|-----------|-------|------|------------|
| 157 | A | O(1) | ❌ |
| 158 | B | O(log n) | ✅ |
| 159 | C | O(n) | ❌ |
| 160 | D | O(n²) | ❌ |

**Pattern:** Each MCQ question has 4 options with sequential IDs starting from (Question Index × 4) + 1

---

## 🧪 Recommended Test Scenarios

### 1. Save Draft (PUT /assessments/incomplete/{id})

**Valid Request - MCQ Answer:**
```
Assessment ID: 9
Question ID: 38
Answer Type: mcq
Option ID: 151
Expected Result: 200 OK - Draft saved
```

**Valid Request - Essay Answer:**
```
Assessment ID: 9
Question ID: 44
Answer Type: essay
Text: "Quick Sort is a divide-and-conquer algorithm..."
Expected Result: 200 OK - Draft saved
```

**Valid Request - File Upload:**
```
Assessment ID: 4
Question ID: 14
Answer Type: file
File: calculator.zip (< 2MB)
Expected Result: 200 OK - Draft saved with file URL
```

---

### 2. Submit Assessment (POST /assessments/incomplete/{id}/submit)

**Valid Complete Submission (Assessment 4):**
```
Assessment ID: 4

answers[0].question_id = 14
answers[0].answer_type = file
answers[0].file = <upload calculator.cs>

answers[1].question_id = 15
answers[1].answer_type = file
answers[1].file = <upload testcases.pdf>

answers[2].question_id = 16
answers[2].answer_type = file
answers[2].file = <upload demo.mp4>

answers[3].question_id = 17
answers[3].answer_type = essay
answers[3].text = "My calculator uses MVC architecture..."

answers[4].question_id = 18
answers[4].answer_type = essay
answers[4].text = "The main challenge was..."

Expected Result: 200 OK - Assessment submitted for review
```

**Incomplete Submission Error:**
```
Assessment ID: 4
Only answer questions 14, 15 (missing 16, 17, 18)

Expected Result: 400 Bad Request
Error Code: INCOMPLETE_ASSESSMENT
Details: { totalQuestions: 5, answeredQuestions: 2, unansweredQuestions: [16, 17, 18] }
```

---

### 3. Expired Assessment (PUT /assessments/incomplete/1)

**Request:**
```
Assessment ID: 1 (Week 1 Quiz - expired 23 days ago)
Question ID: 1
Answer Type: mcq
Option ID: 1

Expected Result: 410 Gone
Error Code: ASSESSMENT_EXPIRED
Error Message: "The deadline for this assessment has passed"
Details: { endDate: "2025-12-15T23:39:10" } (example timestamp)
```

---

### 4. Access Denied - Different Student

**Setup:**
1. Login as Tisha (tisha.jillian@sunib.ac.id)
2. Get assessment details for Assessment ID: 9 (CS201-B)
3. Verify Tisha is NOT enrolled in CS201-B class

**Expected Result:**
```
Status: 403 Forbidden
Error Code: ACCESS_DENIED
Error Message: "You do not have permission to access this assessment"
```

---

### 5. Invalid File Type

**Request:**
```
Assessment ID: 4
Question ID: 14
Answer Type: file
File: malware.exe (executable file)

Expected Result: 400 Bad Request
Error Code: INVALID_FILE_TYPE
Details: { 
  questionId: 14, 
  receivedExtension: ".exe", 
  allowedExtensions: [".pdf", ".doc", ".docx", ".zip", ".jpg", ".png"] 
}
```

---

### 6. File Too Large

**Request:**
```
Assessment ID: 4
Question ID: 14
Answer Type: file
File: large-project.zip (3MB file)

Expected Result: 413 Payload Too Large
Error Code: FILE_TOO_LARGE
Details: { 
  questionId: 14, 
  fileSize: 3145728, 
  maxSize: 2097152 
}
```

---

## 📅 Date Calculations

All assessment dates are relative to `DateTime.Now` at seed time:

- **Expired:** EndDate < Now
- **Active:** StartDate <= Now AND EndDate >= Now  
- **Upcoming:** StartDate > Now

**Important:** Dates will shift based on when you last reset the database. To get fresh dates, delete the database and restart the application to re-seed.

---

## 🔄 Resetting Test Data

**Full Database Reset:**
```bash
# Stop the application
# Delete the database file or run:
dotnet ef database drop --project SubmiSoonProject
dotnet run --project SubmiSoonProject
# Database will auto-seed on startup
```

**Verify Seed Data:**
```sql
SELECT COUNT(*) FROM Users;          -- Should return 73 (2 lecturers + 71 students)
SELECT COUNT(*) FROM Assessments;    -- Should return 15
SELECT COUNT(*) FROM Questions;      -- Should return 73
SELECT COUNT(*) FROM McqOptions;     -- Should return 164 (41 MCQ × 4 options)
```

---

## 🎯 Quick Reference for Postman Variables

Update these in your Postman collection variables:

```json
{
  "baseUrl": "http://localhost:5002",
  "studentEmail": "tisha.jillian@sunib.ac.id",
  "studentPassword": "password2k25",
  "aliceEmail": "alice.johnson@sunib.ac.id",
  "alicePassword": "password123",
  "assessmentId": "4",  // Use 4, 9, or 14 for active assessments
  "expiredAssessmentId": "1",  // For testing expired scenarios
  "questionIdMcq": "38",
  "questionIdEssay": "44",
  "questionIdFile": "14",
  "correctOptionId": "151"  // For question 38
}
```

---

## ⚠️ Important Notes

1. **User IDs vs Student IDs:** User.UserId (3, 4, 5...) ≠ Student.StudentId ("20230001", "20240002"...)
2. **Enrollment:** Not all students are enrolled in all classes - check `StudentEnrollment` table
3. **Dates Are Relative:** All dates use `DateTime.Now` as reference, so they shift with each seed
4. **File Limits:** 
   - Per file: 2MB max
   - Total request: 10MB max
   - Allowed extensions: .pdf, .doc, .docx, .zip, .jpg, .png, .jpeg, .gif
5. **MCQ Options:** Option IDs are auto-generated sequentially - use the correct option ID from this guide

---

## 📞 Need Help?

- Check seeded data: Review `SubmiSoonProject/Data/DbSeeder.cs`
- API documentation: See `docs/APICONTRACT_v1.3.md`
- Test scenarios: See `docs/TESTING.md`

**Last Database Seed:** Check console output when application starts
