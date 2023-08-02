CREATE TABLE [etg].[PrimarySpecWithCode_PDNDB_SOURCE_ARCHIVE]
(
	[MPIN] [bigint] NOT NULL,
	[ProvType] [char](2) NULL,
	[NDB_SPCL_CD] [varchar](3) NULL,
	[SpecTypeCd] [varchar](3) NULL,
	[PrimaryInd] [char](3) NULL,
	[ShortDesc] [varchar](25) NULL,
	[PREM_SPCL_CD] [varchar](25) NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Secondary_Spec] [varchar](12) NULL, 
    [PD_Version] FLOAT NULL,
)
