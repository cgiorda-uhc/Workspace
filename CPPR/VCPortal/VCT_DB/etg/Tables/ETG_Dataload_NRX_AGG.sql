CREATE TABLE [etg].[ETG_Dataload_NRX_AGG]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
		[ETG_Base_Class] [int] NOT NULL,
	[RX_NRX] [varchar](14) NOT NULL,
	[Has_RX] [varchar](5) NOT NULL,
	[Has_NRX] [varchar](5) NOT NULL,
	[RX_RATE] [float] NULL,
	[RX] [varchar](1) NOT NULL,
	[NRX] [varchar](1) NOT NULL, 
    [PD_Version] FLOAT NULL, 
    [MEMBER_COUNT] BIGINT NULL, 
    [EPSD_COUNT] BIGINT NULL, 
    [ETGD_TOT_ALLW_AMT] BIGINT NULL, 
    [ETGD_RX_ALLW_AMT] BIGINT NULL
)
