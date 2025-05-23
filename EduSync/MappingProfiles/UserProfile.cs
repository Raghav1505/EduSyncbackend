using AutoMapper;
using EduSync.Models;
using EduSync.DTOs;

namespace EduSync.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                // we’ll hash the password in the controller or via a service
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, _, srcVal) => srcVal != null));
        }
    }
}
