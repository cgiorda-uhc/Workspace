﻿CREATE TABLE [peg].[DQC_DATA_UHPD_SOURCE]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
		[PEG_ANCH_CATGY] [varchar](11) NULL,
[PEG_ANCH_SBCATGY] [varchar](11) NULL,
[PREM_SPCL_CD] [char](8) NULL,
[SVRTY_LVL_CD] [char](1) NULL,
[APR_DRG_RLLP_NBR] INT NULL,
[QLTY_MSR_NM] [varchar](18) NULL,
[CNFG_POP_SYS_ID] INT NULL,
[MKT_NBR] [varchar](12) NULL,
[UNET_MKT_NBR] INT NULL,
[UNET_MKT_DESC] [varchar](60) NULL,
[Current_Version] [varchar](50) NULL,
[Current_Market_Compliant] BIGINT NULL,
[Current_Market_Opportunity] BIGINT NULL,
[Current_National_Compliant] INT NULL,
[Current_National_Opportunity] INT NULL,
[Previous_Version] [varchar](50) NULL,
[Previous_Market_Compliant] BIGINT NULL,
[Previous_Market_Opportunity] BIGINT NULL,
[Previous_National_Compliant] INT NULL,
[Previous_National_Opportunity] INT NULL,
[DTLocation] [varchar](50) NULL,
[Data_Extract_Dt] [Date] NULL 
)
