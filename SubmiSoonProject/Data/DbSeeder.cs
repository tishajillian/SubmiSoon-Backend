using SubmiSoonProject.Models;
using SubmiSoonProject.Helpers;

namespace SubmiSoonProject.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Check if already seeded
            if (context.Users.Any())
            {
                Console.WriteLine("Database already has data. Skipping seed.");
                return;
            }

            Console.WriteLine("Seeding database...");

            // 1. Seed Users (Lecturers + 70+ Students)
            var usersList = new List<User>
            {
                // Lecturers
                new User
                {
                    Name = "Dr. Jane Smith",
                    Email = "jane.smith@sunib.edu",
                    PasswordHash = PasswordHasher.HashPassword("password123"),
                    Role = UserRole.lecturer
                },
                new User
                {
                    Name = "Prof. Bob Williams",
                    Email = "bob@sunib.ac.id",
                    PasswordHash = PasswordHasher.HashPassword("password123"),
                    Role = UserRole.lecturer
                },
                // Original Student
                new User
                {
                    Name = "Tisha Jillian",
                    Email = "tisha.jillian@sunib.ac.id",
                    PasswordHash = PasswordHasher.HashPassword("password2k25"),
                    Role = UserRole.student
                }
            };

            // Add 70+ students with diverse names
            var studentNames = new[]
            {
                "Alice Johnson", "Benjamin Chen", "Charlotte Davis", "Daniel Kim", "Emma Martinez",
                "Felix Anderson", "Grace Lee", "Henry Taylor", "Isabella Brown", "Jack Wilson",
                "Katherine Moore", "Liam Thompson", "Mia Garcia", "Noah Rodriguez", "Olivia White",
                "Patrick Harris", "Quinn Clark", "Rachel Lewis", "Samuel Walker", "Taylor Hall",
                "Uma Patel", "Victor Allen", "Wendy Young", "Xavier King", "Yuki Tanaka",
                "Zachary Wright", "Aria Lopez", "Brandon Hill", "Chloe Scott", "David Green",
                "Emily Adams", "Frank Baker", "Georgia Nelson", "Hugo Carter", "Iris Mitchell",
                "James Perez", "Kelly Roberts", "Lucas Turner", "Maya Phillips", "Nathan Campbell",
                "Oscar Parker", "Penelope Evans", "Quincy Edwards", "Rose Collins", "Steven Stewart",
                "Tara Morris", "Ulysses Rogers", "Violet Reed", "William Cook", "Xena Morgan",
                "Yale Bell", "Zara Murphy", "Adrian Bailey", "Bella Rivera", "Carlos Cooper",
                "Diana Richardson", "Ethan Cox", "Fiona Howard", "George Ward", "Hannah Torres",
                "Ivan Peterson", "Julia Gray", "Kevin Ramirez", "Luna James", "Marcus Watson",
                "Nina Brooks", "Owen Kelly", "Paige Sanders", "Quinton Price", "Ruby Bennett",
                "Sebastian Wood", "Tiffany Barnes", "Ulrich Ross", "Vanessa Henderson"
            };

            for (int i = 0; i < studentNames.Length; i++)
            {
                usersList.Add(new User
                {
                    Name = studentNames[i],
                    Email = $"{studentNames[i].ToLower().Replace(" ", ".")}@sunib.ac.id",
                    PasswordHash = PasswordHasher.HashPassword("password123"),
                    Role = UserRole.student
                });
            }

            var users = usersList.ToArray();
            context.Users.AddRange(users);
            context.SaveChanges();
            Console.WriteLine($"Seeded {users.Length} users successfully!");

            // 2. Seed Faculties
            var faculties = new[]
            {
                new Faculty { Name = "Faculty of Computer Science" },
                new Faculty { Name = "Faculty of Engineering" }
            };
            context.Faculties.AddRange(faculties);
            context.SaveChanges();
            Console.WriteLine($"Seeded {faculties.Length} faculties successfully!");

            // 3. Seed Program Studies
            var programStudies = new[]
            {
                new ProgramStudy { FacultyId = faculties[0].FacultyId, Name = "Software Engineering" },
                new ProgramStudy { FacultyId = faculties[0].FacultyId, Name = "Information Systems" },
                new ProgramStudy { FacultyId = faculties[1].FacultyId, Name = "Computer Engineering" }
            };
            context.ProgramStudies.AddRange(programStudies);
            context.SaveChanges();
            Console.WriteLine($"Seeded {programStudies.Length} program studies successfully!");

            // 4. Seed Students (All student users become Student entities)
            var studentsList = new List<Student>();
            var studentUsers = users.Where(u => u.Role == UserRole.student).ToArray();
            
            for (int i = 0; i < studentUsers.Length; i++)
            {
                // Distribute enrollment years (mostly 2023-2025)
                int enrollmentYear = 2023 + (i % 3);
                
                // Distribute across program studies
                int programIndex = i % programStudies.Length;
                
                studentsList.Add(new Student
                {
                    UserId = studentUsers[i].UserId,
                    StudentId = $"{enrollmentYear}{(i + 1):D4}",
                    EnrollmentYear = enrollmentYear,
                    ProgramStudyId = programStudies[programIndex].ProgramStudyId
                });
            }

            var students = studentsList.ToArray();
            context.Students.AddRange(students);
            context.SaveChanges();
            Console.WriteLine($"Seeded {students.Length} students successfully!");

            // 5. Seed Lecturers
            var lecturerUsers = users.Where(u => u.Role == UserRole.lecturer).ToArray();
            var lecturers = new[]
            {
                new Lecturer 
                { 
                    UserId = lecturerUsers[0].UserId, 
                    LecturerId = "LEC001", 
                    ProgramStudyId = programStudies[0].ProgramStudyId 
                },
                new Lecturer 
                { 
                    UserId = lecturerUsers[1].UserId, 
                    LecturerId = "LEC002", 
                    ProgramStudyId = programStudies[1].ProgramStudyId 
                }
            };
            context.Lecturers.AddRange(lecturers);
            context.SaveChanges();
            Console.WriteLine($"Seeded {lecturers.Length} lecturers successfully!");

            // 6. Seed Academic Terms
            var academicTerms = new[]
            {
                new AcademicTerm 
                { 
                    Year = 2025, 
                    Semester = SemesterType.odd, 
                    StartDate = new DateTime(2025, 1, 1), 
                    EndDate = new DateTime(2025, 6, 30) 
                },
                new AcademicTerm 
                { 
                    Year = 2025, 
                    Semester = SemesterType.even, 
                    StartDate = new DateTime(2025, 7, 1), 
                    EndDate = new DateTime(2025, 12, 31) 
                }
            };
            context.AcademicTerms.AddRange(academicTerms);
            context.SaveChanges();
            Console.WriteLine($"Seeded {academicTerms.Length} academic terms successfully!");

            // 7. Seed Courses
            var courses = new[]
            {
                new Course { CourseCode = "CS101", Name = "Introduction to Programming" },
                new Course { CourseCode = "CS201", Name = "Data Structures and Algorithms" },
                new Course { CourseCode = "CS301", Name = "Database Systems" }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();
            Console.WriteLine($"Seeded {courses.Length} courses successfully!");

            // 8. Seed Classes
            var classes = new[]
            {
                new Class 
                { 
                    ClassCode = "CS101-A", 
                    CourseId = courses[0].CourseId, 
                    LecturerId = lecturers[0].UserId, 
                    AcademicTermId = academicTerms[0].AcademicTermId 
                },
                new Class 
                { 
                    ClassCode = "CS201-B", 
                    CourseId = courses[1].CourseId, 
                    LecturerId = lecturers[0].UserId, 
                    AcademicTermId = academicTerms[0].AcademicTermId 
                },
                new Class 
                { 
                    ClassCode = "CS301-C", 
                    CourseId = courses[2].CourseId, 
                    LecturerId = lecturers[1].UserId, 
                    AcademicTermId = academicTerms[0].AcademicTermId 
                }
            };
            context.Classes.AddRange(classes);
            context.SaveChanges();
            Console.WriteLine($"Seeded {classes.Length} classes successfully!");

            // 9. Seed Student Enrollments (Each student enrolls in 2-4 classes)
            var enrollmentsList = new List<StudentEnrollment>();
            var random = new Random(42); // Fixed seed for reproducibility

            foreach (var student in students)
            {
                // Each student enrolls in 2-4 classes
                int numClasses = random.Next(2, 5);
                var enrolledClasses = classes.OrderBy(x => random.Next()).Take(numClasses);

                foreach (var cls in enrolledClasses)
                {
                    enrollmentsList.Add(new StudentEnrollment
                    {
                        StudentId = student.UserId,
                        ClassId = cls.ClassId,
                        EnrolledAt = DateTime.Now.AddDays(-random.Next(30, 90)),
                        Status = EnrollmentStatus.active
                    });
                }
            }

            var enrollments = enrollmentsList.ToArray();
            context.StudentEnrollments.AddRange(enrollments);
            context.SaveChanges();
            Console.WriteLine($"Seeded {enrollments.Length} student enrollments successfully!");

            // 10. Seed Assessments (12-15 assessments across all classes)
            var assessmentsList = new List<Assessment>
            {
                // CS101-A: Introduction to Programming (5 assessments)
                new Assessment 
                { 
                    ClassId = classes[0].ClassId, 
                    Title = "Week 1 Quiz: Programming Basics", 
                    StartDate = DateTime.Now.AddDays(-30), 
                    EndDate = DateTime.Now.AddDays(-23), 
                    CreatedAt = DateTime.Now.AddDays(-31) 
                },
                new Assessment 
                { 
                    ClassId = classes[0].ClassId, 
                    Title = "Assignment 1: Hello World Program", 
                    StartDate = DateTime.Now.AddDays(-25), 
                    EndDate = DateTime.Now.AddDays(-18), 
                    CreatedAt = DateTime.Now.AddDays(-26) 
                },
                new Assessment 
                { 
                    ClassId = classes[0].ClassId, 
                    Title = "Week 3 Quiz: Variables and Data Types", 
                    StartDate = DateTime.Now.AddDays(-20), 
                    EndDate = DateTime.Now.AddDays(-13), 
                    CreatedAt = DateTime.Now.AddDays(-21) 
                },
                new Assessment 
                { 
                    ClassId = classes[0].ClassId, 
                    Title = "Midterm Project: Calculator Application", 
                    StartDate = DateTime.Now.AddDays(-10), 
                    EndDate = DateTime.Now.AddDays(5), 
                    CreatedAt = DateTime.Now.AddDays(-12) 
                },
                new Assessment 
                { 
                    ClassId = classes[0].ClassId, 
                    Title = "Final Project: Student Management System", 
                    StartDate = DateTime.Now.AddDays(3), 
                    EndDate = DateTime.Now.AddDays(21), 
                    CreatedAt = DateTime.Now.AddDays(-5) 
                },

                // CS201-B: Data Structures and Algorithms (5 assessments)
                new Assessment 
                { 
                    ClassId = classes[1].ClassId, 
                    Title = "Quiz 1: Arrays and Linked Lists", 
                    StartDate = DateTime.Now.AddDays(-28), 
                    EndDate = DateTime.Now.AddDays(-21), 
                    CreatedAt = DateTime.Now.AddDays(-29) 
                },
                new Assessment 
                { 
                    ClassId = classes[1].ClassId, 
                    Title = "Assignment 1: Implement Stack and Queue", 
                    StartDate = DateTime.Now.AddDays(-22), 
                    EndDate = DateTime.Now.AddDays(-15), 
                    CreatedAt = DateTime.Now.AddDays(-23) 
                },
                new Assessment 
                { 
                    ClassId = classes[1].ClassId, 
                    Title = "Quiz 2: Trees and Graphs", 
                    StartDate = DateTime.Now.AddDays(-14), 
                    EndDate = DateTime.Now.AddDays(-7), 
                    CreatedAt = DateTime.Now.AddDays(-15) 
                },
                new Assessment 
                { 
                    ClassId = classes[1].ClassId, 
                    Title = "Midterm Exam: Sorting and Searching", 
                    StartDate = DateTime.Now.AddDays(-5), 
                    EndDate = DateTime.Now.AddDays(2), 
                    CreatedAt = DateTime.Now.AddDays(-8) 
                },
                new Assessment 
                { 
                    ClassId = classes[1].ClassId, 
                    Title = "Final Project: Algorithm Visualization", 
                    StartDate = DateTime.Now.AddDays(1), 
                    EndDate = DateTime.Now.AddDays(18), 
                    CreatedAt = DateTime.Now.AddDays(-3) 
                },

                // CS301-C: Database Systems (5 assessments)
                new Assessment 
                { 
                    ClassId = classes[2].ClassId, 
                    Title = "Quiz 1: SQL Fundamentals", 
                    StartDate = DateTime.Now.AddDays(-27), 
                    EndDate = DateTime.Now.AddDays(-20), 
                    CreatedAt = DateTime.Now.AddDays(-28) 
                },
                new Assessment 
                { 
                    ClassId = classes[2].ClassId, 
                    Title = "Assignment 1: Database Design", 
                    StartDate = DateTime.Now.AddDays(-19), 
                    EndDate = DateTime.Now.AddDays(-12), 
                    CreatedAt = DateTime.Now.AddDays(-20) 
                },
                new Assessment 
                { 
                    ClassId = classes[2].ClassId, 
                    Title = "Quiz 2: Normalization and Indexing", 
                    StartDate = DateTime.Now.AddDays(-11), 
                    EndDate = DateTime.Now.AddDays(-4), 
                    CreatedAt = DateTime.Now.AddDays(-12) 
                },
                new Assessment 
                { 
                    ClassId = classes[2].ClassId, 
                    Title = "Midterm Exam: Advanced SQL Queries", 
                    StartDate = DateTime.Now.AddDays(-3), 
                    EndDate = DateTime.Now.AddDays(4), 
                    CreatedAt = DateTime.Now.AddDays(-6) 
                },
                new Assessment 
                { 
                    ClassId = classes[2].ClassId, 
                    Title = "Final Project: E-Commerce Database", 
                    StartDate = DateTime.Now.AddDays(2), 
                    EndDate = DateTime.Now.AddDays(20), 
                    CreatedAt = DateTime.Now.AddDays(-2) 
                }
            };

            var assessments = assessmentsList.ToArray();
            context.Assessments.AddRange(assessments);
            context.SaveChanges();
            Console.WriteLine($"Seeded {assessments.Length} assessments successfully!");

            // 11. Seed Questions (60-80 questions across all assessments)
            var questionsList = new List<Question>();

            // CS101 Assessment 1: Week 1 Quiz (4 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[0].AssessmentId, Type = QuestionType.mcq, Content = "What does CPU stand for?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[0].AssessmentId, Type = QuestionType.mcq, Content = "Which of the following is a programming language?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[0].AssessmentId, Type = QuestionType.mcq, Content = "What is the purpose of a compiler?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[0].AssessmentId, Type = QuestionType.mcq, Content = "Which symbol is used for comments in C#?", CreatedAt = DateTime.Now }
            });

            // CS101 Assessment 2: Assignment 1 (2 file + 1 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[1].AssessmentId, Type = QuestionType.file, Content = "Upload your HelloWorld.cs program file", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[1].AssessmentId, Type = QuestionType.file, Content = "Upload a screenshot showing your program output", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[1].AssessmentId, Type = QuestionType.essay, Content = "Explain in your own words what your program does and how it works.", CreatedAt = DateTime.Now }
            });

            // CS101 Assessment 3: Week 3 Quiz (5 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[2].AssessmentId, Type = QuestionType.mcq, Content = "Which of the following is NOT a primitive data type in C#?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[2].AssessmentId, Type = QuestionType.mcq, Content = "What is the default value of an integer variable in C#?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[2].AssessmentId, Type = QuestionType.mcq, Content = "Which keyword is used to declare a constant in C#?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[2].AssessmentId, Type = QuestionType.mcq, Content = "What is type casting?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[2].AssessmentId, Type = QuestionType.mcq, Content = "Which operator is used for string concatenation?", CreatedAt = DateTime.Now }
            });

            // CS101 Assessment 4: Midterm Project (3 file + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[3].AssessmentId, Type = QuestionType.file, Content = "Upload your complete Calculator source code (.cs files)", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[3].AssessmentId, Type = QuestionType.file, Content = "Upload test cases demonstrating all calculator operations", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[3].AssessmentId, Type = QuestionType.file, Content = "Upload a demo video of your calculator application", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[3].AssessmentId, Type = QuestionType.essay, Content = "Describe the architecture of your calculator and explain your design decisions.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[3].AssessmentId, Type = QuestionType.essay, Content = "What challenges did you face and how did you overcome them?", CreatedAt = DateTime.Now }
            });

            // CS101 Assessment 5: Final Project (3 file + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[4].AssessmentId, Type = QuestionType.file, Content = "Upload your complete Student Management System project files (zip)", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[4].AssessmentId, Type = QuestionType.file, Content = "Upload user documentation (PDF)", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[4].AssessmentId, Type = QuestionType.file, Content = "Upload demo video showcasing all features", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[4].AssessmentId, Type = QuestionType.essay, Content = "Explain the database schema and relationships in your system.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[4].AssessmentId, Type = QuestionType.essay, Content = "Reflect on what you learned throughout this course.", CreatedAt = DateTime.Now }
            });

            // CS201 Assessment 1: Arrays and Linked Lists Quiz (5 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[5].AssessmentId, Type = QuestionType.mcq, Content = "What is the time complexity of accessing an element in an array by index?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[5].AssessmentId, Type = QuestionType.mcq, Content = "Which operation is faster in a linked list compared to an array?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[5].AssessmentId, Type = QuestionType.mcq, Content = "What is a doubly linked list?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[5].AssessmentId, Type = QuestionType.mcq, Content = "What is the space complexity of an array of size n?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[5].AssessmentId, Type = QuestionType.mcq, Content = "Which data structure uses LIFO order?", CreatedAt = DateTime.Now }
            });

            // CS201 Assessment 2: Stack and Queue Assignment (2 file + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[6].AssessmentId, Type = QuestionType.file, Content = "Upload your Stack implementation with all methods", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[6].AssessmentId, Type = QuestionType.file, Content = "Upload your Queue implementation with all methods", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[6].AssessmentId, Type = QuestionType.essay, Content = "Compare and contrast Stack and Queue data structures.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[6].AssessmentId, Type = QuestionType.essay, Content = "Provide real-world examples where Stack and Queue are used.", CreatedAt = DateTime.Now }
            });

            // CS201 Assessment 3: Trees and Graphs Quiz (5 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[7].AssessmentId, Type = QuestionType.mcq, Content = "What is the maximum number of children a binary tree node can have?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[7].AssessmentId, Type = QuestionType.mcq, Content = "Which tree traversal visits the root node first?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[7].AssessmentId, Type = QuestionType.mcq, Content = "What is the time complexity of searching in a balanced BST?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[7].AssessmentId, Type = QuestionType.mcq, Content = "Which algorithm is used for finding shortest path in graphs?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[7].AssessmentId, Type = QuestionType.mcq, Content = "What is a spanning tree?", CreatedAt = DateTime.Now }
            });

            // CS201 Assessment 4: Midterm Exam (6 MCQ + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "What is the time complexity of Bubble Sort in the worst case?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "Which sorting algorithm uses divide and conquer?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "What is the time complexity of binary search?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "Which sorting algorithm is most efficient for nearly sorted arrays?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "What does 'stable sort' mean?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.mcq, Content = "Which search algorithm works on unsorted arrays?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.essay, Content = "Explain Quick Sort algorithm with an example.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[8].AssessmentId, Type = QuestionType.essay, Content = "Compare the performance of different sorting algorithms.", CreatedAt = DateTime.Now }
            });

            // CS201 Assessment 5: Final Project (3 file + 1 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[9].AssessmentId, Type = QuestionType.file, Content = "Upload your algorithm visualization project source code", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[9].AssessmentId, Type = QuestionType.file, Content = "Upload screenshots of different algorithm visualizations", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[9].AssessmentId, Type = QuestionType.file, Content = "Upload project documentation and user guide", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[9].AssessmentId, Type = QuestionType.essay, Content = "Explain the algorithms you visualized and the techniques used for visualization.", CreatedAt = DateTime.Now }
            });

            // CS301 Assessment 1: SQL Fundamentals Quiz (5 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[10].AssessmentId, Type = QuestionType.mcq, Content = "Which SQL statement is used to retrieve data from a database?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[10].AssessmentId, Type = QuestionType.mcq, Content = "What does the WHERE clause do?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[10].AssessmentId, Type = QuestionType.mcq, Content = "Which JOIN returns all records when there is a match in either table?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[10].AssessmentId, Type = QuestionType.mcq, Content = "What is a primary key?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[10].AssessmentId, Type = QuestionType.mcq, Content = "Which SQL clause is used to sort results?", CreatedAt = DateTime.Now }
            });

            // CS301 Assessment 2: Database Design Assignment (2 file + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[11].AssessmentId, Type = QuestionType.file, Content = "Upload your ER diagram for the library management system", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[11].AssessmentId, Type = QuestionType.file, Content = "Upload SQL scripts to create your database schema", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[11].AssessmentId, Type = QuestionType.essay, Content = "Explain the entities, attributes, and relationships in your design.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[11].AssessmentId, Type = QuestionType.essay, Content = "Justify your design decisions and explain how it meets requirements.", CreatedAt = DateTime.Now }
            });

            // CS301 Assessment 3: Normalization Quiz (5 MCQ)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[12].AssessmentId, Type = QuestionType.mcq, Content = "What is the main purpose of database normalization?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[12].AssessmentId, Type = QuestionType.mcq, Content = "Which normal form eliminates transitive dependencies?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[12].AssessmentId, Type = QuestionType.mcq, Content = "What is a foreign key?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[12].AssessmentId, Type = QuestionType.mcq, Content = "What is the benefit of database indexing?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[12].AssessmentId, Type = QuestionType.mcq, Content = "Which type of index is created on multiple columns?", CreatedAt = DateTime.Now }
            });

            // CS301 Assessment 4: Midterm Exam (6 MCQ + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "What is a subquery in SQL?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "Which aggregate function calculates the average?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "What does the GROUP BY clause do?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "What is the difference between HAVING and WHERE?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "Which statement is used to combine result sets?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.mcq, Content = "What is a stored procedure?", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.essay, Content = "Write a complex SQL query using JOINs, subqueries, and aggregation.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[13].AssessmentId, Type = QuestionType.essay, Content = "Explain database transactions and ACID properties.", CreatedAt = DateTime.Now }
            });

            // CS301 Assessment 5: Final Project (3 file + 2 essay)
            questionsList.AddRange(new[]
            {
                new Question { AssessmentId = assessments[14].AssessmentId, Type = QuestionType.file, Content = "Upload complete database schema and SQL scripts", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[14].AssessmentId, Type = QuestionType.file, Content = "Upload your application code that interfaces with the database", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[14].AssessmentId, Type = QuestionType.file, Content = "Upload project documentation including ER diagrams", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[14].AssessmentId, Type = QuestionType.essay, Content = "Describe your database design and explain the normalization level achieved.", CreatedAt = DateTime.Now },
                new Question { AssessmentId = assessments[14].AssessmentId, Type = QuestionType.essay, Content = "Discuss performance optimizations you implemented (indexes, queries, etc.).", CreatedAt = DateTime.Now }
            });

            var questions = questionsList.ToArray();
            context.Questions.AddRange(questions);
            context.SaveChanges();
            Console.WriteLine($"Seeded {questions.Length} questions successfully!");

            // 12. Seed MCQ Options (for all MCQ questions)
            var mcqOptionsList = new List<McqOption>();
            var mcqQuestions = questions.Where(q => q.Type == QuestionType.mcq).ToArray();

            // Helper to add 4 options
            void AddMcqOptions(int questionId, string[] options, int correctIndex)
            {
                char[] labels = { 'A', 'B', 'C', 'D' };
                for (int i = 0; i < options.Length; i++)
                {
                    mcqOptionsList.Add(new McqOption
                    {
                        QuestionId = questionId,
                        OptionLabel = labels[i],
                        OptionText = options[i],
                        IsCorrect = i == correctIndex
                    });
                }
            }

            // CS101 Assessment 1 - MCQ Options
            AddMcqOptions(mcqQuestions[0].QuestionId, new[] { "Central Processing Unit", "Computer Personal Unit", "Central Program Utility", "Central Processor Utility" }, 0);
            AddMcqOptions(mcqQuestions[1].QuestionId, new[] { "HTML", "Python", "CSS", "JSON" }, 1);
            AddMcqOptions(mcqQuestions[2].QuestionId, new[] { "To compile code into machine language", "To debug programs", "To write code", "To run tests" }, 0);
            AddMcqOptions(mcqQuestions[3].QuestionId, new[] { "//", "#", "/*", "--" }, 0);

            // CS101 Assessment 3 - MCQ Options
            AddMcqOptions(mcqQuestions[4].QuestionId, new[] { "int", "bool", "string", "char" }, 2);
            AddMcqOptions(mcqQuestions[5].QuestionId, new[] { "null", "0", "1", "-1" }, 1);
            AddMcqOptions(mcqQuestions[6].QuestionId, new[] { "static", "final", "const", "readonly" }, 2);
            AddMcqOptions(mcqQuestions[7].QuestionId, new[] { "Converting one data type to another", "Creating new types", "Deleting variables", "None of the above" }, 0);
            AddMcqOptions(mcqQuestions[8].QuestionId, new[] { "&&", "+", "||", "==" }, 1);

            // CS201 Assessment 1 - MCQ Options
            AddMcqOptions(mcqQuestions[9].QuestionId, new[] { "O(1)", "O(n)", "O(log n)", "O(n²)" }, 0);
            AddMcqOptions(mcqQuestions[10].QuestionId, new[] { "Random access", "Insertion at beginning", "Deletion from end", "Sorting" }, 1);
            AddMcqOptions(mcqQuestions[11].QuestionId, new[] { "A list with two heads", "A list where each node has two pointers", "A list with two tails", "A list with duplicate values" }, 1);
            AddMcqOptions(mcqQuestions[12].QuestionId, new[] { "O(1)", "O(n)", "O(log n)", "O(n log n)" }, 1);
            AddMcqOptions(mcqQuestions[13].QuestionId, new[] { "Array", "Stack", "Queue", "Tree" }, 1);

            // CS201 Assessment 3 - MCQ Options
            AddMcqOptions(mcqQuestions[14].QuestionId, new[] { "1", "2", "3", "4" }, 1);
            AddMcqOptions(mcqQuestions[15].QuestionId, new[] { "Preorder", "Inorder", "Postorder", "Level order" }, 0);
            AddMcqOptions(mcqQuestions[16].QuestionId, new[] { "O(1)", "O(log n)", "O(n)", "O(n log n)" }, 1);
            AddMcqOptions(mcqQuestions[17].QuestionId, new[] { "BFS", "DFS", "Dijkstra's algorithm", "Prim's algorithm" }, 2);
            AddMcqOptions(mcqQuestions[18].QuestionId, new[] { "A tree with no cycles", "A tree connecting all vertices with minimum edges", "A tree with maximum height", "A balanced tree" }, 1);

            // CS201 Assessment 4 - MCQ Options
            AddMcqOptions(mcqQuestions[19].QuestionId, new[] { "O(n)", "O(n log n)", "O(n²)", "O(log n)" }, 2);
            AddMcqOptions(mcqQuestions[20].QuestionId, new[] { "Bubble Sort", "Merge Sort", "Selection Sort", "Insertion Sort" }, 1);
            AddMcqOptions(mcqQuestions[21].QuestionId, new[] { "O(1)", "O(log n)", "O(n)", "O(n²)" }, 1);
            AddMcqOptions(mcqQuestions[22].QuestionId, new[] { "Quick Sort", "Merge Sort", "Insertion Sort", "Heap Sort" }, 2);
            AddMcqOptions(mcqQuestions[23].QuestionId, new[] { "It sorts in ascending order", "It maintains relative order of equal elements", "It uses less memory", "It is faster" }, 1);
            AddMcqOptions(mcqQuestions[24].QuestionId, new[] { "Binary Search", "Linear Search", "Jump Search", "Interpolation Search" }, 1);

            // CS301 Assessment 1 - MCQ Options
            AddMcqOptions(mcqQuestions[25].QuestionId, new[] { "SELECT", "GET", "FETCH", "RETRIEVE" }, 0);
            AddMcqOptions(mcqQuestions[26].QuestionId, new[] { "Sorts records", "Filters records", "Groups records", "Joins tables" }, 1);
            AddMcqOptions(mcqQuestions[27].QuestionId, new[] { "INNER JOIN", "FULL OUTER JOIN", "LEFT JOIN", "RIGHT JOIN" }, 1);
            AddMcqOptions(mcqQuestions[28].QuestionId, new[] { "A unique identifier for a record", "A foreign reference", "An index", "A constraint" }, 0);
            AddMcqOptions(mcqQuestions[29].QuestionId, new[] { "WHERE", "SORT BY", "ORDER BY", "ARRANGE BY" }, 2);

            // CS301 Assessment 3 - MCQ Options
            AddMcqOptions(mcqQuestions[30].QuestionId, new[] { "To speed up queries", "To reduce data redundancy", "To increase storage", "To add constraints" }, 1);
            AddMcqOptions(mcqQuestions[31].QuestionId, new[] { "1NF", "2NF", "3NF", "BCNF" }, 2);
            AddMcqOptions(mcqQuestions[32].QuestionId, new[] { "A key in another table", "A primary key", "A unique key", "An index" }, 0);
            AddMcqOptions(mcqQuestions[33].QuestionId, new[] { "Reduces storage", "Improves query performance", "Ensures data integrity", "Normalizes data" }, 1);
            AddMcqOptions(mcqQuestions[34].QuestionId, new[] { "Composite index", "Unique index", "Clustered index", "Primary index" }, 0);

            // CS301 Assessment 4 - MCQ Options
            AddMcqOptions(mcqQuestions[35].QuestionId, new[] { "A query nested inside another query", "A query that runs sub-operations", "A query for subtraction", "A query template" }, 0);
            AddMcqOptions(mcqQuestions[36].QuestionId, new[] { "SUM", "AVG", "COUNT", "MAX" }, 1);
            AddMcqOptions(mcqQuestions[37].QuestionId, new[] { "Groups rows with same values", "Orders results", "Filters records", "Joins tables" }, 0);
            AddMcqOptions(mcqQuestions[38].QuestionId, new[] { "No difference", "HAVING filters groups, WHERE filters rows", "HAVING is faster", "WHERE filters groups" }, 1);
            AddMcqOptions(mcqQuestions[39].QuestionId, new[] { "MERGE", "UNION", "COMBINE", "CONCAT" }, 1);
            AddMcqOptions(mcqQuestions[40].QuestionId, new[] { "A saved SQL query", "A precompiled SQL code block", "A table procedure", "A query template" }, 1);

            var mcqOptions = mcqOptionsList.ToArray();
            context.McqOptions.AddRange(mcqOptions);
            context.SaveChanges();
            Console.WriteLine($"Seeded {mcqOptions.Length} MCQ options successfully!");

            // 13. Seed User Assessments (Populate leaderboard with varied completion data)
            var userAssessmentsList = new List<UserAssessment>();
            
            // For each student, complete some assessments from their enrolled classes
            foreach (var student in students)
            {
                // Get classes this student is enrolled in
                var studentEnrollments = enrollments.Where(e => e.StudentId == student.UserId).ToList();
                var studentClassIds = studentEnrollments.Select(e => e.ClassId).ToList();
                
                // Get assessments available for this student
                var availableAssessments = assessments
                    .Where(a => studentClassIds.Contains(a.ClassId) && a.EndDate < DateTime.Now)
                    .ToList();

                if (!availableAssessments.Any()) continue;

                // Determine how many assessments this student completes (varied distribution)
                int studentIndex = Array.IndexOf(students, student);
                int completionRate;
                
                if (studentIndex < 10) // Top 10 students - high performers (80-100% completion)
                    completionRate = random.Next(80, 101);
                else if (studentIndex < 30) // Next 20 students - good performers (60-80% completion)
                    completionRate = random.Next(60, 81);
                else if (studentIndex < 55) // Next 25 students - average performers (40-60% completion)
                    completionRate = random.Next(40, 61);
                else // Remaining students - lower performers (20-50% completion)
                    completionRate = random.Next(20, 51);

                int numToComplete = (availableAssessments.Count * completionRate) / 100;
                if (numToComplete == 0 && availableAssessments.Any()) numToComplete = 1; // At least 1

                // Randomly select assessments to complete
                var completedAssessments = availableAssessments
                    .OrderBy(x => random.Next())
                    .Take(numToComplete)
                    .ToList();

                foreach (var assessment in completedAssessments)
                {
                    // Generate score based on student performance tier
                    int score;
                    if (studentIndex < 10) // Top performers: 85-100
                        score = random.Next(85, 101);
                    else if (studentIndex < 30) // Good performers: 75-90
                        score = random.Next(75, 91);
                    else if (studentIndex < 55) // Average performers: 65-85
                        score = random.Next(65, 86);
                    else // Lower performers: 50-75
                        score = random.Next(50, 76);

                    userAssessmentsList.Add(new UserAssessment
                    {
                        UserId = student.UserId,
                        AssessmentId = assessment.AssessmentId,
                        Status = AssessmentStatus.completed,
                        Score = score,
                        CreatedAt = assessment.StartDate.AddDays(1),
                        UpdatedAt = assessment.EndDate.AddDays(-random.Next(1, 3))
                    });
                }

                // Add some draft assessments for ongoing/upcoming assessments
                var ongoingAssessments = assessments
                    .Where(a => studentClassIds.Contains(a.ClassId) && 
                               a.StartDate <= DateTime.Now && 
                               a.EndDate >= DateTime.Now)
                    .OrderBy(x => random.Next())
                    .Take(random.Next(0, 2)) // 0-1 drafts
                    .ToList();

                foreach (var assessment in ongoingAssessments)
                {
                    userAssessmentsList.Add(new UserAssessment
                    {
                        UserId = student.UserId,
                        AssessmentId = assessment.AssessmentId,
                        Status = AssessmentStatus.draft,
                        Score = null,
                        CreatedAt = assessment.StartDate.AddDays(1),
                        UpdatedAt = DateTime.Now.AddHours(-random.Next(1, 48))
                    });
                }

                // Add a few on_review assessments (submitted but awaiting manual grading)
                // Only for some students (about 10% chance) from their past assessments
                if (studentIndex < 30 && random.Next(100) < 10) // Top 30 students, 10% chance
                {
                    var reviewAssessment = availableAssessments
                        .Where(a => !completedAssessments.Contains(a)) // Not already completed
                        .OrderBy(x => random.Next())
                        .FirstOrDefault();

                    if (reviewAssessment != null)
                    {
                        userAssessmentsList.Add(new UserAssessment
                        {
                            UserId = student.UserId,
                            AssessmentId = reviewAssessment.AssessmentId,
                            Status = AssessmentStatus.on_review,
                            Score = null, // Not yet graded
                            CreatedAt = reviewAssessment.StartDate.AddDays(1),
                            UpdatedAt = reviewAssessment.EndDate.AddHours(-random.Next(1, 24))
                        });
                    }
                }
            }

            var userAssessments = userAssessmentsList.ToArray();
            context.UserAssessments.AddRange(userAssessments);
            context.SaveChanges();
            Console.WriteLine($"Seeded {userAssessments.Length} user assessments successfully!");

            Console.WriteLine("Database seeding completed successfully!");
        }
    }
}