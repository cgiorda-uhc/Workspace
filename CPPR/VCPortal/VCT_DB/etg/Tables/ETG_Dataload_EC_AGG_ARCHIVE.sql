CREATE TABLE [etg].[ETG_Dataload_EC_AGG_ARCHIVE]
(
			[Id] [int] IDENTITY(1,1) NOT NULL,
		[Premium_Specialty] [varchar](25) NULL,
	[ETG_Base_Class] [char](6) NULL,
	[EC_Treatment_Indicator] [smallint] NULL,
	[EC_Episode_Count] [int] NULL,
	[EC_Total_Cost] [numeric](19, 2) NULL,
	[EC_Average_Cost] [numeric](19, 2) NULL,
	[EC_Coefficients_of_Variation] [numeric](9, 4) NULL,
	[EC_Normalized_Pricing_Episode_Count] [int] NULL,
	[EC_Normalized_Pricing_Total_Cost] [numeric](19, 2) NULL,
	[EC_Spec_Episode_Count] [int] NULL,
	[EC_Spec_Total_Cost] [numeric](19, 2) NULL,
	[EC_Spec_Average_Cost] [numeric](19, 2) NULL,
	[EC_Spec_Coefficients_of_Variation] [numeric](9, 4) NULL,
	[EC_Spec_Percent_of_Episodes] [numeric](6, 2) NULL,
	[EC_Spec_Normalized_Pricing_Episode_Count] [int] NULL,
	[EC_Spec_Normalized_Pricing_Total_Cost] [numeric](19, 2) NULL,
	[EC_CV3] [varchar](1) NOT NULL,
	[EC_Spec_Episode_Volume] [varchar](1) NOT NULL,
	[PD_Mapped] [varchar](1) NULL, 
    [PD_Version] FLOAT NULL
)
