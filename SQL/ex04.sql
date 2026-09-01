-- ============================================================
-- Exercise 04 – Querying Records
-- Level: Hard
-- Topics: SELECT, WHERE, ORDER BY, GROUP BY, HAVING, JOINs,
--         subqueries, aggregate functions, CASE, TOP, DISTINCT
-- ============================================================

USE SchoolDB;   -- Reuse DB created in Exercise 01
GO

-- ============================================================
-- 1. Basic SELECT – all columns
-- ============================================================
SELECT * FROM Students;

-- ============================================================
-- 2. SELECT specific columns with alias
-- ============================================================
SELECT
    StudentID                               AS ID,
    FirstName + ' ' + LastName             AS FullName,
    DateOfBirth,
    DATEDIFF(YEAR, DateOfBirth, GETDATE()) AS Age
FROM Students;

-- ============================================================
-- 3. WHERE – filter rows
-- ============================================================
-- Students who are male
SELECT * FROM Students WHERE Gender = 'M';

-- Students enrolled after 2001
SELECT * FROM Students WHERE YEAR(DateOfBirth) > 2001;

-- ============================================================
-- 4. ORDER BY – sort results
-- ============================================================
SELECT FirstName, LastName, DateOfBirth
FROM   Students
ORDER  BY DateOfBirth DESC;   -- youngest first

-- ============================================================
-- 5. TOP – limit rows
-- ============================================================
SELECT TOP 3 FirstName, LastName
FROM   Students
ORDER  BY EnrolledOn DESC;    -- 3 most recently enrolled

-- ============================================================
-- 6. DISTINCT – unique values
-- ============================================================
SELECT DISTINCT Grade FROM Enrollments WHERE Grade IS NOT NULL;

-- ============================================================
-- 7. Aggregate Functions: COUNT, SUM, AVG, MIN, MAX
-- ============================================================
SELECT
    COUNT(*)                      AS TotalStudents,
    COUNT(CASE WHEN Gender='M' THEN 1 END) AS MaleCount,
    COUNT(CASE WHEN Gender='F' THEN 1 END) AS FemaleCount
FROM Students;

SELECT
    AVG(Salary)  AS AvgTeacherSalary,
    MAX(Salary)  AS MaxSalary,
    MIN(Salary)  AS MinSalary,
    SUM(Salary)  AS TotalSalaryBill
FROM Teachers;

-- ============================================================
-- 8. GROUP BY – aggregate per group
-- ============================================================
-- Number of enrollments per course
SELECT
    c.CourseName,
    COUNT(e.EnrollmentID) AS EnrolledCount
FROM Courses   c
LEFT JOIN Enrollments e ON c.CourseID = e.CourseID
GROUP BY c.CourseName
ORDER BY EnrolledCount DESC;

-- ============================================================
-- 9. HAVING – filter groups
-- ============================================================
-- Courses with more than 1 enrollment
SELECT
    CourseID,
    COUNT(*) AS EnrollCount
FROM Enrollments
GROUP BY CourseID
HAVING COUNT(*) > 1;

-- ============================================================
-- 10. INNER JOIN
-- ============================================================
SELECT
    s.FirstName + ' ' + s.LastName AS Student,
    c.CourseName,
    e.Grade
FROM Enrollments e
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN Courses  c ON e.CourseID  = c.CourseID
ORDER BY Student;

-- ============================================================
-- 11. LEFT JOIN – include students with no enrollment
-- ============================================================
SELECT
    s.StudentID,
    s.FirstName + ' ' + s.LastName AS Student,
    COUNT(e.EnrollmentID)          AS CourseCount
FROM Students   s
LEFT JOIN Enrollments e ON s.StudentID = e.StudentID
GROUP BY s.StudentID, s.FirstName, s.LastName
ORDER BY CourseCount DESC;

-- ============================================================
-- 12. Subquery – students enrolled in 'Computer Science'
-- ============================================================
SELECT FirstName, LastName
FROM   Students
WHERE  StudentID IN
(
    SELECT StudentID
    FROM   Enrollments
    WHERE  CourseID = (SELECT CourseID FROM Courses WHERE CourseName = 'Computer Science')
);

-- ============================================================
-- 13. CASE expression
-- ============================================================
SELECT
    FirstName + ' ' + LastName AS Student,
    Grade,
    CASE Grade
        WHEN 'A' THEN 'Excellent'
        WHEN 'B' THEN 'Good'
        WHEN 'C' THEN 'Average'
        WHEN 'D' THEN 'Below Average'
        WHEN 'F' THEN 'Fail'
        ELSE 'Not Graded'
    END AS GradeDescription
FROM Enrollments e
JOIN Students    s ON e.StudentID = s.StudentID;

-- ============================================================
-- 14. String functions
-- ============================================================
SELECT
    UPPER(FirstName)                   AS UpperFirst,
    LOWER(LastName)                    AS LowerLast,
    LEN(FirstName)                     AS NameLength,
    SUBSTRING(Email, 1, CHARINDEX('@', Email)-1) AS EmailUsername
FROM Students;

-- ============================================================
-- 15. Date functions
-- ============================================================
SELECT
    FirstName,
    DateOfBirth,
    YEAR(DateOfBirth)                          AS BirthYear,
    MONTH(DateOfBirth)                         AS BirthMonth,
    DATEDIFF(YEAR, DateOfBirth, GETDATE())     AS Age,
    FORMAT(DateOfBirth, 'dd-MMM-yyyy')         AS FormattedDOB
FROM Students;
GO
