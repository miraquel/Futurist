CREATE TABLE [dbo].[Rofo] (
    [Room]        INT              NOT NULL,
    [RofoDate]    DATETIME         NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NOT NULL,
    [Qty]         NUMERIC (32, 16) NOT NULL,
    [QtyRem]      NUMERIC (32, 16) NOT NULL,
    [SalesPrice]  NUMERIC (32, 16) CONSTRAINT [DF_Rofo_SalesPrice] DEFAULT ((0)) NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Rofo] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE UNIQUE NONCLUSTERED INDEX [UC_RoomItemIdRofoDate]
    ON [dbo].[Rofo]([Room] ASC, [ItemId] ASC, [RofoDate] ASC);


GO

CREATE NONCLUSTERED INDEX [NCI_Rofo_ItemId]
    ON [dbo].[Rofo]([ItemId] ASC)
    INCLUDE([Room]);


GO

