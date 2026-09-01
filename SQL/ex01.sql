-- ============================================================
-- Exercise 01 – Creating Tables
-- Level: Easy
-- Topics: CREATE TABLE, data types, basic column definitions
-- ============================================================

USE master;
GO

IF DB_ID('SchoolDB') IS NOT NULL
    DROP DATABASE SchoolDB;
GO
CREATE DATABASE SchoolDB;
GO
USE SchoolDB;
GO

-- ============================================================
-- 1. Students table
-- ============================================================
CREATE TABLE Students (
    StudentID   INT           NOT NULL PRIMARY KEY,
    FirstName   VARCHAR(50)   NOT NULL,
    LastName    VARCHAR(50)   NOT NULL,
    DateOfBirth DATE          NOT NULL,
    Gender      CHAR(1)       NOT NULL,   -- 'M' or 'F'
    Email       VARCHAR(100)      NULL,
    Phone       VARCHAR(15)       NULL,
    EnrolledOn  DATE          NOT NULL DEFAULT GETDATE(),
    IsActive    BIT           NOT NULL DEFAULT 1
);

-- ============================================================
-- 2. Courses table
-- ============================================================
CREATE TABLE Courses (
    CourseID    INT          NOT NULL PRIMARY KEY,
    CourseName  VARCHAR(100) NOT NULL,
    Credits     INT          NOT NULL,
    MaxStudents INT          NOT NULL DEFAULT 30,
    Description NVARCHAR(500)    NULL
);

-- ============================================================
-- 3. Teachers table
-- ============================================================
CREATE TABLE Teachers (
    TeacherID  INT          NOT NULL PRIMARY KEY,
    FullName   VARCHAR(100) NOT NULL,
    Subject    VARCHAR(50)  NOT NULL,
    HireDate   DATE         NOT NULL,
    Salary     DECIMAL(10,2)    NULL
);

-- ============================================================
-- 4. Enrollments table (junction / bridge table)
-- ============================================================
CREATE TABLE Enrollments (
    EnrollmentID INT      NOT NULL PRIMARY KEY,
    StudentID    INT      NOT NULL REFERENCES Students(StudentID),
    CourseID     INT      NOT NULL REFERENCES Courses(CourseID),
    EnrolledDate DATE     NOT NULL DEFAULT GETDATE(),
    Grade        CHAR(2)      NULL   -- A, B, C, D, F
);

-- ============================================================
-- 5. CourseFees table
-- ============================================================
CREATE TABLE CourseFees (
    FeeID      INT           NOT NULL PRIMARY KEY,
    CourseID   INT           NOT NULL REFERENCES Courses(CourseID),
    FeeAmount  DECIMAL(10,2) NOT NULL,
    FeePeriod  VARCHAR(20)   NOT NULL DEFAULT 'Semester'  -- 'Semester','Annual'
);

-- ============================================================
-- Insert sample data
-- ============================================================
INSERT INTO Students (StudentID, FirstName, LastName, DateOfBirth, Gender, Email)
VALUES
(1, 'Arjun',  'Shetty',  '2001-05-15', 'M', 'arjun@school.com'),
(2, 'Priya',  'Nair',    '2002-08-22', 'F', 'priya@school.com'),
(3, 'Rahul',  'Gupta',   '2001-11-30', 'M', 'rahul@school.com'),
(4, 'Sneha',  'Sharma',  '2003-03-10', 'F', 'sneha@school.com'),
(5, 'Kiran',  'Mehta',   '2000-07-05', 'M', 'kiran@school.com');

INSERT INTO Courses (CourseID, CourseName, Credits)
VALUES
(1, 'Mathematics',         4),
(2, 'Computer Science',    4),
(3, 'Physics',             3),
(4, 'English Literature',  3),
(5, 'Chemistry',           3);

INSERT INTO Teachers (TeacherID, FullName, Subject, HireDate, Salary)
VALUES
(1, 'Dr. Anand Kumar',   'Mathematics',      '2018-06-01', 75000),
(2, 'Ms. Rita Sharma',   'Computer Science', '2020-07-15', 80000),
(3, 'Mr. Suresh Patel',  'Physics',          '2019-01-10', 70000);

INSERT INTO Enrollments (EnrollmentID, StudentID, CourseID, Grade)
VALUES
(1, 1, 1, 'A'),
(2, 1, 2, 'B'),
(3, 2, 1, 'A'),
(4, 2, 3, 'B'),
(5, 3, 2, 'A'),
(6, 4, 4, 'B'),
(7, 5, 5, 'C');

-- ============================================================
-- Verification Queries
-- ============================================================
SELECT 'Students'   AS TableName, COUNT(*) AS RowCount FROM Students
UNION ALL
SELECT 'Courses',    COUNT(*) FROM Courses
UNION ALL
SELECT 'Teachers',   COUNT(*) FROM Teachers
UNION ALL
SELECT 'Enrollments',COUNT(*) FROM Enrollments;

-- Show table schema
SELECT
    c.name          AS ColumnName,
    t.name          AS DataType,
    c.max_length,
    c.is_nullable
FROM sys.columns c
JOIN sys.types   t ON c.user_type_id = t.user_type_id
WHERE  OBJECT_NAME(c.object_id) = 'Students'
ORDER  BY c.column_id;
GO
