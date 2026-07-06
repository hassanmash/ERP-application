using Application.DTOs.Auth;
using AutoMapper;
using Domain.Entities;

namespace Application.MapperProfiles
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // Entity -> Response DTO. Token is NOT mapped here — it doesn't come
            // from the User entity, the Service sets it explicitly after mapping.
            CreateMap<User, LoginResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Name : null))
                .ForMember(dest => dest.Token, opt => opt.Ignore());
        }
    }
}
