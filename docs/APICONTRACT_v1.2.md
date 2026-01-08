## Version: 1.2
## Base URL: 

## Authentication
- **Method:** Bearer Token
- **Header:** `Authorization: Bearer <your_access_token>`
- **Description:** All authenticated endpoints require a valid Bearer token in the `Authorization` header.

---
## Endpoints
### 1. Auth
#### 1.1. Login
**Method:** `POST`  
**Path:** `/login` 
**Description:** Login to access the private pages.

**Request Body (application/json)**
```json
{
  "email": "user@example.com",
  "password": "plainTextPassword"
}
```

**Response 200 (application/json)**
```json
{
  "user": {
    "name": "User Name",
    "email": "user@example.com",
    "role": "student"
  },
  "expiresAt": "2026-01-30T06:47:02.591Z",
  "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIs",
  "success": true
}
```

---
#### 1.2. Logout
**Method:** `POST`  
**Path:** `/logout` 
**Description:** Logout endpoint (frontend should remove access token from storage).

**Response 200 (application/json)**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

---
### 2. Leaderboard
**Method:** `GET`  
**Path:** `/leaderboard`  
**Description:** Retrieve a paginated list of students with completed assessments and total assessments they should've completed (sorted by total assessment, descending).

**Query Parameters**

| Query Name | Type                             | Example            | Default      | Notes           |
| ---------- | -------------------------------- | ------------------ | ------------ | --------------- |
| page       | number_string                    | `?page=1`          | 1            | pagination      |
| size       | number_string                    | `?size=10`         | 10           | pagination size |
| sort_by    | string [ `name` , `assessment` ] | `?sort_by=value`   | `null`       | optional sort   |
| sort_order | enum [`asc`,`desc`]              | `?sort_order=desc` | `desc`       | optional sort   |
**Student_Object**
```json
{
	"name": "Budi Sulaiman",
	"totalAssessmentsDone": 10,
	"totalAssessmentsRemaining": 20,
}
```

**Response 200 (application/json)**
```json
{
  "data": Array(Student_Object),
  "paging": {
    "page": 1,
    "size": 10,
    "totalItem": 18,
    "totalPage": 2
  },
  "success": true
}
```

---
### 3. Assessment
#### 3.1 Incomplete Assessments
##### 3.1.1 Get All Incomplete Assessments
**Method:** `GET`  
**Path:** `/assessments/incomplete`  
**Description:** Retrieve a paginated list of student's incomplete assignments.

**Query Parameter**

| Query Name | Type          | Example         | Default | Notes                       |
| ---------- | ------------- | --------------- | ------- | --------------------------- |
| page       | number_string | `?page=1`       | 1       | pagination                  |
| size       | number_string | `?size=10`      | 10      | pagination size             |
| semester   | string        | `?semester=...` | `null`  | id of the selected semester |

**Incomplete_Assessment_Object** 
```json
{
	"id": "1",
	"name": "English Test Submission",
	"class": "LC01",
	"lecturerName": "Vivi Tracia",
	"startDate": "2026-01-01T08:00:00Z",
	"endDate": "2026-01-10T23:59:59Z"
}
```

**Response 200 (application/json)**
```json
{
  "data": Array(Incomplete_Assessment_Object),
  "paging": {
    "page": 1,
    "size": 10,
    "totalItem": 18,
    "totalPage": 2
  },
  "success": true
}
```
##### 3.1.2 Get Incomplete Assessment Detail
**Method:** `GET`  
**Path:** `/assessments/incomplete/:id_assessment`  
**Description:** Retrieve a detail of student's incomplete assignment.

**Path Parameters**
- `id_assessement` (string, required): Registered User ID to be updated.

**Response 200 (application/json)**
- **State: First Time Answering**
```json
{
  "data":
  {
	"assessment": {
	    "id": 1,
	    "title": "English Test Submission",
	    "status": "draft",
	    "updatedAt": "2026-01-03T10:00:00Z"
	},
	"questions": [
	    {
	      "id": 101,
	      "question": "Explain passive voice",
	      "answerType": "essay",
	      "answer": null
	    },
	    {
	      "id": 102,
	      "question": "Choose correct sentence",
	      "answerType": "mcq",
	      "answer": null,
	      "options": [
	        {
	          "optionId": 201,
	          "label": "A",
	          "text": "The book was read by me"
	        },
	        {
	          "optionId": 202,
	          "label": "B",
	          "text": "I read the book"
	        },
	        {
	          "optionId": 203,
	          "label": "C",
	          "text": "The book is reading"
	        },
	        {
	          "optionId": 204,
	          "label": "D",
	          "text": "I am reading the book"
	        }
	      ]
	    },
	    ...
	]
  }
}
```

- **State: Draft**
```json
{
  "data":
  {
	"assessment": {
	    "id": 1,
	    "title": "English Test Submission",
	    "status": "draft",
	    "updatedAt": "2026-01-03T10:00:00Z"
	},
	"questions": [
	    {
	      "id": 101,
	      "question": "Explain passive voice",
	      "answerType": "essay",
	      "answer": {
	        "text": "Passive voice is...",
	        "mcq": null,
	        "file": null
	      }
	    },
	    {
	      "id": 102,
	      "question": "Choose correct sentence",
	      "answerType": "mcq",
	      "answer": {
	        "text": null,
	        "mcq": {
	          "optionId": 202,
	          "label": "B"
	        },
	        "file": null
	      },
	      "options": [
	        {
	          "optionId": 201,
	          "label": "A",
	          "text": "The book was read by me"
	        },
	        {
	          "optionId": 202,
	          "label": "B",
	          "text": "I read the book"
	        },
	        {
	          "optionId": 203,
	          "label": "C",
	          "text": "The book is reading"
	        },
	        {
	          "optionId": 204,
	          "label": "D",
	          "text": "I am reading the book"
	        }
	      ]
	    },
	    {
		  "id": 103,
		  "question": "Upload your essay (PDF)",
		  "answerType": "file",
		  "answer": {
		    "text": null,
		    "mcq": null,
		    "file": {
		      "fileId": 301,
		      "filename": "essay.pdf",
		      "extension": "pdf",
		      "size": 245678
		    }
		  }
	    }
	    ...
	]
  }
}
```

#### 3.1.3 Save an Incomplete Assessment (Draft)
**Method:** `PUT`  
**Path:** `/assessments/incomplete/:id_assessment`  
**Description:** Save or update a student's incomplete assessment as a draft. Supports text answers, MCQ selections, and file uploads in a single request.

**Content-Type:** `multipart/form-data`

**Path Parameters**
- `id_assessment` (string, required): The assessment ID to be saved.

**Request Fields**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `answers[i].questionId` | integer | Yes | ID of the question being answered (where `i` is the index) |
| `answers[i].answerType` | string | Yes | Type of answer: "essay", "mcq", or "file" |
| `answers[i].text` | string | Conditional | Text answer for essay questions |
| `answers[i].optionId` | integer | Conditional | Selected option ID for MCQ questions |
| `answers[i].file` | file | Conditional | File upload for file-type questions |

**Conditional Requirements:**
- If `answerType` = "essay": `text` is required
- If `answerType` = "mcq": `optionId` is required
- If `answerType` = "file": `file` is required

**File Validation (for file-type answers):**
- **Allowed extensions:** .doc, .docx, .pdf, .jpg, .jpeg, .png
- **Maximum size:** 2MB (2,097,152 bytes) per file

**Example Request (multipart/form-data)**
```javascript
answers[0].questionId: 101
answers[0].answerType: "essay"
answers[0].text: "Passive voice is when the subject receives the action rather than performing it..."

answers[1].questionId: 102
answers[1].answerType: "mcq"
answers[1].optionId: 202

answers[2].questionId: 103
answers[2].answerType: "file"
answers[2].file: [File: essay.pdf, Type: application/pdf, Size: 245678 bytes]

answers[3].questionId: 104
answers[3].answerType: "file"
answers[3].file: [File: diagram.jpg, Type: image/jpeg, Size: 512340 bytes]
```

**Response 200 (application/json) - Success**
```json
{
  "data": {
    "assessmentId": 1,
    "status": "draft",
    "updatedAt": "2026-01-03T10:30:00Z",
    "savedAnswers": 4,
    "uploadedFiles": [
      {
        "questionId": 103,
        "fileId": 301,
        "filename": "essay.pdf",
        "size": 245678
      },
      {
        "questionId": 104,
        "fileId": 302,
        "filename": "diagram.jpg",
        "size": 512340
      }
    ]
  },
  "success": true,
  "message": "Assessment saved successfully"
}
```

**Response 400 (application/json) - Invalid File Type**
```json
{
  "success": false,
  "error": {
    "code": "INVALID_FILE_TYPE",
    "message": "File type for question 103 is not allowed",
    "details": {
      "questionId": 103,
      "receivedExtension": ".txt",
      "allowedExtensions": [".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"]
    }
  }
}
```

**Response 413 (application/json) - File Too Large**
```json
{
  "success": false,
  "error": {
    "code": "FILE_TOO_LARGE",
    "message": "File size for question 103 exceeds the limit",
    "details": {
      "questionId": 103,
      "fileSize": 3145728,
      "maxSize": 2097152,
      "maxSizeReadable": "2MB"
    }
  }
}
```

**Response 400 (application/json) - Missing Required Field**
```json
{
  "success": false,
  "error": {
    "code": "MISSING_ANSWER_DATA",
    "message": "Missing required answer data for question 101",
    "details": {
      "questionId": 101,
      "answerType": "essay",
      "missingField": "text"
    }
  }
}
```

**Response 404 (application/json) - Assessment Not Found**
```json
{
  "success": false,
  "error": {
    "code": "ASSESSMENT_NOT_FOUND",
    "message": "Assessment with ID 1 not found"
  }
}
```

**Response 403 (application/json) - Access Denied**
```json
{
  "success": false,
  "error": {
    "code": "ACCESS_DENIED",
    "message": "You don't have permission to access this assessment"
  }
}
```

**Response 410 (application/json) - Assessment Expired**
```json
{
  "success": false,
  "error": {
    "code": "ASSESSMENT_EXPIRED",
    "message": "The deadline for this assessment has passed",
    "details": {
      "endDate": "2026-01-02T23:59:59Z"
    }
  }
}
```

**Notes:**
- Partial saves are allowed - students don't need to answer all questions at once
- Only include answers being saved/updated in the request
- Files are uploaded and processed in the same request
- If updating an existing file answer, the old file will be replaced
- The `updatedAt` timestamp is automatically updated on each save
- Previous draft answers not included in the request remain unchanged
#### 3.1.4 Submit an Assessment

**Method:** `POST`  
**Path:** `/assessments/incomplete/:id_assessment/submit`  
**Description:** Submit a completed assessment for review. Once submitted, the assessment cannot be edited. All required questions must be answered before submission.

**Content-Type:** `multipart/form-data`

**Path Parameters**
- `id_assessment` (string, required): The assessment ID to be submitted.

**Request Fields**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `answers[i].questionId` | integer | Yes | ID of the question being answered (where `i` is the index) |
| `answers[i].answerType` | string | Yes | Type of answer: "essay", "mcq", or "file" |
| `answers[i].text` | string | Conditional | Text answer for essay questions |
| `answers[i].optionId` | integer | Conditional | Selected option ID for MCQ questions |
| `answers[i].file` | file | Conditional | File upload for file-type questions |

**Conditional Requirements:**
- If `answerType` = "essay": `text` is required
- If `answerType` = "mcq": `optionId` is required
- If `answerType` = "file": `file` is required

**File Validation (for file-type answers):**
- **Allowed extensions:** .doc, .docx, .pdf, .jpg, .jpeg, .png
- **Maximum size:** 2MB (2,097,152 bytes) per file

**Example Request (multipart/form-data)**
```javascript
answers[0].questionId: 101
answers[0].answerType: "essay"
answers[0].text: "Passive voice is when the subject receives the action rather than performing it. The structure typically follows: [subject] + [be verb] + [past participle] + [by agent (optional)]..."

answers[1].questionId: 102
answers[1].answerType: "mcq"
answers[1].optionId: 202

answers[2].questionId: 103
answers[2].answerType: "file"
answers[2].file: [File: essay.pdf, Type: application/pdf, Size: 245678 bytes]

answers[3].questionId: 104
answers[3].answerType: "essay"
answers[3].text: "The main differences between active and passive voice are..."

answers[4].questionId: 105
answers[4].answerType: "mcq"
answers[4].optionId: 305

// ... all questions must be answered
```

**Response 200 (application/json) - Success**
```json
{
  "data": {
    "assessmentId": 1,
    "status": "on_review",
    "submittedAt": "2026-01-03T10:45:00Z",
    "totalQuestions": 15,
    "answeredQuestions": 15,
    "uploadedFiles": [
      {
        "questionId": 103,
        "fileId": 301,
        "filename": "essay.pdf",
        "size": 245678
      },
      {
        "questionId": 108,
        "fileId": 302,
        "filename": "diagram.jpg",
        "size": 512340
      }
    ]
  },
  "success": true,
  "message": "Assessment submitted successfully. Your answers are now under review."
}
```

**Response 400 (application/json) - Incomplete Answers**
```json
{
  "success": false,
  "error": {
    "code": "INCOMPLETE_ASSESSMENT",
    "message": "Please answer all questions before submitting",
    "details": {
      "totalQuestions": 15,
      "answeredQuestions": 12,
      "unansweredQuestions": [104, 107, 110]
    }
  }
}
```

**Response 400 (application/json) - Empty Answer**
```json
{
  "success": false,
  "error": {
    "code": "EMPTY_ANSWER",
    "message": "Answer for question 101 cannot be empty",
    "details": {
      "questionId": 101,
      "answerType": "essay"
    }
  }
}
```

**Response 400 (application/json) - Invalid Option ID**
```json
{
  "success": false,
  "error": {
    "code": "INVALID_OPTION",
    "message": "Invalid option selected for question 102",
    "details": {
      "questionId": 102,
      "selected_optionId": 999,
      "valid_optionIds": [201, 202, 203, 204]
    }
  }
}
```

**Response 400 (application/json) - Invalid File Type**
```json
{
  "success": false,
  "error": {
    "code": "INVALID_FILE_TYPE",
    "message": "File type for question 103 is not allowed",
    "details": {
      "questionId": 103,
      "receivedExtension": ".exe",
      "allowedExtensions": [".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"]
    }
  }
}
```

**Response 413 (application/json) - File Too Large**
```json
{
  "success": false,
  "error": {
    "code": "FILE_TOO_LARGE",
    "message": "File size for question 103 exceeds the limit",
    "details": {
      "questionId": 103,
      "fileSize": 3145728,
      "maxSize": 2097152,
      "maxSizeReadable": "2MB"
    }
  }
}
```

**Response 400 (application/json) - Answer Type Mismatch**
```json
{
  "success": false,
  "error": {
    "code": "ANSWER_TYPE_MISMATCH",
    "message": "Answer type mismatch for question 102",
    "details": {
      "questionId": 102,
      "expectedType": "mcq",
      "receivedType": "essay"
    }
  }
}
```

**Response 404 (application/json) - Assessment Not Found**
```json
{
  "success": false,
  "error": {
    "code": "ASSESSMENT_NOT_FOUND",
    "message": "Assessment with ID 1 not found"
  }
}
```

**Response 403 (application/json) - Access Denied**
```json
{
  "success": false,
  "error": {
    "code": "ACCESS_DENIED",
    "message": "You don't have permission to submit this assessment"
  }
}
```

**Response 409 (application/json) - Already Submitted**
```json
{
  "success": false,
  "error": {
    "code": "ALREADY_SUBMITTED",
    "message": "This assessment has already been submitted",
    "details": {
      "submittedAt": "2026-01-02T15:30:00Z",
      "status": "on_review"
    }
  }
}
```

**Response 410 (application/json) - Assessment Expired**
```json
{
  "success": false,
  "error": {
    "code": "ASSESSMENT_EXPIRED",
    "message": "The deadline for this assessment has passed",
    "details": {
      "endDate": "2026-01-02T23:59:59Z",
      "currentTime": "2026-01-03T10:45:00Z"
    }
  }
}
```

**Validation Rules:**
1. **Completeness:** All questions in the assessment must be answered
2. **Answer Type:** Each answer must match its question's `answerType`
3. **MCQ Validation:** The `optionId` must be a valid option for that specific question
4. **File Validation:** 
   - File extension must be in allowed list
   - File size must not exceed 2MB
5. **Essay Validation:** Text must not be empty or only whitespace
6. **Uniqueness:** The assessment must not have been previously submitted
7. **Deadline:** Submission must be within the assessment's date range (`startDate` to `endDate`)

**Workflow:**
1. Student completes all answers (can save drafts using the save endpoint)
2. Student reviews all answers
3. Student clicks "Submit" button
4. Frontend validates all questions are answered
5. Frontend sends all answers (including files) in single multipart request
6. Backend validates:
   - All questions answered
   - All answer types match
   - All files meet requirements
   - Assessment not already submitted
   - Within deadline
7. Backend saves all answers and uploads files in a transaction
8. Assessment status changes from "draft" to "on_review"
9. Assessment moves from "Incomplete" to "Completed" tab
10. Student receives confirmation and can view (but not edit) submission

**Notes:**
- **This is a final submission** - answers cannot be modified after successful submission
- All answers and files must be included in a single request
- The endpoint processes everything atomically - if any validation fails, nothing is saved
- Files are uploaded and associated with the submission in the same transaction
- Once submitted, the assessment is immediately available in the "Completed Assessments" section
- The lecturer can then review and grade the submission
- Students can still view their submitted answers but cannot edit them

---
#### 3.2 Completed Assessments
##### 3.2.1 Get All Completed Assessments
**Method:** `GET`  
**Path:** `/assessments`  
**Description:** Retrieve a paginated list of student's completed assignments.

**Query Parameter**

| Query Name | Type          | Example         | Default | Notes                       |
| ---------- | ------------- | --------------- | ------- | --------------------------- |
| page       | number_string | `?page=1`       | 1       | pagination                  |
| size       | number_string | `?size=10`      | 10      | pagination size             |
| semester   | string        | `?semester=...` | `null`  | id of the selected semester |

**Completed_Assessment_Object** 
```json
{
	"id": "1",
	"name": "English Test Submission",
	"class": "LC01",
	"lecturerName": "Vivi Tracia",
	"submittedAt": "2026-01-10T23:59:59Z"
}
```

**Response 200 (application/json)**
```json
{
  "data": Array(Completed_Assessment_Object),
  "paging": {
    "page": 1,
    "size": 10,
    "totalItem": 18,
    "totalPage": 2
  },
  "success": true
}
```
##### 3.2.2 Get Completed Assessment Detail
**Method:** `GET`  
**Path:** `/assessments/:id_assessment`  
**Description:** Retrieve a detail of student's complete assignment.

**Path Parameters**
- `id_assessement` (string, required): Registered User ID to be updated.

**Response 200 (application/json)**
```json
{
  "data":
  {
	"assessment": {
	    "id": 1,
	    "title": "English Test Submission",
	    "status": "completed",
	    "score": 85,
	    "updatedAt": "2026-01-03T10:00:00Z"
	},
	"questions": [
	    {
	      "id": 101,
	      "question": "Explain passive voice",
	      "answerType": "essay",
	      "answer": {
	        "text": "Passive voice is...",
	        "mcq": null,
	        "file": null
	      }
	    },
	    {
	      "id": 102,
	      "question": "Choose correct sentence",
	      "answerType": "mcq",
	      "answer": {
	        "text": null,
	        "mcq": {
	          "optionId": 202,
	          "label": "B"
	        },
	        "file": null
	      },
	      "options": [
	        {
	          "optionId": 201,
	          "label": "A",
	          "text": "The book was read by me"
	        },
	        {
	          "optionId": 202,
	          "label": "B",
	          "text": "I read the book"
	        },
	        {
	          "optionId": 203,
	          "label": "C",
	          "text": "The book is reading"
	        },
	        {
	          "optionId": 204,
	          "label": "D",
	          "text": "I am reading the book"
	        }
	      ]
	    },
	    {
		  "id": 103,
		  "question": "Upload your essay (PDF)",
		  "answerType": "file",
		  "answer": {
		    "text": null,
		    "mcq": null,
		    "file": {
		      "fileId": 301,
		      "filename": "essay.pdf",
		      "extension": "pdf",
		      "size": 245678
		    }
		  }
	    }
	    ...
	]
  }
}
```

---
## Common Response Codes
- **200**: Success
- **201**: Created
- **400**: Invalid or missing fields
- **401**: Unauthorized
- **404**: Not Found
- **422**: Data conflict
- **500**: Server error
