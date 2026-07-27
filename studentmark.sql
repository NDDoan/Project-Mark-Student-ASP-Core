USE [master]
GO

-- =====================================================
-- BƯỚC 1: Xóa và tạo lại Database (dùng thư mục mặc định của SQL Server)
-- =====================================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ProjectStudentMark')
BEGIN
    ALTER DATABASE [ProjectStudentMark] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [ProjectStudentMark]
END
GO

-- Không chỉ định đường dẫn file → SQL Server tự dùng thư mục mặc định có quyền ghi
CREATE DATABASE [ProjectStudentMark]
GO

-- =====================================================
-- BƯỚC 2: Chuyển sang database vừa tạo
-- =====================================================
USE [ProjectStudentMark]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================================
-- BƯỚC 3: Tạo các bảng (đúng thứ tự: bảng cha trước, bảng con sau)
-- =====================================================

-- Bảng Roles (không phụ thuộc bảng nào)
CREATE TABLE [dbo].[Roles] (
    [RoleId]   INT           IDENTITY(1,1) NOT NULL,
    [RoleName] NVARCHAR(50)  NOT NULL,
    CONSTRAINT [PK__Roles__8AFACE1A85828A51] PRIMARY KEY CLUSTERED ([RoleId] ASC)
) ON [PRIMARY]
GO

-- Bảng Subjects (không phụ thuộc bảng nào)
CREATE TABLE [dbo].[Subjects] (
    [SubjectId]   INT            IDENTITY(1,1) NOT NULL,
    [SubjectName] NVARCHAR(100)  NOT NULL,
    [Description] NVARCHAR(MAX)  NULL,
    CONSTRAINT [PK__Subjects__AC1BA3A8F5EAC07C] PRIMARY KEY CLUSTERED ([SubjectId] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Bảng Students (không phụ thuộc bảng nào)
CREATE TABLE [dbo].[Students] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [Email]       NVARCHAR(100)  NOT NULL,
    [RollNumber]  NVARCHAR(8)    NOT NULL,
    [FirstName]   NVARCHAR(100)  NOT NULL,
    [LastName]    NVARCHAR(100)  NOT NULL,
    [Dob]         DATE           NULL,
    [Address]     NVARCHAR(255)  NULL,
    [PhoneNumber] NVARCHAR(20)   NULL,
    [Gender]      NVARCHAR(10)   NULL,
    [CreatedAt]   DATETIME       NULL CONSTRAINT [DF_Students_CreatedAt] DEFAULT (GETDATE()),
    [UpdatedAt]   DATETIME       NULL CONSTRAINT [DF_Students_UpdatedAt] DEFAULT (GETDATE()),
    CONSTRAINT [PK__Students__3214EC0722EF27E7] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Bảng Users (phụ thuộc Roles)
CREATE TABLE [dbo].[Users] (
    [UserId]       INT            IDENTITY(1,1) NOT NULL,
    [Username]     VARCHAR(50)    NOT NULL,
    [FirstName]    NVARCHAR(100)  NOT NULL,
    [LastName]     NVARCHAR(100)  NOT NULL,
    [Address]      NVARCHAR(255)  NULL,
    [PhoneNumber]  NVARCHAR(20)   NULL,
    [Gender]       NVARCHAR(10)   NULL,
    [AvatarUrl]    NVARCHAR(255)  NULL,
    [PasswordHash] VARCHAR(255)   NOT NULL,
    [RoleId]       INT            NOT NULL,
    [CreatedAt]    DATETIME       NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (GETDATE()),
    [UpdatedAt]    DATETIME       NULL CONSTRAINT [DF_Users_UpdatedAt] DEFAULT (GETDATE()),
    CONSTRAINT [PK__Users__1788CC4CBD78C642] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
) ON [PRIMARY]
GO

-- Bảng Courses (phụ thuộc Subjects, Users)
CREATE TABLE [dbo].[Courses] (
    [CourseId]  INT            IDENTITY(1,1) NOT NULL,
    [Title]     NVARCHAR(100)  NOT NULL,
    [SubjectId] INT            NOT NULL,
    [TeacherId] INT            NULL,
    [StartDate] DATE           NULL,
    CONSTRAINT [PK__Courses__C92D71A722E9F6F6] PRIMARY KEY CLUSTERED ([CourseId] ASC),
    CONSTRAINT [FK_Course_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([SubjectId]),
    CONSTRAINT [FK_Course_Teacher] FOREIGN KEY ([TeacherId]) REFERENCES [dbo].[Users] ([UserId])
) ON [PRIMARY]
GO

-- Bảng GradeItems (phụ thuộc Subjects)
CREATE TABLE [dbo].[GradeItems] (
    [GradeItemId] INT            IDENTITY(1,1) NOT NULL,
    [Title]       NVARCHAR(100)  NOT NULL,
    [Rate]        DECIMAL(5,2)   NOT NULL,
    [SubjectId]   INT            NOT NULL,
    CONSTRAINT [PK__GradeIte__A40A40361FBC6B9C] PRIMARY KEY CLUSTERED ([GradeItemId] ASC),
    CONSTRAINT [FK_GradeItem_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects] ([SubjectId])
) ON [PRIMARY]
GO

-- Bảng StudentCourses (phụ thuộc Students, Courses)
CREATE TABLE [dbo].[StudentCourses] (
    [StudentId] INT NOT NULL,
    [CourseId]  INT NOT NULL,
    CONSTRAINT [PK__StudentC__5E57FC83A69162C6] PRIMARY KEY CLUSTERED ([StudentId] ASC, [CourseId] ASC),
    CONSTRAINT [FK_StudentCourse_Student] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_StudentCourse_Course]  FOREIGN KEY ([CourseId])  REFERENCES [dbo].[Courses] ([CourseId])
) ON [PRIMARY]
GO

-- Bảng Marks (phụ thuộc StudentCourses, GradeItems)
CREATE TABLE [dbo].[Marks] (
    [MarkId]      INT          IDENTITY(1,1) NOT NULL,
    [StudentId]   INT          NOT NULL,
    [CourseId]    INT          NOT NULL,
    [GradeItemId] INT          NOT NULL,
    [Value]       DECIMAL(5,2) NOT NULL,
    CONSTRAINT [PK__Marks__4E30D366C0BE9587] PRIMARY KEY CLUSTERED ([MarkId] ASC),
    CONSTRAINT [FK_Mark_GradeItem]    FOREIGN KEY ([GradeItemId])           REFERENCES [dbo].[GradeItems] ([GradeItemId]),
    CONSTRAINT [FK_Mark_StudentCourse] FOREIGN KEY ([StudentId], [CourseId]) REFERENCES [dbo].[StudentCourses] ([StudentId], [CourseId])
) ON [PRIMARY]
GO

-- =====================================================
-- BƯỚC 4: Tạo Index UNIQUE
-- =====================================================
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Roles__8A2B61604D47F2C0]    ON [dbo].[Roles]    ([RoleName] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Subjects__4C5A7D55E87C4FEF] ON [dbo].[Subjects] ([SubjectName] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Students__A9D105340FBE4CB9] ON [dbo].[Students] ([Email] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Students__E9F06F16913E41A1] ON [dbo].[Students] ([RollNumber] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E47F3304C1]    ON [dbo].[Users]    ([Username] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Marks__69F3F6C23F4C47E3]    ON [dbo].[Marks]    ([StudentId] ASC, [CourseId] ASC, [GradeItemId] ASC)
GO

-- =====================================================
-- BƯỚC 5: Chèn dữ liệu (đúng thứ tự theo FK)
-- =====================================================

-- 5.1 Roles
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (2, N'Quản Lý')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (3, N'Giáo Viên')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO

-- 5.2 Subjects
SET IDENTITY_INSERT [dbo].[Subjects] ON
INSERT [dbo].[Subjects] ([SubjectId], [SubjectName], [Description])
VALUES (1, N'PRN232', N'Building Cross-Platform Back-End Application With .NET')
SET IDENTITY_INSERT [dbo].[Subjects] OFF
GO

-- 5.3 Students
SET IDENTITY_INSERT [dbo].[Students] ON
INSERT [dbo].[Students] ([Id], [Email], [RollNumber], [FirstName], [LastName], [Dob], [Address], [PhoneNumber], [Gender], [CreatedAt], [UpdatedAt])
VALUES (1, N'nguyenkhanh@gmail.com', N'HE160000', N'Duy Khánh', N'Nguyễn',
        CAST(N'2001-11-11' AS Date), N'Phú yên, Hà Nội', N'0669988554', N'Nam',
        CAST(N'2026-07-23T15:12:29.197' AS DateTime), CAST(N'2026-07-25T00:00:05.170' AS DateTime))
SET IDENTITY_INSERT [dbo].[Students] OFF
GO

-- 5.4 Users (phụ thuộc Roles)
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT [dbo].[Users] ([UserId], [Username], [FirstName], [LastName], [Address], [PhoneNumber], [Gender], [AvatarUrl], [PasswordHash], [RoleId], [CreatedAt], [UpdatedAt])
VALUES (3, N'Administrator01', N'Admin', N'Anh', N'Hà Nội, Việt Nam', N'0987654321', N'Nam',
        N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTsP5JMpKWNwknnv7SGoO43CGmPXb4GJZcL6XC7-0hJ7A&s=10',
        N'$2a$12$krlhNkKia/yx0CW9GdfuzObeVXvMr74lvt2390PUSC1toGmEC1DJS', 1,
        CAST(N'2026-07-23T10:38:52.323' AS DateTime), CAST(N'2026-07-23T10:38:52.323' AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [FirstName], [LastName], [Address], [PhoneNumber], [Gender], [AvatarUrl], [PasswordHash], [RoleId], [CreatedAt], [UpdatedAt])
VALUES (4, N'Administrator02', N'Sư', N'Pháp', N'Nà ná anh Pháp Sư Độ', N'0123456789', N'Nam',
        N'https://images-na.ssl-images-amazon.com/images/I/61n5StLylWL.jpg',
        N'ma_hoa_mat_khau_456', 1,
        CAST(N'2026-07-23T10:38:52.323' AS DateTime), CAST(N'2026-07-23T10:38:52.323' AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [FirstName], [LastName], [Address], [PhoneNumber], [Gender], [AvatarUrl], [PasswordHash], [RoleId], [CreatedAt], [UpdatedAt])
VALUES (5, N'supermanager', N'Đại', N'Quản Lý', NULL, N'0456789123', NULL,
        N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPTtNjZzR7FDrx2YuqByoZJkLjPp6v0gWEGAbx-g_y4Q&s=10',
        N'$2a$11$eb0HT.eu0OgGhVeVmyS9V.8.mN8gikGu.Sx7yCmgEVjHPHUNAOteG', 2,
        CAST(N'2026-07-23T11:10:42.253' AS DateTime), CAST(N'2026-07-23T11:10:42.253' AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [FirstName], [LastName], [Address], [PhoneNumber], [Gender], [AvatarUrl], [PasswordHash], [RoleId], [CreatedAt], [UpdatedAt])
VALUES (6, N'superteacher', N'Đại', N'Giáo Viên', N'Thanh hóa', N'0784512469', N'Nam', N'',
        N'giaovien', 3,
        CAST(N'2026-07-23T13:45:54.417' AS DateTime), CAST(N'2026-07-24T23:41:29.977' AS DateTime))
INSERT [dbo].[Users] ([UserId], [Username], [FirstName], [LastName], [Address], [PhoneNumber], [Gender], [AvatarUrl], [PasswordHash], [RoleId], [CreatedAt], [UpdatedAt])
VALUES (7, N'megateacher', N'Giáo Viên', N'Mega', NULL, N'0785566545', NULL, NULL,
        N'$2a$11$BrCnxiScit7M.yS1a0hkWOLa9oZ8QDtWayqPc2mYQ9JwnoNA3b9zy', 3,
        CAST(N'2026-07-23T16:53:43.960' AS DateTime), CAST(N'2026-07-23T16:53:43.960' AS DateTime))
SET IDENTITY_INSERT [dbo].[Users] OFF
GO

-- 5.5 Courses (phụ thuộc Subjects + Users)
SET IDENTITY_INSERT [dbo].[Courses] ON
INSERT [dbo].[Courses] ([CourseId], [Title], [SubjectId], [TeacherId], [StartDate])
VALUES (1, N'SE1900', 1, 6, NULL)
SET IDENTITY_INSERT [dbo].[Courses] OFF
GO

-- 5.6 GradeItems (phụ thuộc Subjects)
SET IDENTITY_INSERT [dbo].[GradeItems] ON
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (1, N'Assignment 1', CAST(5.00  AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (2, N'Assignment 2', CAST(5.00  AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (3, N'PT1',          CAST(5.00  AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (4, N'PT2',          CAST(5.00  AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (6, N'Project',      CAST(25.00 AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (7, N'PE',           CAST(25.00 AS Decimal(5,2)), 1)
INSERT [dbo].[GradeItems] ([GradeItemId], [Title], [Rate], [SubjectId]) VALUES (8, N'FE',           CAST(30.00 AS Decimal(5,2)), 1)
SET IDENTITY_INSERT [dbo].[GradeItems] OFF
GO

-- 5.7 StudentCourses (phụ thuộc Students + Courses)
INSERT [dbo].[StudentCourses] ([StudentId], [CourseId]) VALUES (1, 1)
GO

-- 5.8 Marks (phụ thuộc StudentCourses + GradeItems)
SET IDENTITY_INSERT [dbo].[Marks] ON
INSERT [dbo].[Marks] ([MarkId], [StudentId], [CourseId], [GradeItemId], [Value]) VALUES (1, 1, 1, 1, CAST(7.00 AS Decimal(5,2)))
INSERT [dbo].[Marks] ([MarkId], [StudentId], [CourseId], [GradeItemId], [Value]) VALUES (2, 1, 1, 4, CAST(8.00 AS Decimal(5,2)))
INSERT [dbo].[Marks] ([MarkId], [StudentId], [CourseId], [GradeItemId], [Value]) VALUES (3, 1, 1, 8, CAST(5.00 AS Decimal(5,2)))
SET IDENTITY_INSERT [dbo].[Marks] OFF
GO

-- =====================================================
USE [master]
GO
PRINT 'Database [ProjectStudentMark] da duoc tao thanh cong!'
GO
