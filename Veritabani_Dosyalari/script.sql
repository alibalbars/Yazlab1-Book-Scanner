USE [master]
GO
/****** Object:  Database [YazlabDb]    Script Date: 1.05.2020 23:00:08 ******/
CREATE DATABASE [YazlabDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'YazlabDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\YazlabDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'YazlabDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\YazlabDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [YazlabDb] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [YazlabDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [YazlabDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [YazlabDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [YazlabDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [YazlabDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [YazlabDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [YazlabDb] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [YazlabDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [YazlabDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [YazlabDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [YazlabDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [YazlabDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [YazlabDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [YazlabDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [YazlabDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [YazlabDb] SET  ENABLE_BROKER 
GO
ALTER DATABASE [YazlabDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [YazlabDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [YazlabDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [YazlabDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [YazlabDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [YazlabDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [YazlabDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [YazlabDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [YazlabDb] SET  MULTI_USER 
GO
ALTER DATABASE [YazlabDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [YazlabDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [YazlabDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [YazlabDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [YazlabDb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [YazlabDb] SET QUERY_STORE = OFF
GO
USE [YazlabDb]
GO
/****** Object:  Table [dbo].[Books]    Script Date: 1.05.2020 23:00:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Books](
	[Book_Id] [int] IDENTITY(1,1) NOT NULL,
	[Book_Name] [varchar](max) NULL,
	[Book_Isbn] [varchar](max) NULL,
	[IsCurrent] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Book_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DateShiftLogs]    Script Date: 1.05.2020 23:00:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateShiftLogs](
	[Log_Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_DateShiftLogs] PRIMARY KEY CLUSTERED 
(
	[Log_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Loans]    Script Date: 1.05.2020 23:00:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loans](
	[Loan_Id] [int] IDENTITY(1,1) NOT NULL,
	[Book_Id] [int] NULL,
	[User_Id] [int] NULL,
	[Deliver_Date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Loan_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1.05.2020 23:00:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[User_Id] [int] IDENTITY(1,1) NOT NULL,
	[First_Name] [varchar](50) NULL,
	[Last_Name] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[IsCurrent] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Books] ON 

INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (69, N'kitap3', N'9781234567897', 0)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (70, N'kitap5', N'9781234567897', 0)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (71, N'kitap6', N'9783161484100', 0)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (72, N'kitap7', N'9788492808274', 1)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (73, N'kitap8', N'9780733426094', 0)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (75, N'kitap12', N'9780955716300', 0)
INSERT [dbo].[Books] ([Book_Id], [Book_Name], [Book_Isbn], [IsCurrent]) VALUES (76, N'kitap13', N'9788492808274', 0)
SET IDENTITY_INSERT [dbo].[Books] OFF
SET IDENTITY_INSERT [dbo].[DateShiftLogs] ON 

INSERT [dbo].[DateShiftLogs] ([Log_Id], [Date]) VALUES (1, CAST(N'2020-01-01' AS Date))
SET IDENTITY_INSERT [dbo].[DateShiftLogs] OFF
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([User_Id], [First_Name], [Last_Name], [Password], [IsCurrent]) VALUES (1, N'Ali', N'Balbars', N'1', 0)
INSERT [dbo].[Users] ([User_Id], [First_Name], [Last_Name], [Password], [IsCurrent]) VALUES (2, N'Berke', N'Sayın', N'1', 0)
INSERT [dbo].[Users] ([User_Id], [First_Name], [Last_Name], [Password], [IsCurrent]) VALUES (3, N'Mustafa', N'Tüysüz', N'1', 0)
INSERT [dbo].[Users] ([User_Id], [First_Name], [Last_Name], [Password], [IsCurrent]) VALUES (4, N'Mustafa', N'Topaloğlu', N'1', 0)
INSERT [dbo].[Users] ([User_Id], [First_Name], [Last_Name], [Password], [IsCurrent]) VALUES (5, N'Hakkı', N'Bulut', N'1', 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
USE [master]
GO
ALTER DATABASE [YazlabDb] SET  READ_WRITE 
GO
