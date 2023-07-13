using AutoMapper;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.OrdersViewModel;
using RatioShop.Data.ViewModels.ShipmentViewModel;
using RatioShop.Data.ViewModels.User;

namespace RatioShop.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartDetailResponsViewModel, CartDetailViewModel>();
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(x=>x));
            CreateMap<OrderViewModel, Order>();
            CreateMap<UserViewModel, UserResponseViewModel>()                
                .ForMember(dest => dest.Address, opt => opt.MapFrom(x => x.User.Address))
                .ForPath(dest => dest.Address.AddressDetail, opt => opt.MapFrom(x => x.DefaultShippingAddress))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber));
            CreateMap<ListOrderViewModel, OrdersResponseViewModel>();

            CreateMap<Order, OrderResponseViewModel>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(x => x.Id));
            CreateMap<OrderViewModel, OrderResponseViewModel>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(x => x.Order.Id))
                .ForMember(dest => dest.CartDetail, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentHistory, opt => opt.Ignore());
            CreateMap<CartDetailViewModel, CartDetailResponsViewModel>();
            
            CreateMap<Payment, PaymentResponseViewModel>();
            CreateMap<ShopUser, UserResponseViewModel>();            
            CreateMap<Shipment, ShipmentResponseViewModel>()
                .ForMember(dest => dest.Shipper, opt => opt.MapFrom(x => x.Shipper));            
        }
    }
}
