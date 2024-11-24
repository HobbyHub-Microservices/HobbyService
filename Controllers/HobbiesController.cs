using AutoMapper;
using HobbyService.Data;
using HobbyService.DTO;
using HobbyService.Models;
using Microsoft.AspNetCore.Mvc;

namespace HobbyService.Controllers;

[Route("api/h/users/{userId}/[controller]")]
[ApiController]
public class HobbiesController : ControllerBase
{
    private readonly IHobbyRepo _repo;
    private readonly IMapper _mapper;

    public HobbiesController(IHobbyRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<HobbyReadDto>> GetHobbiesForUser(int userId) //userId gets past on from the Route variable
    {
        Console.WriteLine($"--> Getting Hobbies for user:{userId} from HobbyService");
        if (!_repo.UserExists(userId))
        {
            return NotFound();
        }
        
        var hobbies = _repo.getHobbiesForUser(userId);
        return Ok(_mapper.Map<IEnumerable<HobbyReadDto>>(hobbies));
    }

    [HttpGet("{hobbyId}", Name = "GetHobbyForId")]
    public ActionResult<HobbyReadDto> GetHobbyForId(int userId, int hobbyId)
    {
        Console.WriteLine($"--> Getting Hobby: {hobbyId} for user:{userId} from HobbyService");
        if (!_repo.UserExists(userId))
        {
            return NotFound();
        }
        
        var hobby = _repo.GetHobby(userId, hobbyId);
        if (hobby == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<HobbyReadDto>(hobby));
    }

    [HttpPost]
    public ActionResult<HobbyReadDto> CreateHobbyForUser(int userId, HobbyCreateDto hobbyCreateDto)
    {
        Console.WriteLine($"--> Create Hobby for user:{userId} from HobbyService");
        if (!_repo.UserExists(userId))
        {
            return NotFound();
        }
        
        var hobby = _mapper.Map<Hobby>(hobbyCreateDto);
        _repo.CreateHobby(userId, hobby);
        _repo.SaveChanges(); //This always needs to happen otherwise it won't go to the persistence layer
        
        var hobbyReadDto = _mapper.Map<HobbyReadDto>(hobby);
        return CreatedAtRoute(nameof(GetHobbyForId),
            new{userId = userId, hobbyId = hobbyReadDto.Id}, hobbyReadDto);
    }

}