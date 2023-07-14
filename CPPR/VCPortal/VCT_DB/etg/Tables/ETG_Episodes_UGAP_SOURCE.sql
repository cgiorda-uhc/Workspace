CREATE TABLE [etg].[ETG_Episodes_UGAP_SOURCE]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
		[EPSD_NBR] [varchar](20) NULL,
	[TOT_ALLW_AMT] [float] NULL,
	[SVRTY] [char](1) NULL,
	[ETG_BAS_CLSS_NBR] [char](6) NULL,
	[ETG_TX_IND] [smallint] NULL,
	[PROV_MPIN] [int] NULL,
	[TOT_NP_ALLW_AMT] [float] NULL,
	[LOB_ID] [smallint] NULL
)
