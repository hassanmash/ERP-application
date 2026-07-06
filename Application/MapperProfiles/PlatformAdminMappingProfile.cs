using Application.DTOs.PlatformAdmin;
using AutoMapper;
using Domain.Entities;

namespace Application.MapperProfiles
{
    public class PlatformAdminMappingProfile : Profile
    {
        public PlatformAdminMappingProfile()
        {
            // Request DTO -> Entity. Id/Status/CreatedAt are deliberately NOT
            // mapped from the request — they're server-assigned, never
            // client-supplied, so the Service sets them explicitly after mapping
            // rather than trusting AutoMapper to leave them at defaults correctly.
            CreateMap<CreateOrganizationRequest, Organization>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrgCode, opt => opt.Ignore()); // normalized explicitly in Service, not a direct copy

            CreateMap<Organization, OrganizationResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.EnabledModules, opt => opt.Ignore()) // set explicitly in Service from the OrgModules just created
                .ForMember(dest => dest.AdminUserId, opt => opt.Ignore())
                .ForMember(dest => dest.AdminEmail, opt => opt.Ignore());
        }
    }
}
