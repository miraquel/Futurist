CREATE TABLE [dbo].[JobLogs] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Message]         NVARCHAR (MAX) NULL,
    [MessageTemplate] NVARCHAR (MAX) NULL,
    [Level]           NVARCHAR (MAX) NULL,
    [TimeStamp]       DATETIME       NULL,
    [Exception]       NVARCHAR (MAX) NULL,
    [Properties]      NVARCHAR (MAX) NULL,
    [SourceContext]   VARCHAR (MAX)  NULL,
    [Status]          VARCHAR (MAX)  NULL,
    [JobId]           VARCHAR (MAX)  NULL,
    CONSTRAINT [PK_JobLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO

