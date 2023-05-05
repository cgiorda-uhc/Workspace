using VCCommandAPI.Data.Abstract;
using VCCommandAPI.Models;

namespace VCCommandAPI.Data.Mock
{
    public class MockCommandRepo : ICommandRepo
    {
        public void CreateCommand(Command command)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command>()
            {
                new Command() { Id = 0, HowTo = "Boil an egg", Line="Boil water", Platform="Kettle & pan"},
                new Command() { Id = 1, HowTo = "Cut bread", Line="Get a knife", Platform="Knife and chopping board"},
                new Command() { Id = 2, HowTo = "Make a cup of tea", Line="Place teabag in cup", Platform="Kettle & cup"},
            };
            return commands;
        }

        public Command GetCommandById(int id)
        {
            return new Command() { Id = 0, HowTo = "Boil an egg", Line="Boil water", Platform="Kettle & Pan"};
        }

        public bool SaveChanges()
        {
            return true;
        }

        public void UpdateCommand(Command command)
        {
            throw new NotImplementedException();
        }
    }
}
