using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VCCommandAPI.Data.Abstract;
using VCCommandAPI.Data.Mock;
using VCCommandAPI.Dtos;
using VCCommandAPI.Models;

namespace VCCommandAPI.Controllers
{
    //api/commands
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {

        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        //GET //api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();
            //AutoMapper MAGIC Command.cs FOR DB CommandReadDto.cs FOR CLIENT!!!
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));//200 SUCCESS
        }

        //GET //api/commands/{id}
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem != null)
                return Ok(_mapper.Map<CommandReadDto>(commandItem));//200 SUCCESS

            //THROW 404 NOT FOUND
            return NotFound();

        }


        //CREATE DTO IN AND READ DTO OUT AS RESULTS
        //POST (AKA INSERT) //api/commands/
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            //MAP CommandCreateDto TO Command FOR DB SAVE
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();

            //MAP Command TO CommandReadDto FOR CLIENT RETURN
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);


            //RESTFUL API MUST RETURN URI OF INSERTED OBJECT
            //ADDS 'Location' Header TO CLIENT Ex: Location = https://localhost:7064/api/Commands/6
            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto); //RETURN 201 CREATED

        }


        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if(commandModelFromRepo == null)
            {
                return NotFound(); //404
            }

            
            //commandUpdateDto = SOURCE commandModelFromRepo = DESTINATION
            var command = _mapper.Map(commandUpdateDto, commandModelFromRepo);
            //NOT ACTUALLY IMPLEMENTED WIHTIN CommandRepo UNLIKE INSERTS, UPDATES ARE TRACKED AND HANDLED VIA AutoMapper AND CommandContext
            //GOOD PRACTICE TO CALL ANYWAY IN CASE NEW REPO NEEDS TO IMPLEMENT UNLIKE EF
            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();

            //
            return NoContent(); //204
        }

    }
}
