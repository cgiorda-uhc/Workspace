CREATE TABLE [etg].[ETG_Dataload_NRX_AGG_ARCHIVE]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
		[ETG_Base_Class] [int] NULL,
	[RX_NRX] [varchar](14) NULL,
	[Has_RX] [varchar](5) NULL,
	[Has_NRX] [varchar](5) NULL,
	[RX_RATE] [float] NULL,
	[RX] [varchar](1) NULL,
	[NRX] [varchar](1) NULL, 
    [PD_Version] FLOAT NULL
)
