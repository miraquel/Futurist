CREATE TABLE [dbo].[UnitConversion] (
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    [Factor]      NUMERIC (32, 16) NOT NULL,
    [Numerator]   NUMERIC (32, 16) NOT NULL,
    [Denominator] NUMERIC (32, 16) NOT NULL,
    [FromUnit]    NVARCHAR (20)    NOT NULL,
    [ToUnit]      NVARCHAR (20)    NOT NULL
);


GO

