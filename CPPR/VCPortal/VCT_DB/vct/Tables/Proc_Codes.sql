CREATE TABLE [vct].[Proc_Codes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Proc_Cd] VARCHAR(7) NULL, 
    [Proc_Desc] VARCHAR(100) NULL, 
    [Proc_Cd_Type] VARCHAR(15) NULL, 
    [Proc_Cd_Date] DATETIME NULL
)
