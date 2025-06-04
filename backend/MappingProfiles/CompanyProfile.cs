using AutoMapper;
using AssetWeb.DTOs;
using AssetWeb.Models;

namespace AssetWeb.MappingProfiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyRegistrationRequest, Company>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.FinancialYearMonth, opt => opt.MapFrom(src => src.FinancialYearMonth))
                .ForMember(dest => dest.FinancialYearDay, opt => opt.MapFrom(src => src.FinancialYearDay))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.LogoPath, opt => opt.MapFrom(src => src.LogoPath));

            CreateMap<Company, CompanyResponse>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.FinancialYearMonth, opt => opt.MapFrom(src => src.FinancialYearMonth))
                .ForMember(dest => dest.FinancialYearDay, opt => opt.MapFrom(src => src.FinancialYearDay))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.LogoPath, opt => opt.MapFrom(src => src.LogoPath));

            // Profile mappings
            CreateMap<User, UserProfileResponse>();
        }
    }
} 