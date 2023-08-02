CREATE TABLE [etg].[ETG_Dataload_PC_AGG_ARCHIVE]
(
		[Id] [int] IDENTITY(1,1) NOT NULL,
			[Premium_Specialty] [varchar](25) NULL,
	[ETG_Base_Class] [char](6) NULL,
	[PC_Episode_Count] [int] NULL,
	[PC_Total_Cost] [numeric](19, 2) NULL,
	[PC_Average_Cost] [numeric](19, 2) NULL,
	[PC_Coefficients_of_Variation] [numeric](9, 4) NULL,
	[PC_Normalized_Pricing_Episode_Count] [int] NULL,
	[PC_Normalized_Pricing_Total_Cost] [numeric](19, 2) NULL,
	[PC_Spec_Episode_Count] [int] NULL,
	[PC_Spec_Total_Cost] [numeric](19, 2) NULL,
	[PC_Spec_Average_Cost] [numeric](19, 2) NULL,
	[PC_Spec_CV] [numeric](9, 4) NULL,
	[PC_Spec_Percent_of_Episodes] [numeric](6, 2) NULL,
	[PC_Spec_Normalized_Pricing_Episode_Count] [int] NULL,
	[PC_Spec_Normalized_Pricing_Total_Cost] [numeric](19, 2) NULL,
	[PC_CV3] [varchar](1) NOT NULL,
	[PC_Spec_Epsd_Volume] [varchar](1) NOT NULL, 
    [PD_Version] FLOAT NULL

)
