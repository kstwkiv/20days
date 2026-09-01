-- ============================================================
-- CRUD Application
-- Level: Hard
-- Topics: Complete Create, Read, Update, Delete operations
--         with stored procedures, transactions, error handling,
--         output clauses, merge (upsert)
-- ============================================================

USE master;
GO

IF DB_ID('CrudAppDB') IS NOT NULL
    DROP DATABASE CrudAppDB;
GO
CREATE DATABASE CrudAppDB;
GO
USE CrudAppDB;
GO

-- ============================================================
-- Schema
-- ============================================================
CREATE TABLE Products (
    ProductID    INT           IDENTITY(1,1) PRIMARY KEY,
    ProductName  VARCHAR(100)  NOT NULL,
    Category     VARCHAR(50)   NOT NULL,
    Price        DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Stock        INT           NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt    DATETIME          NULL,
    IsDeleted    BIT           NOT NULL DEFAULT 0   -- soft-delete flag
);

CREATE TABLE ProductAudit (
    AuditID     INT           IDENTITY(1,1) PRIMARY KEY,
    ProductID   INT           NOT NULL,
    Action      VARCHAR(10)   NOT NULL,   -- INSERT / UPDATE / DELETE
    OldValues   NVARCHAR(MAX)     NULL,
    NewValues   NVARCHAR(MAX)     NULL,
    ChangedBy   VARCHAR(100)  NOT NULL DEFAULT SYSTEM_USER,
    ChangedAt   DATETIME      NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- CREATE – Insert a new product
-- ============================================================
CREATE PROCEDURE usp_CreateProduct
    @ProductName  VARCHAR(100),
    @Category     VARCHAR(50),
    @Price        DECIMAL(10,2),
    @Stock        INT,
    @NewProductID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
            INSERT INTO Products (ProductName, Category, Price, Stock)
            VALUES (@ProductName, @Category, @Price, @Stock);

            SET @NewProductID = SCOPE_IDENTITY();

            INSERT INTO ProductAudit (ProductID, Action, NewValues)
            VALUES (@NewProductID, 'INSERT',
                    CONCAT('Name=', @ProductName,
                           ', Price=', @Price,
                           ', Stock=', @Stock));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        THROW;  -- re-raise the error
    END CATCH
END;
GO

-- ============================================================
-- READ – Get all active products / single product
-- ============================================================
CREATE PROCEDURE usp_GetProducts
    @ProductID INT = NULL   -- NULL = get all
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ProductID, ProductName, Category, Price, Stock, CreatedAt, UpdatedAt
    FROM Products
    WHERE IsDeleted = 0
      AND (@ProductID IS NULL OR ProductID = @ProductID)
    ORDER BY ProductID;
END;
GO

-- ============================================================
-- READ with filter – search by category
-- ============================================================
CREATE PROCEDURE usp_GetProductsByCategory
    @Category VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductID, ProductName, Price, Stock
    FROM   Products
    WHERE  IsDeleted = 0
      AND  Category = @Category
    ORDER  BY ProductName;
END;
GO

-- ============================================================
-- UPDATE – Modify an existing product
-- ============================================================
CREATE PROCEDURE usp_UpdateProduct
    @ProductID   INT,
    @ProductName VARCHAR(100) = NULL,
    @Category    VARCHAR(50)  = NULL,
    @Price       DECIMAL(10,2)= NULL,
    @Stock       INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
            DECLARE @OldName  VARCHAR(100),
                    @OldPrice DECIMAL(10,2),
                    @OldStock INT;

            SELECT @OldName  = ProductName,
                   @OldPrice = Price,
                   @OldStock = Stock
            FROM Products WHERE ProductID = @ProductID AND IsDeleted = 0;

            IF @OldName IS NULL
            BEGIN
                RAISERROR('Product ID %d not found.', 16, 1, @ProductID);
                RETURN;
            END

            UPDATE Products
            SET
                ProductName = ISNULL(@ProductName, ProductName),
                Category    = ISNULL(@Category,    Category),
                Price       = ISNULL(@Price,        Price),
                Stock       = ISNULL(@Stock,        Stock),
                UpdatedAt   = GETDATE()
            WHERE ProductID = @ProductID;

            INSERT INTO ProductAudit (ProductID, Action, OldValues, NewValues)
            VALUES (
                @ProductID, 'UPDATE',
                CONCAT('Name=', @OldName,  ', Price=', @OldPrice, ', Stock=', @OldStock),
                CONCAT('Name=', ISNULL(@ProductName, @OldName),
                       ', Price=', ISNULL(@Price, @OldPrice),
                       ', Stock=', ISNULL(@Stock, @OldStock))
            );
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

-- ============================================================
-- DELETE – Soft delete (mark IsDeleted = 1)
-- ============================================================
CREATE PROCEDURE usp_DeleteProduct
    @ProductID INT,
    @HardDelete BIT = 0   -- 0 = soft delete, 1 = hard delete
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
            IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductID = @ProductID AND IsDeleted = 0)
            BEGIN
                RAISERROR('Product ID %d not found.', 16, 1, @ProductID);
                RETURN;
            END

            IF @HardDelete = 0
            BEGIN
                UPDATE Products SET IsDeleted = 1, UpdatedAt = GETDATE()
                WHERE  ProductID = @ProductID;
            END
            ELSE
            BEGIN
                DELETE FROM Products WHERE ProductID = @ProductID;
            END

            INSERT INTO ProductAudit (ProductID, Action, OldValues)
            VALUES (@ProductID, 'DELETE',
                    CASE WHEN @HardDelete = 1 THEN 'Hard Delete' ELSE 'Soft Delete' END);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

-- ============================================================
-- UPSERT – MERGE (Insert if new, Update if exists)
-- ============================================================
CREATE PROCEDURE usp_UpsertProduct
    @ProductName VARCHAR(100),
    @Category    VARCHAR(50),
    @Price       DECIMAL(10,2),
    @Stock       INT
AS
BEGIN
    SET NOCOUNT ON;
    MERGE Products AS Target
    USING (SELECT @ProductName AS PN, @Category AS Cat, @Price AS Pr, @Stock AS St) AS Source
        ON Target.ProductName = Source.PN AND Target.IsDeleted = 0
    WHEN MATCHED THEN
        UPDATE SET
            Price     = Source.Pr,
            Stock     = Source.St,
            UpdatedAt = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (ProductName, Category, Price, Stock)
        VALUES (Source.PN, Source.Cat, Source.Pr, Source.St);
END;
GO

-- ============================================================
-- Demo: run all CRUD operations
-- ============================================================
DECLARE @ID INT;

-- CREATE
EXEC usp_CreateProduct 'Laptop',  'Electronics', 55000, 10, @NewProductID = @ID OUTPUT;
PRINT 'Created ProductID: ' + CAST(@ID AS VARCHAR);
EXEC usp_CreateProduct 'Mouse',   'Electronics',   599, 50, @NewProductID = @ID OUTPUT;
EXEC usp_CreateProduct 'T-Shirt', 'Clothing',       500,100, @NewProductID = @ID OUTPUT;

-- READ
EXEC usp_GetProducts;                           -- all products
EXEC usp_GetProducts @ProductID = 1;            -- single product
EXEC usp_GetProductsByCategory 'Electronics';   -- by category

-- UPDATE
EXEC usp_UpdateProduct @ProductID = 1, @Price = 52000, @Stock = 8;

-- UPSERT
EXEC usp_UpsertProduct 'Laptop', 'Electronics', 50000, 12;  -- should UPDATE
EXEC usp_UpsertProduct 'Keyboard','Electronics', 1299,  40; -- should INSERT

-- DELETE (soft)
EXEC usp_DeleteProduct @ProductID = 3, @HardDelete = 0;

-- READ again to confirm soft delete
EXEC usp_GetProducts;

-- Audit trail
SELECT * FROM ProductAudit ORDER BY AuditID;
GO
