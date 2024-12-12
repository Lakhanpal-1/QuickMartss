using AutoMapper;
using QuickMart.Data.DTO;
using QuickMart.Data.Entities;
using QuickMart.Services.DTO;

namespace QuickMart.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create a mapping between ApplicationUser and ApplicationUserDTO and reverse the mapping as well
            CreateMap<ApplicationUser, ApplicationUserDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
