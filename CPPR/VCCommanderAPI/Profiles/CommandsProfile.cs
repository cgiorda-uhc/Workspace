using AutoMapper;
using VCCommandAPI.Dtos;
using VCCommandAPI.Models;

namespace VCCommandAPI.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //MAP OUR DTO TO OUR MODEL VIA AuotMapper
            //SOURCE -> TARGET
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
        }
    }
}
