CREATE TABLE [vct].[Logs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [log_level] VARCHAR(50) NULL, 
    [event_name] VARCHAR(50) NULL, 
    [source] VARCHAR(50) NULL, 
    [exception_message] VARCHAR(MAX) NULL, 
    [stack_trace] VARCHAR(MAX) NULL, 
    [insert_date] DATETIME NULL DEFAULT getdate(), 
    [state] VARCHAR(255) NULL
)
