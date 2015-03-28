
C:\Users\Morel>sqlcmd -Smorel-pc\sqlexpress
USE [master]
GO
CREATE DATABASE BddMain ON (  NAME = BddMain_dat,  FILENAME = 'D:\Docs\Telecharger\DEV\WpfTutoSynthes\WpfTutoSynthes\BDD\BddMain.mdf')LOG ON (  NAME = BddMain_log,  FILENAME = 'D:\Docs\Telecharger\DEV\WpfTutoSynthes\WpfTutoSynthes\BDD\BddMain_log.ldf')
--CREATE DATABASE BddMain ON (FILENAME=N'D:\Docs\Telecharger\DEV\WpfTutoSynthes\WpfTutoSynthes\BDD\BddMain.mdf'), (FILENAME=N'D:\Docs\Telecharger\DEV\WpfTutoSynthes\WpfTutoSynthes\BDD\BddMain_log.ldf') FOR ATTACH;
GO
use BddMain;
GO
--EXEC sp_detach_db @dbname = N'BddMain';
EXIT



CREATE TABLE [dbo].[Users] (
    [usrId]            INT          NOT NULL,
    [usrNom]           VARCHAR (50) NOT NULL,
    [usrPrenom]        VARCHAR (50) NOT NULL,
    [usrDateNaissance] DATETIME     NULL,
    [usrDateCrea]      DATETIME     NOT NULL,
    [usrActif]         BIT          NOT NULL,
    [usrSrvId]         INT          NOT NULL,
    [usrCvlId]         INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([usrId] ASC)
);
GO
INSERT INTO [dbo].[Users] ([usrId], [usrNom], [usrPrenom], [usrDateNaissance], [usrDateCrea], [usrActif], [usrSrvId], [usrCvlId]) VALUES (1, N'Test', N'David', NULL, N'2015-03-12 00:00:00', 1, 1, 1)
INSERT INTO [dbo].[Users] ([usrId], [usrNom], [usrPrenom], [usrDateNaissance], [usrDateCrea], [usrActif], [usrSrvId], [usrCvlId]) VALUES (2, N'Test', N'Céline', NULL, N'2015-03-12 00:00:00', 1, 2, 2)
INSERT INTO [dbo].[Users] ([usrId], [usrNom], [usrPrenom], [usrDateNaissance], [usrDateCrea], [usrActif], [usrSrvId], [usrCvlId]) VALUES (3, N'Test', N'Alexis', N'2007-03-12 00:00:00', N'2015-03-12 00:00:00', 1, 3, 1)
INSERT INTO [dbo].[Users] ([usrId], [usrNom], [usrPrenom], [usrDateNaissance], [usrDateCrea], [usrActif], [usrSrvId], [usrCvlId]) VALUES (4, N'Test', N'Maxime', N'2012-03-12 00:00:00', N'2015-03-12 00:00:00', 1, 1, 1)
INSERT INTO [dbo].[Users] ([usrId], [usrNom], [usrPrenom], [usrDateNaissance], [usrDateCrea], [usrActif], [usrSrvId], [usrCvlId]) VALUES (5, N'Test', N'Solenn', N'2009-03-12 00:00:00', N'2015-03-12 00:00:00', 1, 4, 2)

CREATE TABLE [dbo].[Services] (
    [srvId]  INT          NOT NULL,
    [srvLib] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([srvId] ASC)
);
GO
INSERT INTO [dbo].[Services] ([srvId], [srvLib]) VALUES (1, N'Developpement')
INSERT INTO [dbo].[Services] ([srvId], [srvLib]) VALUES (2, N'Compta')
INSERT INTO [dbo].[Services] ([srvId], [srvLib]) VALUES (3, N'Intégrateurs')
INSERT INTO [dbo].[Services] ([srvId], [srvLib]) VALUES (4, N'Chefs de projets')

CREATE TABLE [dbo].[Civilites] (
    [cvlId]  INT          NOT NULL,
    [cvlLib] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([cvlId] ASC)
);
GO
INSERT INTO [dbo].[Civilites] ([cvlId], [cvlLib]) VALUES (1, N'Monsieur')
INSERT INTO [dbo].[Civilites] ([cvlId], [cvlLib]) VALUES (2, N'Madame')

