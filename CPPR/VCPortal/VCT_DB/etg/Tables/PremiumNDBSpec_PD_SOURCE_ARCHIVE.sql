CREATE TABLE [etg].[PremiumNDBSpec_PD_SOURCE_ARCHIVE]
(
			[Id] [int] IDENTITY(1,1) NOT NULL,
		[NDB_SPCL_TYP_CD] [varchar](15) NOT NULL,
	[SPCL_TYP_CD_DESC] [varchar](100) NULL,
	[PREM_SPCL_CD] [varchar](15) NULL, 
    [PD_Version] FLOAT NULL
)
