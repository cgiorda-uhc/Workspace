﻿CREATE TABLE [dbo].[PA_Drugs]
(
	[PA_DRUGS_ID] SMALLINT NOT NULL PRIMARY KEY IDENTITY, 
    [PA_DRUGS] VARCHAR(20) NULL, 
    [Is_Archived] BIT NULL DEFAULT 0
)
