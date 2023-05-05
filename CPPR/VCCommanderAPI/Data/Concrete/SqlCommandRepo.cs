using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using VCCommandAPI.Data.Abstract;
using VCCommandAPI.Data.Context;
using VCCommandAPI.Models;

namespace VCCommandAPI.Data.Concrete
{
    public class SqlCommandRepo : ICommandRepo
    {
        private readonly CommandContext _context;

        public SqlCommandRepo(CommandContext context)
        {
            _context = context;
        }

        public void CreateCommand(Command command)
        {
            if(command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _context.Add(command);

        }

        public IEnumerable<Command> GetAllCommands()
        {
            return _context.Commands.ToList();
        }

        public Command GetCommandById(int id)
        {
            return _context.Commands.FirstOrDefault(p=>p.Id == id);
        }

        public bool SaveChanges()
        {
            return(_context.SaveChanges() >= 0);
        }

        //MUST IMPLEMENT FOR INTERFACE
        public void UpdateCommand(Command command)
        {
            //NOTHING
            //HANDLE UPDATES VIA DB CONTEXT CommandContext.cs
        }
    }
}
