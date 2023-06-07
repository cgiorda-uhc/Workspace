CREATE TABLE [deploy].[ETG_Base_Class](
	[MPC] [varchar](2) NULL,
	[ETG_Base_Class ] [float] NULL,
	[Description] [nvarchar](255) NULL,
	[Short_Description] [nvarchar](255) NULL,
	[Clean_Period] [float] NULL,
	[Drug_pre-period] [float] NULL,
	[Drug_post-period] [float] NULL,
	[Chronic_Indicator] [nvarchar](255) NULL,
	[Gender_Specific] [nvarchar](255) NULL,
	[Age_Lower_Limit] [nvarchar](255) NULL,
	[Age_Upper_Limit] [nvarchar](255) NULL,
	[Standard] [nvarchar](255) NULL,
	[Oncology] [nvarchar](255) NULL
) ON [PRIMARY]