using AutoMapper;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models;
using RatioShop.Areas.Admin.Models.SiteSettings;
using RatioShop.Areas.Admin.Models.User;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Data.ViewModels.MyAccountViewModel;
using RatioShop.Data.ViewModels.OrdersViewModel;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Data.ViewModels.ShipmentViewModel;
using RatioShop.Data.ViewModels.User;
using RatioShop.Helpers;
using RatioShop.Helpers.SiteSettingsHelper;

namespace RatioShop.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // user
            CreateMap<ShopUser, UserResponseViewModel>();
            CreateMap<ShopUser, BaseUserViewModel>()
                .ForMember(dest => dest.ShippingAddress1, opt => opt.MapFrom(x => x.Address.Address1))
                .ForMember(dest => dest.ShippingAddress2, opt => opt.MapFrom(x => x.Address.Address2))
                .ForMember(dest => dest.FullShippingAddress, opt => opt.MapFrom(x => $"{x.AddressDetail} - {x.Address.Address2} - {x.Address.Address1}"));
            CreateMap<ShopUser, EmployeeViewModel>()
                .ForMember(dest => dest.ShippingAddress1, opt => opt.MapFrom(x => x.Address.Address1))
                .ForMember(dest => dest.ShippingAddress2, opt => opt.MapFrom(x => x.Address.Address2))
                .ForMember(dest => dest.FullShippingAddress, opt => opt.MapFrom(x => $"{x.AddressDetail} - {x.Address.Address2} - {x.Address.Address1}"));
            CreateMap<EmployeeViewModel, ShopUser>();
            CreateMap<UserViewModel, UserResponseViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(x => x.User.Address))
                .ForPath(dest => dest.Address.AddressDetail, opt => opt.MapFrom(x => x.DefaultShippingAddress))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber));

            // cart detail
            CreateMap<CartDetailResponsViewModel, CartDetailViewModel>();
            CreateMap<CartDetailViewModel, CartDetailResponsViewModel>();

            // order
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(x => x));
            CreateMap<Order, OrderResponseViewModel>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(x => x.Id));
            CreateMap<OrderViewModel, OrderResponseViewModel>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(x => x.Order.Id))
                .ForMember(dest => dest.CartDetail, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentHistory, opt => opt.Ignore());
            CreateMap<ListOrderViewModel, OrdersResponseViewModel>();
            CreateMap<ListOrderViewModel, ListShipmentViewModel>()
                .ForMember(dest => dest.Orders, opt => opt.Ignore());

            // shipment
            CreateMap<Shipment, ShipmentResponseViewModel>()
                .ForMember(dest => dest.Shipper, opt => opt.MapFrom(x => x.Shipper));

            // payment
            CreateMap<Payment, PaymentResponseViewModel>();

            // search
            CreateMap<BaseSearchArgs, BaseSearchRequest>()
                .ForMember(dest => dest.FilterItems, opt => opt.MapFrom(x => JsonConvert.DeserializeObject<IEnumerable<FacetFilterItem>>(x.FilterItems)));
            CreateMap<ProductSearchRequest, BaseSearchRequest>()
                .ForMember(dest => dest.FilterItems, opt => opt.MapFrom(x => JsonConvert.DeserializeObject<IEnumerable<FacetFilterItem>>(x.FilterItems)));

            // products
            CreateMap<ListProductViewModel, ListProductResponseViewModel>();

            // package
            CreateMap<PackageViewModel, Package>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(x => x.ManualPrice));
            CreateMap<Package, PackageViewModel>()
                .ForMember(dest => dest.ManualPrice, opt => opt.MapFrom(x => x.Price))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(x => x.Image.ResolvePackageImages().FirstOrDefault()));

            CreateMap<ListPackageViewModel, ListProductViewModel>();
            
            // product variant
            CreateMap<ProductVariant, ProductVariantViewModel>()
                .ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(x => x.Price * (decimal)(100 - (x.DiscountRate ?? 0)) / 100))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Product.ProductFriendlyName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.Images) ? x.Images.ResolveProductImages().FirstOrDefault() : x.Product.ProductImage.ResolveProductImages().FirstOrDefault()));

            // site setting            
            CreateMap<SiteSetting, SiteSettingDetailViewModel>();                

            CreateMap<SiteSettingDetailViewModel, SiteSetting>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(x => MappingSettingTypeHelper.MappingSettingValue(x)));
                                     
        }
    }
}
