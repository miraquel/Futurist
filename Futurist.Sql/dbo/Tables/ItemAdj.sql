CREATE TABLE [dbo].[ItemAdj] (
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    [Room]        INT              NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [AdjPrice]    NUMERIC (32, 16) NOT NULL,
    [RmPrice]     NUMERIC (32, 16) NULL,
    [PmPrice]     NUMERIC (32, 16) NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_ItemAdj] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemAdj_Room]
    ON [dbo].[ItemAdj]([Room] ASC)
    INCLUDE([AdjPrice]);


GO

