using AutoMapper;
using GuitarShopApp.Application.DTO;
using GuitarShopApp.Domain.Entities;

namespace GuitarShopApp.Application.Mapping;

public class GeneralMapping : Profile
{
    public GeneralMapping()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<ProductDTO, ProductDTO>().ReverseMap();
        CreateMap<OrderDTO, Order>().ReverseMap();
        CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
        CreateMap<User, LoginDTO>().ReverseMap();
        CreateMap<User, User>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
    }
}