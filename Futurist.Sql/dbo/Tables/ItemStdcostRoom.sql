CREATE TABLE [dbo].[ItemStdcostRoom] (
    [RecId]  INT              NOT NULL,
    [ItemId] NVARCHAR (20)    NOT NULL,
    [Price]  NUMERIC (32, 16) NOT NULL,
    [Room]   INT              NOT NULL
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemStdcostRoom_ItemId]
    ON [dbo].[ItemStdcostRoom]([ItemId] ASC)
    INCLUDE([Room]);


GO

