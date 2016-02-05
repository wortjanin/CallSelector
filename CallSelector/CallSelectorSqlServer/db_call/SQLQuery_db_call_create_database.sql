
--CAUTION!
--BEFORE RUNNING THIS SCRIPT ON SQL SERVER 2008 UNDER AN ADMINISTRATOR ACCOUNT
--GIVE THE ->FILENAME<- PROPERTIES AN APPROPRIATE VALUE (CHANGE C:\temp\somewhere... TO WHATEVER YOU NEED )
--DO NOT FORGET EITHER TO VISIT FILES CallSelectorConfig.xml AND CallSelectorConfigServer.xml
--AND CHANGE ->server=WS_ITPARK\SMSDELIVERY<- PROPERTY TO YOUR SERVER AS WELL
--...AND COMMENT THIS NOTICE OUT
-- AFTER RUNNING THE SCRIPT, CREATE USER call_selector / call_selector_pass
-- AND GIVE IT APPROPRIATE RIGHTS


USE [master]
GO

/****** Object:  Database [db_call]    Script Date: 02/18/2012 07:04:02 ******/
CREATE DATABASE [db_call] ON  PRIMARY 
( NAME = N'db_call', FILENAME = N'C:\Users\0\db_call\db_call.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'db_call_log', FILENAME = N'C:\Users\0\db_call\db_call_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [db_call] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_call].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [db_call] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [db_call] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [db_call] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [db_call] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [db_call] SET ARITHABORT OFF 
GO

ALTER DATABASE [db_call] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [db_call] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [db_call] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [db_call] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [db_call] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [db_call] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [db_call] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [db_call] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [db_call] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [db_call] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [db_call] SET  DISABLE_BROKER 
GO

ALTER DATABASE [db_call] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [db_call] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [db_call] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [db_call] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO

ALTER DATABASE [db_call] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [db_call] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [db_call] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [db_call] SET  READ_WRITE 
GO

ALTER DATABASE [db_call] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [db_call] SET  MULTI_USER 
GO

ALTER DATABASE [db_call] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [db_call] SET DB_CHAINING OFF 
GO
