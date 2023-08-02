CREATE TABLE [etg].[ETG_Cancer_Flag_PD_SOURCE_ARCHIVE]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ETG_BASE_CLASS] [int] NOT NULL,
	[CNCR_IND] [char](1) NULL, 
    [PD_Version] FLOAT NULL
)
