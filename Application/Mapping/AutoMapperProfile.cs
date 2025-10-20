using Application.DTOs;
using AutoMapper;
using Core.Entities;

namespace Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryId,
                           opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ReverseMap();

            CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
    .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderDto, Order>();

            CreateMap<OrderItem, OrderItemDto>()
    .ForMember(dest => dest.ProductName,
           opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));


            CreateMap<OrderItemDto, OrderItem>();



        }
    }
}
