CREATE TABLE [dbo].[RofoRoom] (
    [Room]        INT              CONSTRAINT [DF_RofoRoom_Room] DEFAULT ((0)) NOT NULL,
    [RofoDate]    DATETIME         NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NOT NULL,
    [Qty]         NUMERIC (32, 16) NOT NULL,
    [QtyRem]      NUMERIC (32, 16) NOT NULL,
    [SalesPrice]  NUMERIC (32, 16) CONSTRAINT [DF_RofoRoom_SalesPrice] DEFAULT ((0)) NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [RecId]       INT              NOT NULL,
    CONSTRAINT [PK_RofoRoom] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_RofoRoom_Room]
    ON [dbo].[RofoRoom]([Room] ASC);


GO

