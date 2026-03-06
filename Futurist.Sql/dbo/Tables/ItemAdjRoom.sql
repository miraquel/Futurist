CREATE TABLE [dbo].[ItemAdjRoom] (
    [RecId]       INT              NOT NULL,
    [Room]        INT              NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [AdjPrice]    NUMERIC (32, 16) NOT NULL,
    [RmPrice]     NUMERIC (32, 16) NULL,
    [PmPrice]     NUMERIC (32, 16) NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_ItemAdjRoom_1] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemAdjRoom_Room]
    ON [dbo].[ItemAdjRoom]([Room] ASC)
    INCLUDE([ItemId]);


GO

