USE [master]
GO
/****** Object:  Database [KeyManager]    Script Date: 30/06/2022 15:04:02 ******/
CREATE DATABASE [KeyManager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KeyManager', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.KKRR_SQL\MSSQL\DATA\KeyManager.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'KeyManager_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.KKRR_SQL\MSSQL\DATA\KeyManager_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [KeyManager] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KeyManager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KeyManager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KeyManager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KeyManager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KeyManager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KeyManager] SET ARITHABORT OFF 
GO
ALTER DATABASE [KeyManager] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KeyManager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KeyManager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KeyManager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KeyManager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KeyManager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KeyManager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KeyManager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KeyManager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KeyManager] SET  DISABLE_BROKER 
GO
ALTER DATABASE [KeyManager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KeyManager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KeyManager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KeyManager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KeyManager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KeyManager] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KeyManager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KeyManager] SET RECOVERY FULL 
GO
ALTER DATABASE [KeyManager] SET  MULTI_USER 
GO
ALTER DATABASE [KeyManager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KeyManager] SET DB_CHAINING OFF 
GO
ALTER DATABASE [KeyManager] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [KeyManager] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [KeyManager] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [KeyManager] SET QUERY_STORE = OFF
GO
USE [KeyManager]
GO
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [KeyManager]
GO
/****** Object:  Table [dbo].[activitylog]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[activitylog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Time] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[What] [nvarchar](2500) NOT NULL,
 CONSTRAINT [PK_activitylog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customer]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[KTNumber] [nvarchar](10) NOT NULL,
	[CustomerCode] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[SummPNumber] [nvarchar](10) NOT NULL,
	[StatusCheck] [int] NOT NULL,
 CONSTRAINT [PK_customer_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ecode]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ecode](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ECode] [nvarchar](44) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[ECodeServiceCardID] [int] NOT NULL,
 CONSTRAINT [PK_ecode_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[group]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[group](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[PNumber] [nvarchar](10) NULL,
	[CreationDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ECodeID] [int] NOT NULL,
	[UCodeID] [int] NOT NULL,
	[StatusCheck] [int] NOT NULL,
 CONSTRAINT [PK_group] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ucode]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ucode](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UCode] [nvarchar](44) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[UCodeServiceCardID] [int] NOT NULL,
 CONSTRAINT [PK_ucode] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](10) NOT NULL,
	[Password] [nvarchar](60) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[DeletedDate] [datetime] NULL,
	[UserTypeID] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[StatusCheck] [int] NOT NULL,
 CONSTRAINT [PK_user] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usertype]    Script Date: 30/06/2022 15:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usertype](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserType] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[user] ON 
INSERT [dbo].[user] ([ID], [UserName], [Password], [CreationDate], [DeletedDate], [UserTypeID], [IsDeleted], [StatusCheck]) VALUES (1, N'Admin', N'$2b$11$XXuZN3WZ7TE4.XJNZorHLeBjWLUqGuV8b48NiY7reZLG1lIocadxW', CAST(N'2022-06-25T00:00:00.000' AS DateTime), NULL, 1, 0, 1)
SET IDENTITY_INSERT [dbo].[user] OFF

SET IDENTITY_INSERT [dbo].[usertype] ON 
INSERT [dbo].[usertype] ([ID], [UserType]) VALUES (1, N'Admin')
INSERT [dbo].[usertype] ([ID], [UserType]) VALUES (2, N'Technik')
INSERT [dbo].[usertype] ([ID], [UserType]) VALUES (3, N'Sachbearbeiter')
SET IDENTITY_INSERT [dbo].[usertype] OFF

SET IDENTITY_INSERT [dbo].[customer] ON
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (1, N'KT123456', N'AB12', N'Firma Test AG', CAST(N'2022-06-01T11:51:00.000' AS DateTime), NULL, 1, N'P123456789', 1)
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (2, N'KT654321', N'BA21', N'Firma Test GmbH', CAST(N'2022-06-01T11:00:00.000' AS DateTime), NULL, 0, N'P987654321', 1)
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (3, N'KT162534', N'BC34', N'TestFirma', CAST(N'2022-06-01T12:40:00.000' AS DateTime), NULL, 1, N'P192837465', 1)
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (8, N'KT956742', N'ZF85', N'Test Gmbh', CAST(N'2022-06-01T12:40:00.000' AS DateTime), NULL, 0, N'P876549231', 1)
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (11, N'svvvswcsw', N'svvvswcsw', N'hdjd', CAST(N'2022-06-01T15:55:21.000' AS DateTime), NULL, 0, N'0', 1)
INSERT [dbo].[customer] ([ID], [KTNumber], [CustomerCode], [Name], [CreationDate], [DeletedDate], [IsDeleted], [SummPNumber], [StatusCheck] ) VALUES (12, N'sdfgsklh', N'sdfgsklh', N'hsdkvhkl', CAST(N'2022-06-01T16:02:32.000' AS DateTime), NULL, 0, N'0', 1)
SET IDENTITY_INSERT [dbo].[customer] OFF

SET IDENTITY_INSERT [dbo].[ecode] ON 
INSERT [dbo].[ecode] ([ID], [ECode], [CreationDate], [DeletedDate], [IsDeleted], [ECodeServiceCardID]) VALUES (1, N'ABCDEFG1', CAST(N'2018-02-13T11:51:00.000' AS DateTime), NULL, 1, 1)
INSERT [dbo].[ecode] ([ID], [ECode], [CreationDate], [DeletedDate], [IsDeleted], [ECodeServiceCardID]) VALUES (2, N'GFEDCBA2', CAST(N'2018-02-13T11:00:00.000' AS DateTime), NULL, 1, 2)
SET IDENTITY_INSERT [dbo].[ecode] OFF

SET IDENTITY_INSERT [dbo].[group] ON 
INSERT [dbo].[group] ([ID], [Name], [PNumber], [CreationDate], [DeletedDate], [IsDeleted], [CustomerID], [ECodeID], [UCodeID], [StatusCheck] ) VALUES (3, N'Region X', N'P12345', CAST(N'2022-06-01T11:51:00.000' AS DateTime), NULL, 1, 1, 1, 1, 1)
INSERT [dbo].[group] ([ID], [Name], [PNumber], [CreationDate], [DeletedDate], [IsDeleted], [CustomerID], [ECodeID], [UCodeID], [StatusCheck] ) VALUES (4, N'Region SO', N'P54321', CAST(N'2022-06-01T11:00:00.000' AS DateTime), NULL, 1, 2, 2, 2, 1)
INSERT [dbo].[group] ([ID], [Name], [PNumber], [CreationDate], [DeletedDate], [IsDeleted], [CustomerID], [ECodeID], [UCodeID], [StatusCheck] ) VALUES (5, N'Region BE', N'P51423', CAST(N'2022-06-01T15:28:00.000' AS DateTime), NULL, 1, 3, 2, 2, 1)
INSERT [dbo].[group] ([ID], [Name], [PNumber], [CreationDate], [DeletedDate], [IsDeleted], [CustomerID], [ECodeID], [UCodeID], [StatusCheck] ) VALUES (6, N'Region LU', N'P65785', CAST(N'2022-06-01T12:40:00.000' AS DateTime), NULL, 0, 1, 2, 1, 1)
INSERT [dbo].[group] ([ID], [Name], [PNumber], [CreationDate], [DeletedDate], [IsDeleted], [CustomerID], [ECodeID], [UCodeID], [StatusCheck] ) VALUES (10, N'Region LUX', N'P657857', CAST(N'2022-06-01T12:40:00.000' AS DateTime), NULL, 0, 1, 2, 1, 1)
SET IDENTITY_INSERT [dbo].[group] OFF

SET IDENTITY_INSERT [dbo].[ucode] ON 
INSERT [dbo].[ucode] ([ID], [UCode], [CreationDate], [DeletedDate], [IsDeleted], [UCodeServiceCardID]) VALUES (1, N'BCDEFGH1', CAST(N'2022-06-01T11:51:00.000' AS DateTime), NULL, 1, 1)
INSERT [dbo].[ucode] ([ID], [UCode], [CreationDate], [DeletedDate], [IsDeleted], [UCodeServiceCardID]) VALUES (2, N'HGFEDCB2', CAST(N'2022-06-01T11:00:00.000' AS DateTime), NULL, 1, 2)
SET IDENTITY_INSERT [dbo].[ucode] OFF

/****** Object:  Index [IX_customer_CreationDate]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_customer_CreationDate] ON [dbo].[customer]
(
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_customer_CustomerCode]    Script Date: 30/06/2022 15:04:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_customer_CustomerCode] ON [dbo].[customer]
(
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_customer_DeletedDate]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_customer_DeletedDate] ON [dbo].[customer]
(
	[DeletedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_customer_KTNumber]    Script Date: 30/06/2022 15:04:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_customer_KTNumber] ON [dbo].[customer]
(
	[KTNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_customer_Name]    Script Date: 30/06/2022 15:04:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_customer_Name] ON [dbo].[customer]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_customer_Status]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_customer_Status] ON [dbo].[customer]
(
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ecode_ServiceCardID]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_ecode_ServiceCardID] ON [dbo].[ecode]
(
	[ECodeServiceCardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_group_CreationDate]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_group_CreationDate] ON [dbo].[group]
(
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_group_DeletedDate]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_group_DeletedDate] ON [dbo].[group]
(
	[DeletedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_group_Name]    Script Date: 30/06/2022 15:04:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_group_Name] ON [dbo].[group]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_group_PNumber]    Script Date: 30/06/2022 15:04:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_group_PNumber] ON [dbo].[group]
(
	[PNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_group_Status]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_group_Status] ON [dbo].[group]
(
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ucode_ServiceCardID]    Script Date: 30/06/2022 15:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_ucode_ServiceCardID] ON [dbo].[ucode]
(
	[UCodeServiceCardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[activitylog]  WITH CHECK ADD  CONSTRAINT [FK_activitylog_user] FOREIGN KEY([UserID])
REFERENCES [dbo].[user] ([ID])
GO
ALTER TABLE [dbo].[activitylog] CHECK CONSTRAINT [FK_activitylog_user]
GO
ALTER TABLE [dbo].[group]  WITH CHECK ADD  CONSTRAINT [FK_group_customerID] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[customer] ([ID])
GO
ALTER TABLE [dbo].[group] CHECK CONSTRAINT [FK_group_customerID]
GO
ALTER TABLE [dbo].[group]  WITH CHECK ADD  CONSTRAINT [FK_group_ecode] FOREIGN KEY([ECodeID])
REFERENCES [dbo].[ecode] ([ID])
GO
ALTER TABLE [dbo].[group] CHECK CONSTRAINT [FK_group_ecode]
GO
ALTER TABLE [dbo].[group]  WITH CHECK ADD  CONSTRAINT [FK_group_ucode] FOREIGN KEY([UCodeID])
REFERENCES [dbo].[ucode] ([ID])
GO
ALTER TABLE [dbo].[group] CHECK CONSTRAINT [FK_group_ucode]
GO
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_usertype] FOREIGN KEY([UserTypeID])
REFERENCES [dbo].[usertype] ([ID])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_user_usertype]
GO
USE [master]
GO
ALTER DATABASE [KeyManager] SET  READ_WRITE 
GO
