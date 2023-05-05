using VCCommandAPI.Models;

namespace VCCommandAPI.Data.Abstract
{
    public interface ICommandRepo
    {

        bool SaveChanges();


        IEnumerable<Command> GetAllCommands();

        Command GetCommandById(int id);

        void CreateCommand(Command command);

        void UpdateCommand(Command command);

    }
}
