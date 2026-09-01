-- ============================================================
-- Exercise 03 – Constraints
-- Level: Medium
-- Topics: PRIMARY KEY, FOREIGN KEY, UNIQUE, CHECK, DEFAULT, NOT NULL
-- ============================================================

USE master;
GO

-- Create a fresh database for this exercise
IF DB_ID('ConstraintsDB') IS NOT NULL
    DROP DATABASE ConstraintsDB;
GO
CREATE DATABASE ConstraintsDB;
GO
USE ConstraintsDB;
GO

-- ============================================================
-- 1. PRIMARY KEY Constraint
--    Uniquely identifies each row; cannot be NULL.
-- ============================================================
CREATE TABLE Departments (
    DepartmentID   INT           NOT NULL,
    DepartmentName VARCHAR(100)  NOT NULL,
    Location       VARCHAR(100),
    CONSTRAINT PK_Departments PRIMARY KEY (DepartmentID)
);

-- ============================================================
-- 2. FOREIGN KEY Constraint
--    Enforces referential integrity between tables.
-- ============================================================
CREATE TABLE Employees (
    EmployeeID   INT          NOT NULL,
    FirstName    VARCHAR(50)  NOT NULL,
    LastName     VARCHAR(50)  NOT NULL,
    DepartmentID INT          NOT NULL,
    CONSTRAINT PK_Employees  PRIMARY KEY (EmployeeID),
    CONSTRAINT FK_Emp_Dept   FOREIGN KEY (DepartmentID)
        REFERENCES Departments(DepartmentID)
        ON DELETE NO ACTION
        ON UPDATE CASCADE
);

-- ============================================================
-- 3. UNIQUE Constraint
--    Ensures all values in a column (or column set) are distinct.
-- ============================================================
CREATE TABLE Products (
    ProductID   INT           NOT NULL  CONSTRAINT PK_Products PRIMARY KEY,
    ProductCode VARCHAR(20)   NOT NULL  CONSTRAINT UQ_ProductCode UNIQUE,
    ProductName VARCHAR(100)  NOT NULL,
    Price       DECIMAL(10,2) NOT NULL
);

-- ============================================================
-- 4. CHECK Constraint
--    Validates that column values satisfy a boolean expression.
-- ============================================================
CREATE TABLE Orders (
    OrderID    INT           NOT NULL  CONSTRAINT PK_Orders PRIMARY KEY,
    Quantity   INT           NOT NULL  CONSTRAINT CHK_Quantity   CHECK (Quantity > 0),
    UnitPrice  DECIMAL(10,2) NOT NULL  CONSTRAINT CHK_UnitPrice  CHECK (UnitPrice >= 0),
    OrderDate  DATE          NOT NULL,
    Status     VARCHAR(20)   NOT NULL  CONSTRAINT CHK_Status
                                        CHECK (Status IN ('Pending','Processing','Shipped','Delivered','Cancelled'))
);

-- ============================================================
-- 5. DEFAULT Constraint
--    Provides a default value when no value is supplied.
-- ============================================================
CREATE TABLE Customers (
    CustomerID   INT          NOT NULL  CONSTRAINT PK_Customers PRIMARY KEY,
    CustomerName VARCHAR(100) NOT NULL,
    Country      VARCHAR(50)  NOT NULL  CONSTRAINT DF_Country    DEFAULT 'India',
    IsActive     BIT          NOT NULL  CONSTRAINT DF_IsActive   DEFAULT 1,
    CreatedAt    DATETIME     NOT NULL  CONSTRAINT DF_CreatedAt  DEFAULT GETDATE()
);

-- ============================================================
-- 6. NOT NULL Constraint
--    Demonstrated inline; FirstName & LastName above are examples.
--    Here we show altering a column to add NOT NULL.
-- ============================================================
ALTER TABLE Departments
    ALTER COLUMN Location VARCHAR(100) NOT NULL;

-- ============================================================
-- Demo: Insert valid data
-- ============================================================
INSERT INTO Departments (DepartmentID, DepartmentName, Location)
VALUES (1, 'Engineering', 'Bengaluru'),
       (2, 'HR',          'Mumbai'),
       (3, 'Finance',     'Delhi');

INSERT INTO Employees (EmployeeID, FirstName, LastName, DepartmentID)
VALUES (1, 'Arjun',  'Shetty', 1),
       (2, 'Priya',  'Nair',   2),
       (3, 'Rahul',  'Gupta',  1);

INSERT INTO Customers (CustomerID, CustomerName)        -- Country/IsActive/CreatedAt use DEFAULTs
VALUES (1, 'TechCorp Solutions'),
       (2, 'Global Traders');

INSERT INTO Products (ProductID, ProductCode, ProductName, Price)
VALUES (1, 'PRD-001', 'Laptop',     75000.00),
       (2, 'PRD-002', 'Mouse',       599.00),
       (3, 'PRD-003', 'Keyboard',   1299.00);

INSERT INTO Orders (OrderID, Quantity, UnitPrice, OrderDate, Status)
VALUES (1, 2, 75000.00, '2025-01-15', 'Delivered'),
       (2, 5,   599.00, '2025-03-20', 'Shipped');

-- ============================================================
-- Demo: Constraint violation tests (commented out – would error)
-- ============================================================
-- INSERT INTO Employees (EmployeeID, FirstName, LastName, DepartmentID)
-- VALUES (4, 'Test', 'User', 99);  -- FK violation: DeptID 99 doesn't exist

-- INSERT INTO Products (ProductID, ProductCode, ProductName, Price)
-- VALUES (4, 'PRD-001', 'Monitor', 15000.00);  -- UNIQUE violation: PRD-001 already exists

-- INSERT INTO Orders (OrderID, Quantity, UnitPrice, OrderDate, Status)
-- VALUES (3, -1, 200, GETDATE(), 'Pending');  -- CHECK violation: Quantity must be > 0

-- ============================================================
-- Verification Queries
-- ============================================================
SELECT * FROM Departments;
SELECT * FROM Employees;
SELECT * FROM Customers;
SELECT * FROM Products;
SELECT * FROM Orders;

-- List all constraints on a table
SELECT
    tc.CONSTRAINT_NAME,
    tc.CONSTRAINT_TYPE,
    kcu.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE  kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE tc.TABLE_NAME = 'Employees';
GO
