CREATE TABLE [deploy].[ETG_Fact_Symmetry_Final](
	[ETG_Base_Class] [nvarchar](255) NULL,
	[Premium_Specialty_id] SMALLINT NULL,
	[Never_Map] [nvarchar](255) NULL,


	[Current_Rx_NRx] [nvarchar](255) NULL,

	[Current_LOB] [nvarchar](255) NULL,

	[Has_Commercial] [bit] NULL,
	[Has_Medicare] [bit] NULL,
	[Has_Medicaid] [bit] NULL,
	[Has_NRX] [bit] NULL,
	[Has_RX] [bit] NULL,




	[PC_Current_Treatment_Indicator] [nvarchar](255) NULL,
	[PC_Spec_Episode_Cnt] [float] NULL,
	[PC_Spec_Episode_Distribution] [float] NULL,
	[PC_Spec_Perc_of_Episodes] [float] NULL,
	[PC_Spec_Tot_Cost] [money] NULL,
	[PC_Spec_Avg_Cost] [money] NULL,
	[PC_Spec_Normalized_Pricing] [money] NULL,
	[PC_Spec_CV] [float] NULL,

	[PC_Current_Attribution] [nvarchar](255) NULL,
	[PC_Change_Comments] [nvarchar](255) NULL,

	[EC_Current_Treatment_Indicator] [nvarchar](255) NULL,
	[EC_Spec_Episode_Cnt] [float] NULL,
	[EC_Spec_Episode_Distribution] [float] NULL,
	[EC_Spec_Perc_of_Episodes] [float] NULL,
	[EC_Spec_Tot_Cost] [money] NULL,
	[EC_Spec_Avg_Cost] [money] NULL,
	[EC_Spec_Normalized_Pricing] [money] NULL,
	[EC_Spec_CV] [float] NULL,

	[EC_Current_Mapping] [nvarchar](255) NULL,
	[EC_Change_Comments] [nvarchar](255) NULL,
	[Measure_Status] [nvarchar](255) NULL,
	[Symmetry_Version] [float] NULL,
	[PD_Version] [smallint] NULL, 
    [Deploy_Id] INT NOT NULL IDENTITY,

) 