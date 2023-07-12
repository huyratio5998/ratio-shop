using AutoMapper;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.User;

namespace RatioShop.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartDetailResponsetViewModel, CartDetailViewModel>();
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(x=>x));
            CreateMap<OrderViewModel, Order>();
            CreateMap<UserViewModel, UserResponseViewModel>()                
                .ForMember(dest => dest.Address, opt => opt.MapFrom(x => x.User.Address))
                .ForPath(dest => dest.Address.AddressDetail, opt => opt.MapFrom(x => x.DefaultShippingAddress))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber));
            

        }
    }
}
