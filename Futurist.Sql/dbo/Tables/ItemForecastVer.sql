CREATE TABLE [dbo].[ItemForecastVer] (
    [RecId]         INT              NOT NULL,
    [ItemId]        NVARCHAR (20)    NOT NULL,
    [Unit]          NVARCHAR (20)    NOT NULL,
    [ForecastDate]  DATETIME         NOT NULL,
    [ForecastPrice] NUMERIC (32, 16) NOT NULL,
    [ForcedPrice]   NUMERIC (32, 16) NOT NULL,
    [Room]          INT              NOT NULL,
    [VerId]         INT              NOT NULL,
    CONSTRAINT [FK_ItemForecastVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

