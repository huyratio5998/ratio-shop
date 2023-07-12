using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.Cart;

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
        CartDetailResponsetViewModel? GetCartDetail(Guid id);
        bool UpdateCartUserByUserId(string cartId, string userId);
        Cart? GetCartByUserId(string userId);
        IEnumerable<Cart> GetAllCartByUserId(string userId);
        bool UpdateCartStatus(string cartId, string status);
        bool TrackingProductItemByCart(CartDetailResponsetViewModel cartDetail, Guid cartId);
        bool RevertTrackingProductItemByCart(Guid cartId);
        bool UpdateStoreItemsForCart(Guid cartId, ref CartDetailResponsetViewModel? cartDetail);
        bool ValidateItemsOnCartWhenCheckout(CartDetailResponsetViewModel? cartDetail);
    }
}
