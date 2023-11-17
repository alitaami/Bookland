-- User table
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(255) UNIQUE,
    Email VARCHAR(255) UNIQUE,
    HashedPassword TEXT NOT NULL,
    IsActive BOOLEAN NOT NULL,
    RegistrationDate TIMESTAMP NOT NULL,
    LastLoginDate TIMESTAMP,
    IsPublisher BOOLEAN NOT NULL,
    PhoneNumber VARCHAR(20) UNIQUE,
    PhoneNumber2 VARCHAR(20),
    Address TEXT,
    IdentityImage TEXT,
    CardNumber VARCHAR(50),
    PublicationsName VARCHAR(255) UNIQUE,
    PublicationsImage TEXT,
    IsConfirm BOOLEAN NOT NULL
);

CREATE TABLE UserActivityCodes (
    Id SERIAL PRIMARY KEY,
    Email VARCHAR(255),
    ActivationCode VARCHAR(6),
    CreatedDateTime TIMESTAMP,
    ExpireDateTime TIMESTAMP
);

-- Role table
CREATE TABLE Roles (
    Id SERIAL PRIMARY KEY,
    Role VARCHAR(50) UNIQUE,
    Description TEXT
);

-- UserRole table
CREATE TABLE UserRoles (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER REFERENCES Users(Id),
    RoleId INTEGER REFERENCES Roles(Id),
    UNIQUE(UserId, RoleId)
);

-- Language table
CREATE TABLE Languages (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(50) UNIQUE
);

-- Book table
CREATE TABLE Books (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER REFERENCES Users(Id),
    BookName VARCHAR(255) NOT NULL,
    AuthorName VARCHAR(255) NOT NULL,
    TranslatorName VARCHAR(255),
    ReleasedDate INTEGER NOT NULL,
    BookCoverImage TEXT,
    Price DECIMAL(10,2) NOT NULL,
    Description TEXT,
    NumberOfPages INTEGER,
    LanguageId INTEGER REFERENCES Languages(Id),
    IsDelete BOOLEAN NOT NULL,
    ReviewId INTEGER,
    ReviewAverage DECIMAL(2,1)
);

-- Review table
CREATE TABLE Reviews (
    Id SERIAL PRIMARY KEY,
    User_Id INTEGER REFERENCES Users(Id),
    Book_Id INTEGER REFERENCES Books(Id),
    Rating INTEGER CHECK (Rating BETWEEN 1 AND 5),
    CreatedAt TIMESTAMP
);

-- BookFiles table
CREATE TABLE BookFiles (
    Id SERIAL PRIMARY KEY,
    BookId INTEGER REFERENCES Books(Id),
    BookDemoFile TEXT,
    BookOriginalFile TEXT
);

-- UserBooks table
CREATE TABLE UserBooks (
    Id SERIAL PRIMARY KEY,
    BookId INTEGER REFERENCES Books(Id),
    UserId INTEGER REFERENCES Users(Id),
    BoughtTime TIMESTAMP
);

-- UserBookmarks table
CREATE TABLE UserBookmarks (
    Id SERIAL PRIMARY KEY,
    BookId INTEGER REFERENCES Books(Id),
    UserId INTEGER REFERENCES Users(Id),
    AddedTime TIMESTAMP,
    IsDelete BOOLEAN
);

-- Comments table
CREATE TABLE Comments (
    Id SERIAL PRIMARY KEY,
    BookId INTEGER REFERENCES Books(Id),
    UserId INTEGER REFERENCES Users(Id),
    Comment TEXT,
    IsDelete BOOLEAN,
    CreatedDate TIMESTAMP
);

-- Category table
CREATE TABLE Categories (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) UNIQUE
);

-- BookCategory table
CREATE TABLE BookCategories (
    Id SERIAL PRIMARY KEY,
    CategoryId INTEGER REFERENCES Categories(Id),
    BookId INTEGER REFERENCES Books(Id),
    IsDelete BOOLEAN
);

-- WalletActionType table
CREATE TABLE WalletActionTypes (
    Id SERIAL PRIMARY KEY,
    ActionType VARCHAR(255) UNIQUE
);

-- WalletAction table
CREATE TABLE WalletActions (
    Id SERIAL PRIMARY KEY,
    ActionTypeId INTEGER REFERENCES WalletActionTypes(Id),
    UserId INTEGER REFERENCES Users(Id),
    Amount DECIMAL(10,2),
    IsSuccessful BOOLEAN,
    Description TEXT,
    CreatedDate TIMESTAMP
);

-- Discount table
CREATE TABLE Discounts (
    Id SERIAL PRIMARY KEY,
    Code VARCHAR(255) UNIQUE,
    Quantity INTEGER,
    "Percent" DECIMAL(4,2),
    CreatedDate TIMESTAMP,
    ExpireDate TIMESTAMP,
    IsDelete BOOLEAN
);

-- UserDiscounts table
CREATE TABLE UserDiscounts (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER REFERENCES Users(Id),
    DiscountId INTEGER REFERENCES Discounts(Id)
);
 -- Insert data into Language
INSERT INTO Languages (Name) VALUES
('انگلیسی'),  -- English
('فارسی'),    -- Persian
('فرانسوی'),  -- French
('آلمانی'),   -- German
('ترکی استانبولی'), -- Turkish
('اسپانیایی'), -- Spanish
('ایتالیایی'), -- Italian
('روسی'),     -- Russian
('چینی'),     -- Chinese
('ژاپنی');    -- Japanese

-- Insert data into Role
INSERT INTO Roles (Role, Description) VALUES
('Admin', 'Administrator of the platform'),
('Publisher', 'Publishes books on the platform'),
('Customer', 'Buys and reviews books on the platform');

-- Insert data into Category
INSERT INTO Categories (Name) VALUES
('داستانی'),          -- Fiction
('غیر داستانی'),      -- Non-fiction
('زندگینامه'),        -- Biography
('طنز'),              -- Humor
('رمان'),             -- Novel
('کتاب آشپزی'),       -- CookBook
('هیجان انگیز'),      -- Thriller
('رازآلود'),          -- Mystery
('شعر'),              -- Poetry
('فلسفه'),            -- Philosophy
('علمی-تخیلی'),       -- Science-Fiction
('تاریخ'),            -- History
('خودیاری'),          -- Self-Help
('فانتزی'),           -- Fantasy
('عاشقانه');          -- Romance

-- Insert data into User
INSERT INTO Users (Username, Email, HashedPassword, IsActive, RegistrationDate, IsPublisher, IsConfirm)
VALUES 
('Ali', 'alitaami2002@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-01', FALSE, TRUE),
('Elyar', 'ElyarNejati@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-02', TRUE, TRUE),
('Admin', 'alitaami81@gmail.com', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', TRUE, '2023-01-03', FALSE, TRUE),
('John', 'john@example.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-04', TRUE, TRUE),
('Jane', 'jane@example.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-05', FALSE, TRUE),
('Robert', 'robert@example.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-06', TRUE, TRUE),
('Emily', 'emily@example.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-07', FALSE, TRUE),
('William', 'william@example.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE, '2023-01-08', TRUE, TRUE);

-- Insert data into UserRole
INSERT INTO UserRoles (UserId, RoleId)
VALUES 
(1, 3), -- Ali is a Customer
(2, 2), -- Elyar is a Publisher
(3, 1), -- Admin is an Admin
(4, 2), -- John is a Publisher
(5, 3), -- Jane is a Customer
(6, 2), -- Robert is a Publisher
(7, 3), -- Emily is a Customer
(8, 2); -- William is a Publisher

-- Insert data into Book
INSERT INTO Books (UserId, BookName, AuthorName, ReleasedDate, Price, NumberOfPages, LanguageId, IsDelete)
VALUES 
(2, 'جزیره مرموز', 'ژول ورن',1874, 100, 450, 1, FALSE),
(2, 'دور دنیا در 80 روز', 'ژول ورن', 1873,0, 250, 1, FALSE),
(4, 'ماجراجویی بزرگ', 'جان اسمیت', 2020,10000, 350, 2, FALSE),
(6, 'رازهای جهان', 'رابرت براون',2019,20000, 500, 3, FALSE),
(8, 'عشق در پاریس', 'ویلیام جانسون',2021, 14000, 300, 4, FALSE);

-- Insert data into Review
INSERT INTO Reviews (User_Id, Book_Id, Rating, CreatedAt)
VALUES 
(1, 1, 5, '2023-01-10 10:00:00'),
(3, 2, 4, '2023-01-15 14:00:00'),
(5, 3, 4, '2023-01-16 10:00:00'),
(7, 4, 5, '2023-01-17 11:00:00'),
(1, 5, 3, '2023-01-18 12:00:00');

-- Insert data into BookFiles
INSERT INTO BookFiles (BookId, BookDemoFile, BookOriginalFile)
VALUES 
(1, 'path_to_demo_file1', 'path_to_original_file1'),
(2, 'path_to_demo_file2', 'path_to_original_file2'),
(3, 'path_to_demo_file3', 'path_to_original_file3'),
(4, 'path_to_demo_file4', 'path_to_original_file4'),
(5, 'path_to_demo_file5', 'path_to_original_file5');

-- Insert data into UserBooks
INSERT INTO UserBooks (BookId, UserId, BoughtTime)
VALUES 
(1, 1, '2023-01-10 10:05:00'),
(2, 3, '2023-01-15 14:05:00'),
(3, 5, '2023-01-16 10:05:00'),
(4, 7, '2023-01-17 11:05:00'),
(5, 1, '2023-01-18 12:05:00');

-- Insert data into UserBookmarks
INSERT INTO UserBookmarks (BookId, UserId, AddedTime, IsDelete)
VALUES 
(1, 3, '2023-01-16 09:00:00', FALSE),
(2, 5, '2023-01-17 09:00:00', FALSE),
(3, 7, '2023-01-18 09:00:00', FALSE),
(4, 1, '2023-01-19 09:00:00', FALSE),
(5, 3, '2023-01-20 09:00:00', FALSE);

-- Insert data into Comments
INSERT INTO Comments (BookId, UserId, Comment, IsDelete, CreatedDate)
VALUES 
(1, 1, 'این کتاب را دوست داشتم!', FALSE, '2023-01-10'),
(2, 3, 'یک ماجراجویی هیجان انگیز!', FALSE, '2023-01-15'),
(3, 5, 'خواندنی عالی!', FALSE, '2023-01-16'),
(4, 7, 'بسیار توصیه می‌شود!', FALSE, '2023-01-17'),
(5, 1, 'به سلیقه من نبود.', FALSE, '2023-01-18');

-- Insert data into BookCategory
INSERT INTO BookCategories (CategoryId, BookId, IsDelete)
VALUES 
(1, 1, FALSE), -- The Mysterious Island is Fiction
(2, 2, FALSE), -- Around the World in 80 Days is Non-fiction
(1, 3, FALSE), -- The Great Adventure is Fiction
(7, 4, FALSE), -- Mysteries of the Universe is Thriller
(15, 5, FALSE); -- Romance in Paris is Romance

-- Insert data into WalletActionType
INSERT INTO WalletActionTypes (ActionType)
VALUES 
('Deposit'),
('Withdraw');
 
-- Insert data into WalletAction
INSERT INTO WalletActions (ActionTypeId, UserId, Amount, IsSuccessful, Description, CreatedDate)
VALUES 
(1, 1, 100.00, TRUE, 'Deposited $100', '2023-01-09'),
(2, 1, 15.99, TRUE, 'Purchased "The Mysterious Island"', '2023-01-10'),
(1, 3, 200.00, TRUE, 'Deposited $200', '2023-01-15'),
(2, 3, 12.99, TRUE, 'Purchased "Around the World in 80 Days"', '2023-01-15'),
(1, 5, 150.00, TRUE, 'Deposited $150', '2023-01-16');

-- Insert data into Discount
INSERT INTO Discounts (Code, Quantity, "Percent", CreatedDate, ExpireDate, IsDelete)
VALUES 
('DISCOUNT10', 100, 10.00, '2023-01-01', '2023-12-31', FALSE),
('DISCOUNT15', 50, 15.00, '2023-01-01', '2023-06-30', FALSE),
('DISCOUNT20', 75, 20.00, '2023-01-01', '2023-12-31', FALSE);

-- Insert data into UserDiscounts
INSERT INTO UserDiscounts (UserId, DiscountId)
VALUES 
(1, 1),
(3, 2),
(5, 3);