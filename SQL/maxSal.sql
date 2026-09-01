-- ============================================================
-- MAX Salary
-- Level: Medium
-- Topics: MAX(), GROUP BY, subqueries, DENSE_RANK, Nth max salary
-- ============================================================

USE master;
GO

IF DB_ID('SalaryDB') IS NOT NULL
    DROP DATABASE SalaryDB;
GO
CREATE DATABASE SalaryDB;
GO
USE SalaryDB;
GO

-- ============================================================
-- Setup
-- ============================================================
CREATE TABLE Employees (
    EmpID      INT          NOT NULL PRIMARY KEY,
    EmpName    VARCHAR(100) NOT NULL,
    Department VARCHAR(50)  NOT NULL,
    Salary     DECIMAL(10,2) NOT NULL
);

INSERT INTO Employees VALUES
(1,  'Alice',   'Engineering', 95000),
(2,  'Bob',     'Engineering', 88000),
(3,  'Charlie', 'Engineering', 95000),   -- tied with Alice
(4,  'Diana',   'HR',          62000),
(5,  'Edward',  'HR',          70000),
(6,  'Fiona',   'Finance',     80000),
(7,  'George',  'Finance',     85000),
(8,  'Hannah',  'Finance',     80000),   -- tied with Fiona
(9,  'Ivan',    'Engineering', 75000),
(10, 'Julia',   'HR',          55000);

-- ============================================================
-- 1. Global MAX salary
-- ============================================================
SELECT MAX(Salary) AS MaxSalary
FROM   Employees;

-- ============================================================
-- 2. MAX salary per department
-- ============================================================
SELECT
    Department,
    MAX(Salary) AS MaxSalary
FROM   Employees
GROUP BY Department
ORDER BY MaxSalary DESC;

-- ============================================================
-- 3. Employee(s) earning the global MAX salary
-- ============================================================
SELECT EmpID, EmpName, Department, Salary
FROM   Employees
WHERE  Salary = (SELECT MAX(Salary) FROM Employees);

-- ============================================================
-- 4. Employee with MAX salary in EACH department
-- ============================================================
SELECT e.EmpID, e.EmpName, e.Department, e.Salary
FROM   Employees e
INNER JOIN
(
    SELECT Department, MAX(Salary) AS MaxSalary
    FROM   Employees
    GROUP  BY Department
) m ON e.Department = m.Department
   AND e.Salary     = m.MaxSalary
ORDER BY e.Department;

-- ============================================================
-- 5. 2nd highest salary (globally) – using subquery
-- ============================================================
SELECT MAX(Salary) AS SecondHighestSalary
FROM   Employees
WHERE  Salary < (SELECT MAX(Salary) FROM Employees);

-- ============================================================
-- 6. Nth highest salary – using DENSE_RANK (handles ties)
-- ============================================================
DECLARE @N INT = 2;   -- change to get Nth highest

SELECT DISTINCT Salary AS [NthHighestSalary]
FROM
(
    SELECT
        Salary,
        DENSE_RANK() OVER (ORDER BY Salary DESC) AS SalaryRank
    FROM Employees
) AS Ranked
WHERE SalaryRank = @N;

-- ============================================================
-- 7. Salary ranking per department using DENSE_RANK
-- ============================================================
SELECT
    EmpName,
    Department,
    Salary,
    DENSE_RANK() OVER (PARTITION BY Department ORDER BY Salary DESC) AS DeptSalaryRank
FROM Employees
ORDER BY Department, DeptSalaryRank;

-- ============================================================
-- 8. Employee with 2nd highest salary IN EACH department
-- ============================================================
SELECT EmpName, Department, Salary, DeptSalaryRank
FROM
(
    SELECT
        EmpName, Department, Salary,
        DENSE_RANK() OVER (PARTITION BY Department ORDER BY Salary DESC) AS DeptSalaryRank
    FROM Employees
) AS Ranked
WHERE DeptSalaryRank = 2;

-- ============================================================
-- 9. Top 3 unique salaries across entire company
-- ============================================================
SELECT DISTINCT TOP 3 Salary
FROM   Employees
ORDER  BY Salary DESC;

-- ============================================================
-- 10. MIN and MAX salary side-by-side per department
-- ============================================================
SELECT
    Department,
    MIN(Salary) AS MinSalary,
    MAX(Salary) AS MaxSalary,
    MAX(Salary) - MIN(Salary) AS SalarySpread,
    AVG(Salary) AS AvgSalary
FROM Employees
GROUP BY Department
ORDER BY Department;
GO
