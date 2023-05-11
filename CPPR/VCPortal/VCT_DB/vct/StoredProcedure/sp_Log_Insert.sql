CREATE PROCEDURE [vct].[sp_Log_Insert]
    @log_level VARCHAR(50) NULL, 
    @event_name VARCHAR(50) NULL, 
   @source VARCHAR(50) NULL, 
   @exception_message VARCHAR(MAX) NULL, 
    @stack_trace VARCHAR(MAX) NULL,
    @state VARCHAR(255) NULL
AS
BEGIN
	INSERT INTO [vct].[Logs] (    [log_level] , 
    [event_name], 
    [source], 
    [exception_message], 
    [stack_trace],[state] 
    ) VALUES (@log_level, 
    @event_name, 
   @source, 
   @exception_message, 
    @stack_trace, @state ); 

END
