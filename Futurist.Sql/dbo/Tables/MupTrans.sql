CREATE TABLE [dbo].[MupTrans] (
    [RecId]       INT IDENTITY (1, 1) NOT NULL,
    [Room]        INT NOT NULL,
    [MupId]       INT NOT NULL,
    [ItemTransId] INT NOT NULL,
    CONSTRAINT [PK_MupTrans] PRIMARY KEY CLUSTERED ([RecId] ASC),
    CONSTRAINT [FK_dbo.MupTrans_dbo.ItemTrans_RecId] FOREIGN KEY ([ItemTransId]) REFERENCES [dbo].[ItemTrans] ([RecId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MupTrans_dbo.Mup_RecId] FOREIGN KEY ([MupId]) REFERENCES [dbo].[Mup] ([RecId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_MupTrans_Room]
    ON [dbo].[MupTrans]([Room] ASC);


GO

CREATE NONCLUSTERED INDEX [NCI_MupTrans_MupId]
    ON [dbo].[MupTrans]([MupId] ASC)
    INCLUDE([ItemTransId]);


GO

