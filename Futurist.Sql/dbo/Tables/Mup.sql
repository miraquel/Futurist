CREATE TABLE [dbo].[Mup] (
    [Room]      INT              NOT NULL,
    [MupDate]   DATETIME         NOT NULL,
    [ItemId]    NVARCHAR (20)    NOT NULL,
    [ItemGroup] NVARCHAR (20)    NOT NULL,
    [Qty]       NUMERIC (32, 16) NOT NULL,
    [QtyRem]    NUMERIC (32, 16) NOT NULL,
    [RofoId]    INT              NOT NULL,
    [RecId]     INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Mup] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.Mup_dbo.Rofo_RecId] FOREIGN KEY ([RofoId]) REFERENCES [dbo].[Rofo] ([RecId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_Mup_RofoId]
    ON [dbo].[Mup]([RofoId] ASC)
    INCLUDE([ItemId], [RecId]);


GO

CREATE NONCLUSTERED INDEX [NCI_Mup_ItemId]
    ON [dbo].[Mup]([ItemId] ASC)
    INCLUDE([RofoId], [Room], [QtyRem]);


GO

