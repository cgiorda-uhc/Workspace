CREATE TABLE [etgsymm].[ETG_Fact_Symmetry_Update_Tracker](
	[ETG_Fact_Symmetry_UT_id] [bigint] IDENTITY(1,1) NOT NULL,
	[ETG_Fact_Symmetry_id] [bigint] NOT NULL,
	[Has_Commercial] [bit] NULL,
	[Has_Medicare] [bit] NULL,
	[Has_Medicaid] [bit] NULL,
	[Has_NRX] [bit] NULL,
	[Has_RX] [bit] NULL,
	[PC_Treatment_Indicator] [varchar](10) NULL,
	[PC_Attribution] [varchar](25) NULL,
	[PC_Change_Comments] [ntext] NULL,
	[Patient_Centric_Mapping] [varchar](25) NULL,
	[EC_Mapping] [varchar](50) NULL,
	[EC_Treatment_Indicator] [varchar](10) NULL,
	[EC_Change_Comments] [ntext] NULL,
	[Patient_Centric_Change_Comments] [ntext] NULL,


	[Has_Commercial_Previous] [bit] NULL,
	[Has_Medicare_Previous] [bit] NULL,
	[Has_Medicaid_Previous] [bit] NULL,
	[Has_NRX_Previous] [bit] NULL,
	[Has_RX_Previous] [bit] NULL,
	[PC_Treatment_Indicator_Previous] [varchar](10) NULL,
	[PC_Attribution_Previous] [varchar](25) NULL,
	[PC_Change_Comments_Previous] [ntext] NULL,
	[Patient_Centric_Mapping_Previous] [varchar](25) NULL,
	[EC_Mapping_Previous] [varchar](50) NULL,
	[EC_Treatment_Indicator_Previous] [varchar](10) NULL,
	[EC_Change_Comments_Previous] [ntext] NULL,
	[Patient_Centric_Change_Comments_Previous] [ntext] NULL,

	[username] [varchar](10) NULL,
	[update_date] [datetime] NULL,
[Never_Mapped] BIT NULL, 
    [Never_Mapped_Previous] BIT NULL, 
    CONSTRAINT [PK_ETG_Fact_Symmetry_Update_Tracker] PRIMARY KEY CLUSTERED 
(
	[ETG_Fact_Symmetry_UT_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO