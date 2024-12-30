using AutoMapper;
using HobbyService.AsyncDataServices;
using HobbyService.Data;
using HobbyService.DTO;
using HobbyService.Models;
using Microsoft.AspNetCore.Mvc;

namespace HobbyService.Controllers
{
    //the h is because of both the hobbyservice and the userservice have an usercontroller
    [Route("api/[controller]")]
    [ApiController]
    public class HobbyController : ControllerBase
    {
        private readonly IHobbyRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public HobbyController(IHobbyRepo repository, IMapper mapper, IMessageBusClient messageBus)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBusClient = messageBus;
        }

        [HttpGet(Name = "GetAllHobbies")]
        public IActionResult GetAllHobbies()
        {
            return Ok(_repository.getAllHobbies());
        }

        [HttpGet("{id}", Name = "GetHobbyById")]
        public ActionResult<HobbyReadDto> GetHobbyById(int id)
        {
            var hobbyItem = _repository.GetHobby(id);
            if (hobbyItem == null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<HobbyReadDto>(hobbyItem));
        }

        [HttpPost]
        public IActionResult CreateHobby(HobbyCreateDto hobbyCreateDto)
        {
            var hobby = _mapper.Map<Hobby>(hobbyCreateDto);
            _repository.CreateHobby(hobby);
            _repository.SaveChanges();
            
            var hobbyReadDto = _mapper.Map<HobbyReadDto>(hobby);
            
            return CreatedAtRoute(nameof(GetHobbyById), new {Id = hobbyReadDto.Id}, hobbyReadDto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateHobby(int id, HobbyCreateDto hobbyCreateDto)
        {
            if (!_repository.HobbyExists(id))
            {
                return NotFound(new { Message = $"Hobby with id {id} not found" });
            }

            var hobbyFromRepo = _repository.GetHobby(id);
            var oldHobbyName = hobbyFromRepo.Name;
            _mapper.Map(hobbyCreateDto, hobbyFromRepo); 
          
            try
            {
                _repository.UpdateHobby(hobbyFromRepo);
                _repository.SaveChanges();
                try
                {
                    var hobbyReadDto =_mapper.Map<HobbyReadDto>(hobbyFromRepo);
                    var hobbyPublishDto = _mapper.Map<HobbyEditPublishDTO>(hobbyReadDto);
                    hobbyPublishDto.Event = "Hobby_Edited";
                    hobbyPublishDto.Name_old = oldHobbyName;
                    _messageBusClient.SendMessage_HobbyEdited(hobbyPublishDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send async: {ex.Message}");
                }
                
                return Accepted(new { message = $"Hobby with id: {id} updated successfully." });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { Message = ex.Message });
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteHobby(int id)
        {
            if (!_repository.HobbyExists(id))
            {
                return NotFound(new { Message = $"Question with id {id} not found" });
            }
            try
            {
                var HobbyName = _repository.GetHobby(id);
                _repository.DeleteHobby(id);
                _repository.SaveChanges();
                try
                {
                    
                    var hobbyPublishDto = _mapper.Map<HobbyDeletePublishDTO>(HobbyName);
                    hobbyPublishDto.Event = "Hobby_Deleted";
                    _messageBusClient.SendMessage_HobbyDeleted(hobbyPublishDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send async: {ex.Message}");
                }
                
                return Accepted(new { message = $"Question with id: {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}