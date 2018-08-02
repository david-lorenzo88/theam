using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Theam.API.Models;

namespace Theam
{
    /// <summary>
    /// Automapper mappings profile where all mappings are defined
    /// </summary>
    internal class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<User, UserDTO>()
                //We leave this unmapped so Password won't be returned in users API response
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<UserDTO, User>();
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
        }
    }
}