using AutoMapper;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;

namespace ChoresHub.Application.MappingProfiles
{
    public class ShoppingProductMappingProfile :Profile
    {
        public ShoppingProductMappingProfile()
        {
            CreateMap<ShoppingProduct, ShoppingProductDTO>().ReverseMap();
            CreateMap<ShoppingProduct, CreateShoppingProductDTO>().ReverseMap();

        }
    }
}