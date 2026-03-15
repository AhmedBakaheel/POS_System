using AutoMapper;
using POS.Domain.Entities;
using POS.Application.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using POS.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>()
                 .ForMember(dest => dest.CategoryName,
        opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<ProductDTO, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        CreateMap<Shift, BoxReportViewModel>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.User.FullName));

        CreateMap<SupplierDTO, Supplier>().ReverseMap();
        CreateMap<SalesReturn, SalesReturnDTO>().ReverseMap();

        CreateMap<SalesReturnItem, SalesReturnItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ReverseMap();

        CreateMap<SaleItem, SalesReturnItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));
    }
}