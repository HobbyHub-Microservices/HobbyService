using AutoMapper;
using HobbyService.Data;
using HobbyService.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HobbyService.Controllers
{
    //the h is because of both the hobbyservice and the userservice have an usercontroller
    [Route("api/h/[controller]")]
    [ApiController]
    public class HobbyController : ControllerBase
    {
        private readonly IHobbyRepo _repository;
        private readonly IMapper _mapper;

        public HobbyController(IHobbyRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAllHobbies")]
        public IActionResult GetAllHobbies()
        {
            return Ok(_repository.getAllHobbies());
        }
    }
}