CREATE PROCEDURE [chemopx].[sp_ChemotherapyPX_Delete]
	@Id int
AS
BEGIN
	--WE DONT DELETE!!!
	UPDATE [dbo].[ChemotherapyPX] SET [Is_Archived] = 1 WHERE [Id] = @Id;
END
