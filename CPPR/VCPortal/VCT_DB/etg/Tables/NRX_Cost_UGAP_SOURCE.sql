CREATE TABLE [etg].[NRX_Cost_UGAP_SOURCE]
(
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ETG_BAS_CLSS_NBR] [int] NOT NULL,
	[TRT_CD] [smallint] NULL,
	[MEMBER_COUNT] [int] NULL,
	[EPSD_COUNT] [int] NULL,
	[ETGD_TOT_ALLW_AMT] [float] NULL,
	[ETGD_RX_ALLW_AMT] [float] NULL,
	[RX_RATE] [float] NULL
)
