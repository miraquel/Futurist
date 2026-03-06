CREATE TABLE [dbo].[ItemGroupProcurement] (
    [ItemId]    NVARCHAR (20) NULL,
    [ItemName]  NVARCHAR (60) NULL,
    [GroupName] NVARCHAR (20) NULL,
    [RecId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemGroupProcurement] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemGroupProcurement_ItemId]
    ON [dbo].[ItemGroupProcurement]([ItemId] ASC);


GO

