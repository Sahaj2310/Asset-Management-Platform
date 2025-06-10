using AutoMapper;
using AssetWeb.Models;
using AssetWeb.DTOs;

namespace AssetWeb.MappingProfiles
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<CreateLocationRequest, Location>();
            CreateMap<Location, LocationResponse>()
                .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.Site.Name))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName));
        }
    }
} 