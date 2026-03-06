CREATE TABLE [dbo].[DimItemSubstituteVer] (
    [ItemId]                 NVARCHAR (20) NOT NULL,
    [VtaMpSubstitusiGroupId] NVARCHAR (20) NOT NULL,
    [Description]            NVARCHAR (60) NOT NULL,
    [VerId]                  INT           NOT NULL
);


GO

CREATE NONCLUSTERED INDEX [NCI_DimItemSubstituteVer_VerId]
    ON [dbo].[DimItemSubstituteVer]([VerId] ASC)
    INCLUDE([ItemId], [VtaMpSubstitusiGroupId]);


GO

