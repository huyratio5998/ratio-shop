using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;

namespace RatioShop.Services.Abstract
{
    public interface ICartService
    {
        IQueryable<Cart> GetCarts();
        Cart? GetCart(string id);
        Task<Cart> CreateCart(Cart Cart);
        bool UpdateCart(Cart Cart);
        bool DeleteCart(string id);

        // Addition logics
        Task<AddToCartResponsetViewModel> AddToCart(AddToCartRequestViewModel request);
        AddToCartResponsetViewModel ChangeCartItem(AddToCartRequestViewModel request);
        CartDetailResponsViewModel? GetCartDetail(Guid id, bool getLatestVariantPrice = true, bool includeInActiveDiscount = false);
        bool UpdateCartUserByUserId(string cartId, string userId);
        Cart? GetCartByUserId(string userId);
        IEnumerable<Cart> GetAllCartByUserId(string userId);
        bool UpdateCartStatus(string cartId, string status);
        bool UpdateCartItemPriceAndDiscountRate(CartDetailResponsViewModel cartDetail, Guid cartId);
        bool TrackingProductItemByCart(CartDetailResponsViewModel cartDetail, Guid cartId);
        bool CompleteCartBeingRevertStock(Guid cartId);
        bool RevertTrackingProductItemByCart(Guid cartId);
        bool UpdateStoreItemsForCart(Guid cartId, ref CartDetailResponsViewModel? cartDetail);
        bool ValidateItemsOnCartWhenCheckout(CartDetailResponsViewModel? cartDetail);
    }
}
