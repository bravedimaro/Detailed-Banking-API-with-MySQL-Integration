-- Banking API - MySQL Database Setup Script
-- Run this script to create the database and user (optional)
-- EF Core migrations will handle table creation automatically

CREATE DATABASE IF NOT EXISTS BankingDB
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

-- Optional: create a dedicated DB user
-- CREATE USER IF NOT EXISTS 'bankingapi'@'localhost' IDENTIFIED BY 'StrongPassword123!';
-- GRANT ALL PRIVILEGES ON BankingDB.* TO 'bankingapi'@'localhost';
-- FLUSH PRIVILEGES;

USE BankingDB;

-- Tables are created by EF Core migrations (dotnet ef database update)
-- The schema below is for reference only

/*
CREATE TABLE Users (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(256) NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    PhoneNumber VARCHAR(20) NULL,
    Address VARCHAR(500) NULL,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)
);

CREATE TABLE Accounts (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    UserId CHAR(36) NOT NULL UNIQUE,
    AccountNumber VARCHAR(50) NOT NULL UNIQUE,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT FK_Accounts_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE Transactions (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    SenderAccountId CHAR(36) NOT NULL,
    ReceiverAccountId CHAR(36) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description TEXT NULL,
    Status INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT FK_Transactions_Sender FOREIGN KEY (SenderAccountId) REFERENCES Accounts(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Transactions_Receiver FOREIGN KEY (ReceiverAccountId) REFERENCES Accounts(Id) ON DELETE RESTRICT
);
*/
