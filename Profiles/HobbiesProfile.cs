using AutoMapper;
using HobbyService.DTO;
using HobbyService.Models;
using UserService;


namespace HobbyService.Profiles;

public class HobbiesProfile : Profile
{
    public HobbiesProfile()
    {
       // Source -> Target
       CreateMap<User, UserReadDto>();
       CreateMap<HobbyCreateDto, Hobby>();
       CreateMap<Hobby, HobbyReadDto>();
       CreateMap<UserPublishedDto, User>()
           .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
       CreateMap<GrpcUserModel, User>()
           .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.UserId))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Hobbies, opt => opt.Ignore());
    }
}