/****** Object:  Database [formdesigner-cpro-ch-qa]    Script Date: 5/13/2025 2:26:16 AM ******/
CREATE DATABASE [formdesigner-cpro-ch-qa]
(EDITION = 'Standard', SERVICE_OBJECTIVE = 'ElasticPool', MAXSIZE = 250 GB)
WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET COMPATIBILITY_LEVEL = 170
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ARITHABORT OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET  MULTI_USER 
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET ENCRYPTION ON
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET QUERY_STORE = ON
GO
ALTER DATABASE [formdesigner-cpro-ch-qa] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Schema [formdesigner-qa]    Script Date: 5/13/2025 2:26:16 AM ******/
CREATE SCHEMA [formdesigner-qa]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory]
(
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Designer]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Designer]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [DesignerId] [int] NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    CONSTRAINT [PK_Designer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DesignerHistory]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DesignerHistory]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [DesignerHistoryId] [int] NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    [FormVersion] [int] NOT NULL,
    CONSTRAINT [PK_DesignerHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormDatas]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormDatas]
(
    [Id] [nvarchar](450) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [DocumentId] [nvarchar](max) NOT NULL,
    [Status] [nvarchar](max) NOT NULL,
    [TenantId] [int] NOT NULL,
    [FormId] [nvarchar](max) NOT NULL,
    [TenantName] [nvarchar](max) NULL,
    [StorageUrl] [nvarchar](max) NULL,
    [SubmittedDate] [datetime2](7) NOT NULL,
    [Version] [int] NOT NULL,
    CONSTRAINT [PK_FormDatas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormDesigns]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormDesigns]
(
    [Id] [nvarchar](450) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [TenantId] [int] NOT NULL,
    [FormId] [int] NOT NULL,
    [TenantName] [nvarchar](max) NOT NULL,
    [StorageUrl] [nvarchar](max) NULL,
    [Version] [int] NOT NULL,
    [CreatedBy] [nvarchar](max) NULL,
    [IsActive] [bit] NULL,
    [Tags] [nvarchar](max) NULL,
    [DateCreated] [datetimeoffset](7) NOT NULL,
    [DateUpdated] [datetimeoffset](7) NULL,
    CONSTRAINT [PK_FormDesigns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormDesignsHistory]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormDesignsHistory]
(
    [Id] [nvarchar](450) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [TenantId] [int] NOT NULL,
    [FormId] [int] NOT NULL,
    [TenantName] [nvarchar](max) NOT NULL,
    [StorageUrl] [nvarchar](max) NOT NULL,
    [Version] [int] NOT NULL,
    [CreatedBy] [nvarchar](max) NULL,
    [FormDesignId] [nvarchar](50) NOT NULL,
    [DateCreated] [datetimeoffset](7) NOT NULL,
    [DateUpdated] [datetimeoffset](7) NULL,
    CONSTRAINT [PK_FormDesignsHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormStatesConfig]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormStatesConfig]
(
    [Label] [nvarchar](max) NOT NULL,
    [Value] [nvarchar](max) NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    [FormStatesConfigId] [int] IDENTITY(1,1) NOT NULL,
    CONSTRAINT [PK_FormStatesConfig] PRIMARY KEY CLUSTERED 
(
	[FormStatesConfigId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormStatesConfigHistory]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormStatesConfigHistory]
(
    [FormStatesConfigHistoryId] [int] IDENTITY(1,1) NOT NULL,
    [Label] [nvarchar](max) NOT NULL,
    [Value] [nvarchar](max) NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    [FormVersion] [int] NOT NULL,
    CONSTRAINT [PK_FormStatesConfigHistory] PRIMARY KEY CLUSTERED 
(
	[FormStatesConfigHistoryId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormTemplates]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormTemplates]
(
    [Id] [nvarchar](450) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [TenantId] [int] NOT NULL,
    [TenantName] [nvarchar](max) NULL,
    [StorageUrl] [nvarchar](max) NULL,
    [Version] [int] NOT NULL,
    CONSTRAINT [PK_FormTemplates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Processor]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Processor]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ProcessorId] [int] NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    CONSTRAINT [PK_Processor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessorHistory]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessorHistory]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ProcessorHistoryId] [int] NOT NULL,
    [FormDesignId] [nvarchar](450) NOT NULL,
    [FormVersion] [int] NOT NULL,
    CONSTRAINT [PK_ProcessorHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 5/13/2025 2:26:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users]
(
    [Id] [uniqueidentifier] NOT NULL,
    [FirstName] [nvarchar](20) NULL,
    [LastName] [nvarchar](20) NULL,
    [Email] [nvarchar](50) NOT NULL,
    [Role] [int] NOT NULL,
    [CreatedBy] [int] NULL,
    [DateCreated] [datetimeoffset](7) NOT NULL,
    [DateDeleted] [datetimeoffset](7) NULL,
    [DateUpdated] [datetimeoffset](7) NULL,
    [DeletedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FormStatesConfig_FormDesignId]    Script Date: 5/13/2025 2:26:16 AM ******/
CREATE NONCLUSTERED INDEX [IX_FormStatesConfig_FormDesignId] ON [dbo].[FormStatesConfig]
(
	[FormDesignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FormStatesConfigHistory_FormDesignId]    Script Date: 5/13/2025 2:26:16 AM ******/
CREATE NONCLUSTERED INDEX [IX_FormStatesConfigHistory_FormDesignId] ON [dbo].[FormStatesConfigHistory]
(
	[FormDesignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 5/13/2025 2:26:16 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DesignerHistory] ADD  DEFAULT ((0)) FOR [FormVersion]
GO
ALTER TABLE [dbo].[FormDatas] ADD  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[FormDesigns] ADD  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[FormDesigns] ADD  DEFAULT ('0001-01-01T00:00:00.0000000+00:00') FOR [DateCreated]
GO
ALTER TABLE [dbo].[FormDesignsHistory] ADD  DEFAULT (N'') FOR [FormDesignId]
GO
ALTER TABLE [dbo].[FormDesignsHistory] ADD  DEFAULT ('0001-01-01T00:00:00.0000000+00:00') FOR [DateCreated]
GO
ALTER TABLE [dbo].[FormStatesConfig] ADD  DEFAULT (N'') FOR [FormDesignId]
GO
ALTER TABLE [dbo].[FormStatesConfigHistory] ADD  DEFAULT ((0)) FOR [FormVersion]
GO
ALTER TABLE [dbo].[FormTemplates] ADD  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[ProcessorHistory] ADD  DEFAULT ((0)) FOR [FormVersion]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((sysdatetimeoffset() AT TIME ZONE 'UTC')) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Designer]  WITH CHECK ADD  CONSTRAINT [FK_Designer_FormDesigns_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesigns] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Designer] CHECK CONSTRAINT [FK_Designer_FormDesigns_FormDesignId]
GO
ALTER TABLE [dbo].[DesignerHistory]  WITH CHECK ADD  CONSTRAINT [FK_DesignerHistory_FormDesignsHistory_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesignsHistory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DesignerHistory] CHECK CONSTRAINT [FK_DesignerHistory_FormDesignsHistory_FormDesignId]
GO
ALTER TABLE [dbo].[FormStatesConfig]  WITH CHECK ADD  CONSTRAINT [FK_FormStatesConfig_FormDesigns_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesigns] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FormStatesConfig] CHECK CONSTRAINT [FK_FormStatesConfig_FormDesigns_FormDesignId]
GO
ALTER TABLE [dbo].[FormStatesConfigHistory]  WITH CHECK ADD  CONSTRAINT [FK_FormStatesConfigHistory_FormDesignsHistory_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesignsHistory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FormStatesConfigHistory] CHECK CONSTRAINT [FK_FormStatesConfigHistory_FormDesignsHistory_FormDesignId]
GO
ALTER TABLE [dbo].[Processor]  WITH CHECK ADD  CONSTRAINT [FK_Processor_FormDesigns_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesigns] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Processor] CHECK CONSTRAINT [FK_Processor_FormDesigns_FormDesignId]
GO
ALTER TABLE [dbo].[ProcessorHistory]  WITH CHECK ADD  CONSTRAINT [FK_ProcessorHistory_FormDesignsHistory_FormDesignId] FOREIGN KEY([FormDesignId])
REFERENCES [dbo].[FormDesignsHistory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProcessorHistory] CHECK CONSTRAINT [FK_ProcessorHistory_FormDesignsHistory_FormDesignId]
GO