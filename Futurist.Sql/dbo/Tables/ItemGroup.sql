CREATE TABLE [dbo].[ItemGroup] (
    [ItemId]   NVARCHAR (20) NOT NULL,
    [ItemName] NVARCHAR (60) NOT NULL,
    [Group01]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group01] DEFAULT ('') NOT NULL,
    [Group02]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group02] DEFAULT ('') NOT NULL,
    [Group03]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group03] DEFAULT ('') NOT NULL,
    [Group04]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group04] DEFAULT ('') NOT NULL,
    [Group05]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group05] DEFAULT ('') NOT NULL,
    [Group06]  NVARCHAR (20) CONSTRAINT [DF_ItemGroup_Group06] DEFAULT ('') NOT NULL,
    [RecId]    INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemGroup] PRIMARY KEY CLUSTERED ([ItemId] ASC)
);


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group Material Commercial', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group02';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group Material Procurement', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group01';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group FG', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group04';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group FG', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group03';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group FG', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group06';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Group FG', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemGroup', @level2type = N'COLUMN', @level2name = N'Group05';


GO

