﻿using Newtonsoft.Json;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.CartViewModel;
using RatioShop.Enums;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariant;
        private readonly IProductVariantCartService _productVariantCartService;
        private readonly IAddressService _addressService;
        private readonly ICartDiscountService _cartDiscountService;
        private readonly IShopUserService _shopUserService;
        private readonly IStockService _stockService;
        private readonly IProductVariantPackageService _productVariantPackageService;
        private readonly IPackageService _packageService;


        private readonly Guid _anonymousUserID = Guid.Parse(UserTest.UserAnonymousID);

        public CartService(ICartRepository CartRepository, IProductService productService, IProductVariantService productVariant, IProductVariantCartService productVariantCartService, IAddressService addressService, ICartDiscountService cartDiscountService, IShopUserService shopUserService, IStockService stockService, IProductVariantPackageService productVariantPackageService, IPackageService packageService)
        {
            _cartRepository = CartRepository;
            _productService = productService;
            _productVariant = productVariant;
            _productVariantCartService = productVariantCartService;
            _addressService = addressService;
            _cartDiscountService = cartDiscountService;
            _shopUserService = shopUserService;
            _stockService = stockService;
            _productVariantPackageService = productVariantPackageService;
            _packageService = packageService;
        }

        public Task<Cart> CreateCart(Cart Cart)
        {
            Cart.CreatedDate = DateTime.UtcNow;
            Cart.ModifiedDate = DateTime.UtcNow;
            return _cartRepository.CreateCart(Cart);
        }

        public bool DeleteCart(string id)
        {
            return _cartRepository.DeleteCart(id);
        }

        public IQueryable<Cart> GetCarts()
        {
            return _cartRepository.GetCarts();
        }

        public Cart? GetCart(string id)
        {
            return _cartRepository.GetCart(id);
        }

        public bool UpdateCart(Cart Cart)
        {
            Cart.ModifiedDate = DateTime.UtcNow;
            return _cartRepository.UpdateCart(Cart);
        }

        public async Task<AddToCartResponsetViewModel> AddToCart(AddToCartRequestViewModel request)
        {
            if (request == null || (request.VariantId == Guid.Empty && request.PackageId == Guid.Empty) || request.Number <= 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");
            ProductVariant? variant = null;
            ProductVariantCart? cartDetail = null;
            IEnumerable<ProductVariantPackage> packageItems = null;
            bool isAddPackage = false;

            // validate packages
            if (request.PackageId != null && request.PackageId != Guid.Empty)
            {
                isAddPackage = true;
                packageItems = _productVariantPackageService.GetProductVariantPackages().Where(x => x.PackageId == request.PackageId).ToList();
                if (packageItems == null || !packageItems.Any()) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");
                foreach (var item in packageItems)
                {
                    var packageItem = _productVariant.GetProductVariant(item.ProductVariantId.ToString());
                    if (packageItem == null || packageItem.Number <= 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");

                    var packageCartDetail = _productVariantCartService.GetProductVariantCarts().FirstOrDefault(x => x.CartId == request.CartId && x.ProductVariantId == item.ProductVariantId && x.PackageId == request.PackageId && x.ItemType == CartItemType.PackageItem);
                    if (packageItem.Number < (request.Number * item.ItemNumber)
                        || (packageCartDetail != null && packageItem.Number < (packageCartDetail.ItemNumber + (request.Number * item.ItemNumber))))
                        return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, $"Product in Packages out of stock. Product {packageItem.Product?.ProductFriendlyName} ({packageItem.Code}) remains: {packageItem.Number}");
                }
            }
            else
            {
                // validate product            
                if (request.VariantId == null || request.VariantId == Guid.Empty) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");
                variant = _productVariant.GetProductVariant(request.VariantId.ToString());
                if (variant == null || variant.Number <= 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");

                cartDetail = _productVariantCartService
                    .GetProductVariantCarts()
                    .FirstOrDefault(x => x.CartId == request.CartId 
                    && x.ProductVariantId == request.VariantId 
                    && x.ItemType == CartItemType.Product);
                if (variant.Number < request.Number
                    || (cartDetail != null && variant.Number < (cartDetail.ItemNumber + request.Number)))
                    return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, $"Out of stock. Product remains: {variant.Number}");
            }

            // save to DB
            //save cart
            var cart = GetCart(request.CartId.ToString());
            var currentUserId = request.UserId.ToString();
            var currentUser = _shopUserService.GetShopUser(currentUserId);

            if (cart == null)
                cart = await CreateCart(new Cart()
                {
                    Status = CommonStatus.CartStatus.Created,
                    ShopUserId = currentUserId,
                    AddressDetail = currentUser?.AddressDetail,
                    AddressId = currentUser?.AddressId,
                    FullName = currentUser?.FullName,
                    PhoneNumber = currentUser?.PhoneNumber,
                });
            else
            {
                if (Guid.Parse(cart.ShopUserId) == _anonymousUserID && request.UserId != _anonymousUserID)
                {
                    cart.ShopUserId = currentUserId;
                    // need test cart change or not
                    UpdateCart(cart);
                }
                var isChange = false;
                // phone number
                if (string.IsNullOrWhiteSpace(cart.PhoneNumber) && !string.IsNullOrWhiteSpace(currentUser?.PhoneNumber))
                {
                    isChange = true;
                    cart.PhoneNumber = currentUser?.PhoneNumber;
                }
                // full name
                if (string.IsNullOrWhiteSpace(cart.FullName) && !string.IsNullOrWhiteSpace(currentUser?.FullName))
                {
                    isChange = true;
                    cart.FullName = currentUser?.FullName;
                }
                // address id
                if ((cart.AddressId == null || cart.AddressId == 0) && (currentUser?.AddressId != null && currentUser?.AddressId != 0))
                {
                    isChange = true;
                    cart.AddressId = currentUser?.AddressId;
                }
                // address detail
                if (string.IsNullOrWhiteSpace(cart.AddressDetail) && !string.IsNullOrWhiteSpace(currentUser?.AddressDetail))
                {
                    isChange = true;
                    cart.AddressDetail = currentUser?.AddressDetail;
                }

                if (isChange) UpdateCart(cart);
            }

            //save productVariantCart
            if (isAddPackage)
            {
                foreach (var item in packageItems)
                {
                    var packageCartDetail = _productVariantCartService.GetProductVariantCarts()
                        .FirstOrDefault(x => x.CartId == request.CartId
                            && x.ProductVariantId == item.ProductVariantId
                            && x.PackageId == request.PackageId
                            && x.ItemType == CartItemType.PackageItem);

                    var packageItem = _productVariant.GetProductVariant(item.ProductVariantId.ToString());
                    var cartItem = packageCartDetail;
                    if (cartItem != null)
                    {
                        // update
                        cartItem.ItemNumber += (request.Number * item.ItemNumber);
                        cartItem.PackageNumber += request.Number;
                        cartItem.ItemPrice = packageItem?.Price;
                        cartItem.DiscountRate = packageItem?.DiscountRate ?? 0;
                        _productVariantCartService.UpdateProductVariantCart(cartItem);
                    }
                    else
                    {
                        //create
                        var itemAdd = new ProductVariantCart()
                        {
                            ItemNumber = request.Number * item.ItemNumber,
                            PackageNumber = request.Number,
                            ProductVariantId = item.ProductVariantId,
                            PackageId = item.PackageId,
                            CartId = cart.Id,
                            ItemPrice = packageItem?.Price,
                            DiscountRate = packageItem?.DiscountRate ?? 0,
                            ItemType = CartItemType.PackageItem
                        };
                        await _productVariantCartService.CreateProductVariantCart(itemAdd);
                    }
                }
                // build response
                var result = new AddToCartResponsetViewModel(cart.Id, CommonStatus.Success, "Add to cart success!");

                return result;
            }
            else
            {
                var cartItem = cartDetail;
                if (cartItem != null)
                {
                    // update
                    cartItem.ItemNumber += request.Number;
                    cartItem.ItemPrice = variant.Price;
                    cartItem.DiscountRate = variant.DiscountRate;
                    _productVariantCartService.UpdateProductVariantCart(cartItem);
                }
                else
                {
                    //create
                    var item = new ProductVariantCart()
                    {
                        ItemNumber = request.Number,
                        ProductVariantId = (Guid)request.VariantId,
                        CartId = cart.Id,
                        ItemPrice = variant.Price,
                        DiscountRate = variant.DiscountRate,
                        ItemType = CartItemType.Product
                    };
                    await _productVariantCartService.CreateProductVariantCart(item);
                }
                // build response
                var result = new AddToCartResponsetViewModel(cart.Id, CommonStatus.Success, "Add to cart success!");

                return result;
            }
        }

        public CartDetailResponsViewModel? GetCartDetail(Guid id, bool getLatestVariantPrice = true, bool includeInActiveDiscount = false, bool isMergePackageItem = false)
        {
            if (id == Guid.Empty) return null;

            var cartItems = GetCarts()
                .Join(_productVariantCartService.GetProductVariantCarts(),
                x => x.Id,
                y => y.CartId,
                (x, y) => new { cart = x, variantCart = y })
                .Where(z => z.cart.Id == id)
                .Select(cartDetail => new CartItemResponseViewModel()
                {
                    VariantId = cartDetail.variantCart.ProductVariantId,
                    PackageId = cartDetail.variantCart.PackageId,
                    ItemType = cartDetail.variantCart.PackageId != null ? CartItemType.PackageItem : CartItemType.Product,
                    Number = cartDetail.variantCart.ItemNumber,
                    PackageNumber = cartDetail.variantCart.PackageNumber,
                    Price = cartDetail.variantCart.ItemPrice,
                    DiscountRate = cartDetail.variantCart.DiscountRate,
                    StockItems = string.IsNullOrWhiteSpace(cartDetail.variantCart.StockItems) ? null : JsonConvert.DeserializeObject<List<CartStockItem>>(cartDetail.variantCart.StockItems),

                }).ToList();

            foreach (var item in cartItems)
            {
                var productVariantDetail = _productVariant.GetProductVariant(item.VariantId.ToString());
                if (productVariantDetail != null)
                {
                    item.Price = getLatestVariantPrice ? productVariantDetail.Price : item.Price != null ? item.Price : productVariantDetail.Price;
                    item.DiscountRate = getLatestVariantPrice ? productVariantDetail.DiscountRate : item.DiscountRate != null ? item.DiscountRate : productVariantDetail.DiscountRate;
                    item.DiscountPrice = item.Price * (decimal)(100 - (item.DiscountRate ?? 0)) / 100;
                    item.VariableName = productVariantDetail.Code;
                    // product info
                    var productDetail = _productService.GetProduct(productVariantDetail.ProductId);
                    if (productDetail != null)
                    {
                        item.ProductCode = productDetail.Product.Code;
                        item.Name = productDetail.Product.ProductFriendlyName;
                        item.Image = productDetail.ProductDefaultImage;
                        item.EnableStockTracking = productDetail.Product.EnableStockTracking;
                    }
                }
            }

            // merge package item into 1
            if (isMergePackageItem)
            {
                var cartPackageItem = cartItems.Where(x => x.ItemType == CartItemType.PackageItem).ToList();
                foreach (var item in cartPackageItem)
                {
                    var isExist = cartItems.FirstOrDefault(x => x.ItemType == CartItemType.Package && x.PackageId == item.PackageId);
                    if (isExist == null)
                    {
                        if (item.PackageId == null) continue;
                        var packageInfo = _packageService.GetPackageViewModel((Guid)item.PackageId);
                        if (packageInfo == null) continue;

                        cartItems.Add(new CartItemResponseViewModel
                        {
                            PackageId = item.PackageId,
                            ItemType = CartItemType.Package,
                            Number = item.PackageNumber,
                            PackageNumber = item.PackageNumber,
                            Price = packageInfo.ManualPrice != null ? packageInfo.ManualPrice : packageInfo.AutoCalculatedPrice,
                            DiscountPrice = packageInfo.ManualPrice != null ? packageInfo.ManualPrice : packageInfo.AutoCalculatedPrice,
                            DiscountRate = packageInfo.ManualPrice == null ? (packageInfo.DiscountRate ?? 0) : 0,
                            Name = packageInfo.Name,
                            ProductCode = packageInfo.Code,
                            Image = packageInfo.ImageUrl,
                            Description = packageInfo.PackageItems != null && packageInfo.PackageItems.Any()
                                        ? String.Join(", ", packageInfo.PackageItems.Select(x => $"{x.Name} x {x.Number}"))
                                        : ""
                        });
                    }
                    cartItems.Remove(item);
                }
            }

            var currentCart = GetCart(id.ToString());
            DefaultShippingAddressViewModel defaultShippingAddress = null;
            if (currentCart != null && !currentCart.ShopUserId.Equals(UserTest.UserAnonymousID))
            {
                currentCart.ShopUser = _shopUserService.GetShopUser(currentCart.ShopUserId);
                defaultShippingAddress = new DefaultShippingAddressViewModel
                {
                    ShippingAddressId = currentCart?.ShopUser?.AddressId ?? null,
                    ShippingAddressDetail = currentCart?.ShopUser?.AddressDetail,
                    FullName = currentCart?.ShopUser?.FullName,
                    PhoneNumber = currentCart?.ShopUser?.PhoneNumber,
                };
            }
            var couponCodes = _cartDiscountService.GetListCouponApplyByCartId(id, includeInActiveDiscount).ToList();
            var totalItemPrice = cartItems?.Sum(x => (x.DiscountPrice * x.Number)) ?? 0;
            var shippingAddress = _addressService.GetAddress(currentCart?.AddressId ?? 0);
            var shippingFee = cartItems?.Count == 0 ? 0 : shippingAddress?.ShippingFee ?? null;

            var cartDetail = new CartDetailResponsViewModel()
            {
                CartId = id,
                CartItems = cartItems,
                TotalItems = cartItems?.Sum(x => x.Number) ?? 0,
                TotalPrice = totalItemPrice,
                CouponCodes = couponCodes,
                Status = currentCart?.Status ?? string.Empty,
                ShippingAddressId = currentCart?.AddressId,
                ShippingAddressDetail = currentCart?.AddressDetail,
                ShippingAddress1 = shippingAddress?.Address1,
                ShippingAddress2 = shippingAddress?.Address2,
                FullShippingAddress = $"{currentCart?.AddressDetail} - {shippingAddress?.Address2} - {shippingAddress?.Address1}",
                ShippingFee = shippingFee,
                TotalFinalPrice = CalculateFinalPrice(totalItemPrice, couponCodes, shippingFee),
                FullName = currentCart?.FullName,
                PhoneNumber = currentCart?.PhoneNumber,
                ShippingAddressDefault = defaultShippingAddress,
                IsEnoughShippingInformation = CheckIsEnoughShippingInformation(currentCart)
            };

            return cartDetail;
        }

        private bool CheckIsEnoughShippingInformation(Cart? cart)
        {
            if (cart == null
                || string.IsNullOrWhiteSpace(cart.AddressDetail)
                || cart.AddressId == null || cart.AddressId == 0
                || string.IsNullOrWhiteSpace(cart.FullName)
                || string.IsNullOrWhiteSpace(cart.PhoneNumber)) return false;
            return true;
        }

        public AddToCartResponsetViewModel ChangeCartItem(AddToCartRequestViewModel request)
        {
            if (request == null || (request.VariantId == Guid.Empty && request.PackageId == Guid.Empty) || request.Number < 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");
            ProductVariant? variant = null;
            ProductVariantCart? cartDetail = null;
            IEnumerable<ProductVariantPackage> packageItems = null;
            bool isAddPackage = false;

            // validate packages
            if (request.PackageId != null && request.PackageId != Guid.Empty)
            {
                isAddPackage = true;
                packageItems = _productVariantPackageService.GetProductVariantPackages().Where(x => x.PackageId == request.PackageId).ToList();
                if (packageItems == null || !packageItems.Any()) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");
                foreach (var item in packageItems)
                {
                    var packageItem = _productVariant.GetProductVariant(item.ProductVariantId.ToString());
                    if (packageItem == null || packageItem.Number < 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request");

                    if (packageItem.Number < (request.Number * item.ItemNumber))
                        return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, $"Product in Packages out of stock. Product {packageItem.Product?.ProductFriendlyName} ({packageItem.Code}) remains: {packageItem.Number}");
                }
            }
            else
            {
                // validate product            
                if (request == null || request.VariantId == null || request.VariantId == Guid.Empty || request.Number < 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request. Can not change cart item!");
                variant = _productVariant.GetProductVariant(request.VariantId.ToString());
                if (variant == null || variant.Number < 0) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Bad request. Can not change cart item!");
                if (variant.Number < request.Number) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, $"Too much products. Product remains: {variant.Number}!");
            }

            // save to DB
            //save cart
            var cart = GetCart(request.CartId.ToString());
            if (cart == null) return new AddToCartResponsetViewModel(Guid.Empty, CommonStatus.Failure, "Cart empty!");

            if (isAddPackage)
            {
                foreach (var item in packageItems)
                {
                    var packageCartDetail = _productVariantCartService.GetProductVariantCarts()
                        .FirstOrDefault(x => x.CartId == request.CartId
                            && x.ProductVariantId == item.ProductVariantId
                            && x.PackageId == request.PackageId
                            && x.ItemType == CartItemType.PackageItem);

                    var packageItem = _productVariant.GetProductVariant(item.ProductVariantId.ToString());
                    var cartItem = packageCartDetail;
                    if (cartItem != null)
                    {
                        if (request.Number == 0) _productVariantCartService.DeleteProductVariantCart(cartItem.Id.ToString());
                        else
                        {
                            cartItem.ItemNumber = (request.Number * item.ItemNumber);
                            cartItem.PackageNumber = request.Number;
                            cartItem.ItemPrice = packageItem?.Price;
                            cartItem.DiscountRate = packageItem?.DiscountRate ?? 0;
                            _productVariantCartService.UpdateProductVariantCart(cartItem);
                        }
                    }
                }

                var result = new AddToCartResponsetViewModel(cart.Id, CommonStatus.Success, "Add to cart success!");

                return result;
            }
            else
            {
                //save productVariantCart
                var cartItem = _productVariantCartService.GetProductVariantCarts()
                    .FirstOrDefault(x => x.CartId == cart.Id
                    && x.ProductVariantId == request.VariantId && x.ItemType == CartItemType.Product);
                if (cartItem != null)
                {
                    // update
                    cartItem.ItemNumber = request.Number;
                    cartItem.ItemPrice = variant.Price;
                    cartItem.DiscountRate = variant.DiscountRate;
                    if (cartItem.ItemNumber == 0) _productVariantCartService.DeleteProductVariantCart(cartItem.Id.ToString());
                    else _productVariantCartService.UpdateProductVariantCart(cartItem);
                }

                // build response
                var result = new AddToCartResponsetViewModel(cart.Id, CommonStatus.Success, "Add to cart success!");

                return result;
            }
        }
        private decimal CalculateFinalPrice(decimal totalPrice, List<string> coupons, decimal? shippingFee)
        {
            var finalPrice = totalPrice;
            var shipFee = shippingFee ?? 0;
            var discounts = _cartDiscountService.GetDicountsByCouponsCode(coupons);

            var perecentDiscount = discounts.FirstOrDefault(x => x.DiscountType == DiscountType.Percent);
            var directDiscount = discounts.Where(x => x.DiscountType == DiscountType.DirectValue);
            var shippingDiscount = discounts.FirstOrDefault(x => x.DiscountType == DiscountType.Freeship);
            var partialShippingDiscount = discounts.FirstOrDefault(x => x.DiscountType == DiscountType.PartialFreeship);

            // calculate
            if (perecentDiscount != null) finalPrice = finalPrice * (100 - perecentDiscount.Value) / 100;
            foreach (var item in directDiscount)
            {
                finalPrice -= item.Value;
            }

            if (partialShippingDiscount != null) shipFee -= partialShippingDiscount.Value;
            if (shippingDiscount != null) shipFee = 0;

            if (finalPrice < 0) finalPrice = 0;
            if (shipFee < 0) shipFee = 0;

            finalPrice += shipFee;

            return finalPrice;
        }

        public bool UpdateCartUserByUserId(string cartId, string userId)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(userId)) return false;

            var existingCart = GetCarts().FirstOrDefault(x => x.ShopUserId.Equals(userId) && x.Status.Equals(CommonStatus.CartStatus.Created));
            if (existingCart != null) return false;

            var cart = GetCart(cartId);

            if (cart != null && cart.Status.Equals(CommonStatus.CartStatus.Created) && cart.ShopUserId == UserTest.UserAnonymousID)
            {
                cart.ShopUserId = userId;
                return UpdateCart(cart);
            }

            return false;
        }

        public Cart? GetCartByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var cart = GetCarts().FirstOrDefault(x => x.ShopUserId.Equals(userId) && x.Status.Equals(CommonStatus.CartStatus.Created));
            return cart;
        }

        public bool UpdateCartStatus(string cartId, string status)
        {
            var cart = GetCart(cartId);
            if (cart == null) return false;

            cart.Status = status;
            return UpdateCart(cart);
        }

        public bool TrackingProductItemByCart(CartDetailResponsViewModel? cartDetail, Guid cartId)
        {
            if (cartId == Guid.Empty || cartDetail == null || cartDetail.CartItems == null || !cartDetail.CartItems.Any()) return true;

            foreach (var item in cartDetail.CartItems)
            {
                if (!item.EnableStockTracking) continue;
                // should reduce 
                var reduceStatus = _productVariant.ReduceProductVariantNumber(item.VariantId, item.Number, item.StockItems);
                if (reduceStatus)
                {
                    var cartItem = _productVariantCartService.GetProductVariantCarts().FirstOrDefault(x => x.CartId == cartId && x.ProductVariantId == item.VariantId && x.PackageId == item.PackageId);
                    if (cartItem != null)
                    {
                        // update reduce status
                        cartItem.StockTrackingStatus = CommonStatus.Tracking.Active;
                        cartItem.TrackUpdated = true;
                        cartItem.StockItems = JsonConvert.SerializeObject(item.StockItems);

                        var updateVariantCartStatus = _productVariantCartService.UpdateProductVariantCart(cartItem);
                        if (!updateVariantCartStatus) return false;
                    }
                }
                else return false;
            }
            return true;
        }

        public bool CompleteCartBeingRevertStock(Guid cartId)
        {
            if (cartId == Guid.Empty) return false;

            var variantCarts = _productVariantCartService.GetProductVariantCarts().Where(x => x.CartId == cartId && x.IsReverted && x.TrackUpdated && x.StockTrackingStatus == CommonStatus.Tracking.Active).ToList();
            if (variantCarts != null && variantCarts.Any())
            {
                foreach (var item in variantCarts)
                {
                    var reduceStatus = _productVariant.ReduceProductVariantNumber(item.ProductVariantId, item.ItemNumber, JsonConvert.DeserializeObject<List<CartStockItem>>(item.StockItems));
                    if (reduceStatus)
                    {
                        item.IsReverted = false;
                        var updateVariantCartStatus = _productVariantCartService.UpdateProductVariantCart(item);
                        if (!updateVariantCartStatus) return false;
                    }
                }
            }

            return true;
        }

        public bool RevertTrackingProductItemByCart(Guid cartId)
        {
            if (cartId == Guid.Empty) return false;

            var trackUpdatedItems = _productVariantCartService.GetProductVariantCarts().Where(x => x.CartId == cartId && x.TrackUpdated && x.StockTrackingStatus == CommonStatus.Tracking.Active && !x.IsReverted).ToList();
            if (trackUpdatedItems == null || !trackUpdatedItems.Any()) return true;

            foreach (var item in trackUpdatedItems)
            {
                if (string.IsNullOrWhiteSpace(item.StockItems)) continue;

                var cartStockItems = JsonConvert.DeserializeObject<List<CartStockItem>>(item.StockItems);
                var revertStatus = _productVariant.RevertProductVariantNumber(item.ProductVariantId, cartStockItems);

                if (revertStatus)
                {
                    item.IsReverted = true;
                    var updateVariantCart = _productVariantCartService.UpdateProductVariantCart(item);
                    if (!updateVariantCart) return false;
                }
                else return false;
            }

            return true;
        }

        /// <summary>
        /// Only call this function when checkout step.
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public bool UpdateStoreItemsForCart(Guid cartId, ref CartDetailResponsViewModel? cartDetail)
        {
            if (cartId == Guid.Empty) return false;
            if (cartDetail == null) return true;

            var addressId = GetCart(cartId.ToString())?.AddressId;
            if (addressId == null || addressId == 0) return false;

            var shippingAddress = _addressService.GetAddress((int)addressId);
            if (shippingAddress == null) return false;

            var stocksSorted = _stockService.GetStocks().AsQueryable()
                .Join(_addressService.GetAddresses(),
                x => x.AddressId,
                y => y.Id, (x, y) => new { Stocks = x, Address1 = y.Address1, Address2 = y.Address2 })
                .Where(x => x.Stocks.IsActive)
                .OrderByDescending(x => x.Address1 == shippingAddress.Address1)
                .ThenBy(x => x.Address1)
                .Select(x => x.Stocks)
                .ToList();

            if (stocksSorted == null) return false;

            var variantStocksSorted = _productVariant.GetVariantStocksOrderByStocks(stocksSorted);

            // find nearest stocks have all item
            // => has value => update stockitems to each item in cart.
            // if empty => find all nearest stock has much item as posible
            // => foreach => add stockitems to each item in cart.

            if (cartDetail.CartItems == null || !cartDetail.CartItems.Any()) return false;

            var cartItemsReduced = new List<CartItemResponseViewModel>();
            foreach (var cartItem in cartDetail.CartItems)
            {
                if (!cartItem.EnableStockTracking) continue;
                if (cartItemsReduced.Select(x => x.VariantId).Contains(cartItem.VariantId)) continue;

                var cartItemNumber = cartDetail.CartItems.Where(x => x.VariantId == cartItem.VariantId).Sum(x => x.Number);
                cartItemsReduced.Add(cartItem);

                var currentStockItems = new List<CartStockItem>();
                var itemInStocks = variantStocksSorted.Where(x => x.ProductVariantId == cartItem.VariantId);
                foreach (var variantStockItem in itemInStocks)
                {
                    if (cartItemNumber <= variantStockItem.ProductNumber)
                    {
                        currentStockItems.Add(new CartStockItem { ItemNumber = cartItemNumber, StockId = variantStockItem.StockId });
                        cartItem.StockItems = currentStockItems;
                        break;
                    }
                    else
                    {
                        currentStockItems.Add(new CartStockItem { ItemNumber = variantStockItem.ProductNumber, StockId = variantStockItem.StockId });
                        cartItemNumber -= variantStockItem.ProductNumber;
                    }
                }
            }

            return true;
        }

        public bool ValidateItemsOnCartWhenCheckout(CartDetailResponsViewModel? cartDetail)
        {
            if (cartDetail == null || cartDetail.TotalItems == 0 || cartDetail.CartItems == null || !cartDetail.CartItems.Any()) return false;

            foreach (var item in cartDetail.CartItems)
            {
                var productVariant = _productVariant.GetProductVariant(item.VariantId.ToString());
                var totalProductNumber = cartDetail.CartItems.Where(x => x.VariantId == item.VariantId).Sum(x => x.Number);

                if (productVariant == null) return false;

                if (totalProductNumber > productVariant.Number) return false;
            }

            return true;
        }

        public IEnumerable<Cart> GetAllCartByUserId(string userId)
        {
            if (userId == null) return Enumerable.Empty<Cart>();

            return GetCarts().Where(x => x.ShopUserId.Equals(userId));
        }

        public bool UpdateCartItemPriceAndDiscountRate(CartDetailResponsViewModel cartDetail, Guid cartId)
        {
            if (cartId == Guid.Empty || cartDetail == null || cartDetail.CartItems == null || !cartDetail.CartItems.Any()) return true;

            foreach (var item in cartDetail.CartItems)
            {
                var cartItem = _productVariantCartService.GetProductVariantCarts().FirstOrDefault(x => x.CartId == cartId && x.ProductVariantId == item.VariantId && x.PackageId == item.PackageId);
                if (cartItem != null)
                {
                    if (cartItem.DiscountRate == item.DiscountRate && cartItem.ItemPrice == item.Price) continue;
                    cartItem.DiscountRate = item.DiscountRate;
                    cartItem.ItemPrice = item.Price;

                    var updateVariantCartStatus = _productVariantCartService.UpdateProductVariantCart(cartItem);
                    if (!updateVariantCartStatus) return false;
                }
            }
            return true;
        }
    }
}
