USE [db_call]
GO

/****** Object:  Table [dbo].[operator]    Script Date: 02/18/2012 03:05:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[operator](
	[id_operator] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
	[mail] [nvarchar](128) NULL,
	CONSTRAINT [uq_id_operator] UNIQUE ([mail])
) ON [PRIMARY]

GO


USE [db_call]
GO

/****** Object:  Table [dbo].[call]    Script Date: 02/18/2012 03:09:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[phone_call](
	[id_phone_call] [numeric](18, 0) NOT NULL PRIMARY KEY IDENTITY(1,1),
	[id_operator] [int] NOT NULL,
	[phone] [numeric](12, 0) NOT NULL,
	[date_start] [datetime] NOT NULL,
	[date_interval] [time] NOT NULL,
	[sender_mail] [nvarchar](128) NOT NULL,
	[file_name] [nvarchar](max) NULL,
	CONSTRAINT fk_id_operator FOREIGN KEY (id_operator) REFERENCES operator(id_operator)    
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[failed_message](
	[id_failed_message] [numeric](18, 0) NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sender_mail] [nvarchar](128) NOT NULL,
	[file_name] [nvarchar](max) NULL,
	[message_plain_text] [nvarchar](max) NULL,
	[description] [nvarchar](max) NULL,
) ON [PRIMARY]

GO