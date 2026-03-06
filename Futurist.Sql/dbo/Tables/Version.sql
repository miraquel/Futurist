CREATE TABLE [dbo].[Version] (
    [VerId]   INT           IDENTITY (1, 1) NOT NULL,
    [VerDate] DATETIME      NOT NULL,
    [Room]    INT           NOT NULL,
    [Notes]   NVARCHAR (50) NOT NULL,
    [Cancel]  INT           CONSTRAINT [DF_Version_Cancel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED ([VerId] ASC)
);


GO

