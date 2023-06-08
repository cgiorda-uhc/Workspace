CREATE TABLE [vct].[PrimarySpecWithCode]
(
	[MPIN] BIGINT NOT NULL , 
    [ProvType] CHAR(2) NULL, 
    [NDB_SPCL_CD] VARCHAR(3) NULL, 
    [SpecTypeCd] VARCHAR(3) NULL, 
    [PrimaryInd] CHAR(3) NULL, 
    [ShortDesc] VARCHAR(25) NULL, 
    [PREM_SPCL_CD] VARCHAR(25) NULL, 
    [Id] BIGINT NOT NULL IDENTITY, 
    PRIMARY KEY ([Id])
)
