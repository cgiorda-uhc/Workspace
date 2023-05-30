CREATE TABLE [deploy].[ETG_Dim_Premium_Spec_Master](
	[Premium_Specialty_id] [smallint] NOT NULL IDENTITY,
	[Premium_Specialty] [varchar](255) NULL, 
    CONSTRAINT [PK_ETG_Dim_Premium_Spec_Master] PRIMARY KEY ([Premium_Specialty_id])
) ON [PRIMARY]
