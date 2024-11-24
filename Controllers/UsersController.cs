using AutoMapper;
using HobbyService.Data;
using HobbyService.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HobbyService.Controllers
{
    //the h is because of both the hobbyservice and the userservice have an usercontroller
    [Route("api/h/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IHobbyRepo _repository;
        private readonly IMapper _mapper;

        public UsersController(IHobbyRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<UserReadDto>> GetUsers()
        {
            Console.WriteLine("--> Getting Users from the HobbyService");
            var userItems = _repository.getAllUsers();
            
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST from UserService");
            return Ok("Inbound test from User Controller");
        }
    }

}