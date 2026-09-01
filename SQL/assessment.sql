-- ============================================================
-- Assess 1 – SQL Server
-- Level: Medium
-- Topics: Broad assessment – DDL, DML, joins, subqueries,
--         functions, stored procedures, views, indexes
-- ============================================================

USE master;
GO

IF DB_ID('Assess1DB') IS NOT NULL
    DROP DATABASE Assess1DB;
GO
CREATE DATABASE Assess1DB;
GO
USE Assess1DB;
GO

-- ============================================================
-- Schema Setup
-- ============================================================
CREATE TABLE Categories (
    CategoryID   INT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL
);

CREATE TABLE Products (
    ProductID    INT           PRIMARY KEY,
    ProductName  VARCHAR(100)  NOT NULL,
    CategoryID   INT           REFERENCES Categories(CategoryID),
    Price        DECIMAL(10,2) NOT NULL,
    Stock        INT           NOT NULL DEFAULT 0,
    AddedDate    DATE          NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Customers (
    CustomerID   INT          PRIMARY KEY,
    CustomerName VARCHAR(100) NOT NULL,
    City         VARCHAR(50),
    Email        VARCHAR(100) UNIQUE
);

CREATE TABLE Orders (
    OrderID    INT           PRIMARY KEY,
    CustomerID INT           REFERENCES Customers(CustomerID),
    OrderDate  DATE          NOT NULL DEFAULT GETDATE(),
    TotalAmt   DECIMAL(12,2) NOT NULL DEFAULT 0
);

CREATE TABLE OrderItems (
    OrderItemID INT           PRIMARY KEY,
    OrderID     INT           REFERENCES Orders(OrderID),
    ProductID   INT           REFERENCES Products(ProductID),
    Qty         INT           NOT NULL,
    UnitPrice   DECIMAL(10,2) NOT NULL
);

-- Sample data
INSERT INTO Categories VALUES (1,'Electronics'),(2,'Clothing'),(3,'Books'),(4,'Food');
INSERT INTO Products VALUES
(1,'Laptop',   1, 55000, 10, '2024-01-10'),
(2,'Phone',    1, 20000, 25, '2024-02-15'),
(3,'T-Shirt',  2,   500, 50, '2024-01-20'),
(4,'Jeans',    2,  1500, 30, '2024-03-05'),
(5,'SQL Book', 3,   800, 20, '2024-04-01'),
(6,'Rice 5kg', 4,   300,100, '2024-05-10');
INSERT INTO Customers VALUES
(1,'Arjun Shetty','Bengaluru','arjun@email.com'),
(2,'Priya Nair',  'Mumbai',   'priya@email.com'),
(3,'Rahul Gupta', 'Delhi',    'rahul@email.com'),
(4,'Sneha Sharma','Pune',     'sneha@email.com');
INSERT INTO Orders VALUES
(1,1,'2025-01-10',75500),(2,2,'2025-01-15',2000),
(3,1,'2025-02-20',800),  (4,3,'2025-03-01',300),
(5,4,'2025-03-10',21500),(6,2,'2025-04-05',1500);
INSERT INTO OrderItems VALUES
(1,1,1,1,55000),(2,1,2,1,20000),(3,1,3,1,500),
(4,2,3,2,500),  (5,2,4,1,1000),
(6,3,5,1,800),
(7,4,6,1,300),
(8,5,2,1,20000),(9,5,4,1,1500),
(10,6,4,1,1500);

-- ============================================================
-- Q1. Products with price above average
-- ============================================================
SELECT ProductName, Price
FROM   Products
WHERE  Price > (SELECT AVG(Price) FROM Products)
ORDER  BY Price DESC;

-- ============================================================
-- Q2. Total revenue per customer
-- ============================================================
SELECT
    c.CustomerName,
    SUM(oi.Qty * oi.UnitPrice) AS TotalRevenue
FROM Customers   c
JOIN Orders      o  ON c.CustomerID = o.CustomerID
JOIN OrderItems  oi ON o.OrderID    = oi.OrderID
GROUP BY c.CustomerName
ORDER BY TotalRevenue DESC;

-- ============================================================
-- Q3. Most popular product (by quantity sold)
-- ============================================================
SELECT TOP 1
    p.ProductName,
    SUM(oi.Qty) AS TotalQtySold
FROM Products   p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
GROUP BY p.ProductName
ORDER BY TotalQtySold DESC;

-- ============================================================
-- Q4. Customers who have never placed an order
-- ============================================================
SELECT CustomerName, Email
FROM   Customers
WHERE  CustomerID NOT IN (SELECT DISTINCT CustomerID FROM Orders);

-- ============================================================
-- Q5. Monthly order count and revenue
-- ============================================================
SELECT
    YEAR(o.OrderDate)   AS OrderYear,
    MONTH(o.OrderDate)  AS OrderMonth,
    COUNT(o.OrderID)    AS OrderCount,
    SUM(oi.Qty * oi.UnitPrice) AS MonthlyRevenue
FROM Orders      o
JOIN OrderItems  oi ON o.OrderID = oi.OrderID
GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate)
ORDER BY OrderYear, OrderMonth;

-- ============================================================
-- Q6. Create a VIEW for order summary
-- ============================================================
CREATE VIEW vw_OrderSummary AS
SELECT
    o.OrderID,
    c.CustomerName,
    o.OrderDate,
    COUNT(oi.OrderItemID)       AS ItemCount,
    SUM(oi.Qty * oi.UnitPrice)  AS OrderTotal
FROM Orders     o
JOIN Customers  c  ON o.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID    = oi.OrderID
GROUP BY o.OrderID, c.CustomerName, o.OrderDate;
GO

SELECT * FROM vw_OrderSummary ORDER BY OrderDate;
GO

-- ============================================================
-- Q7. Create a STORED PROCEDURE – get orders by customer
-- ============================================================
CREATE PROCEDURE usp_GetCustomerOrders
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        o.OrderID,
        o.OrderDate,
        p.ProductName,
        oi.Qty,
        oi.UnitPrice,
        oi.Qty * oi.UnitPrice AS LineTotal
    FROM Orders     o
    JOIN OrderItems oi ON o.OrderID    = oi.OrderID
    JOIN Products   p  ON oi.ProductID = p.ProductID
    WHERE o.CustomerID = @CustomerID
    ORDER BY o.OrderDate DESC;
END;
GO

EXEC usp_GetCustomerOrders @CustomerID = 1;
GO

-- ============================================================
-- Q8. Create an INDEX for performance
-- ============================================================
CREATE NONCLUSTERED INDEX IX_Orders_CustomerID
    ON Orders (CustomerID)
    INCLUDE (OrderDate, TotalAmt);

CREATE NONCLUSTERED INDEX IX_OrderItems_ProductID
    ON OrderItems (ProductID)
    INCLUDE (Qty, UnitPrice);

-- ============================================================
-- Q9. Stock update after sale (DML transaction)
-- ============================================================
BEGIN TRAN;
    UPDATE Products SET Stock = Stock - 1 WHERE ProductID = 1;  -- sold 1 Laptop
    -- If something goes wrong: ROLLBACK TRAN;
COMMIT TRAN;

SELECT ProductID, ProductName, Stock FROM Products WHERE ProductID = 1;

-- ============================================================
-- Q10. Category-wise revenue report
-- ============================================================
SELECT
    cat.CategoryName,
    COUNT(DISTINCT o.OrderID)         AS TotalOrders,
    SUM(oi.Qty)                       AS UnitsSold,
    SUM(oi.Qty * oi.UnitPrice)        AS Revenue
FROM Categories  cat
JOIN Products    p   ON cat.CategoryID = p.CategoryID
JOIN OrderItems  oi  ON p.ProductID    = oi.ProductID
JOIN Orders      o   ON oi.OrderID     = o.OrderID
GROUP BY cat.CategoryName
ORDER BY Revenue DESC;
GO
