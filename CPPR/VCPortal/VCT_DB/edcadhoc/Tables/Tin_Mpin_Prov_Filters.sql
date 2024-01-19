CREATE TABLE [edcadhoc].[Tin_Mpin_Prov_Filters]
(
	[TIN] BIGINT NOT NULL PRIMARY KEY, 
    [MPIN] BIGINT NULL, 
    [Prov_Sys_Id] BIGINT NULL, 
    [TIN_Name] VARCHAR(255) NULL, 
    [MPIN_Name] VARCHAR(255) NULL
)
