using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RatioShop.Constants;
using RatioShop.Data;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.Repository.Implement;
using RatioShop.Data.ViewModels;
using RatioShop.Enums;
using RatioShop.Services.Abstract;
using RatioShop.Services.Implement;

namespace RatioShop.Test.UnitTest.Service
{
    [TestClass]
    public class CartServiceTest
    {
        private ICartService _cartService;

        private Mock<ICartRepository> _cartRepository;

        private Mock<IProductService> _productService;
        private Mock<IProductVariantService> _productVariant;
        private Mock<IProductVariantCartService> _productVariantCartService;
        private Mock<IAddressService> _addressService;
        private Mock<ICartDiscountService> _cartDiscountService;
        private Mock<IShopUserService> _shopUserService;
        private Mock<IStockService> _stockService;
        private Mock<IProductVariantPackageService> _productVariantPackageService;
        private Mock<IPackageService> _packageService;

        public CartServiceTest()
        {
            _cartRepository = new Mock<ICartRepository>();
            _productService = new Mock<IProductService>();
            _productVariant = new Mock<IProductVariantService>();
            _addressService = new Mock<IAddressService>();
            _cartDiscountService = new Mock<ICartDiscountService>();
            _productVariantCartService = new Mock<IProductVariantCartService>();
            _shopUserService = new Mock<IShopUserService>();
            _stockService = new Mock<IStockService>();
            _productVariantPackageService = new Mock<IProductVariantPackageService>();
            _packageService = new Mock<IPackageService>();

            _cartService = new CartService(_cartRepository.Object,
                _productService.Object,
                _productVariant.Object,
                _productVariantCartService.Object,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);
        }

        private async Task<ApplicationDbContext> GetDbContext(
            List<ProductVariant>? productVariants = null,
            List<Cart>? carts = null,
            List<ProductVariantCart>? productVariantCarts = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataBaseContext = new ApplicationDbContext(options);
            dataBaseContext.Database.EnsureCreated();

            // product variant
            if (productVariants != null && productVariants.Any())
            {
                await dataBaseContext.ProductVariant.AddRangeAsync(productVariants);
            };

            // cart
            if (carts != null && carts.Any())
            {
                await dataBaseContext.Cart.AddRangeAsync(carts);
            };

            // product variant cart
            if (productVariantCarts != null && productVariantCarts.Any())
            {
                await dataBaseContext.Set<ProductVariantCart>().AddRangeAsync(productVariantCarts);
            };

            await dataBaseContext.SaveChangesAsync();

            return dataBaseContext;
        }

        [TestMethod]
        public async Task AddToCart_RequestEmpty_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel();

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_WrongVariantId_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 1
            };

            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns((ProductVariant)null);
            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_VariantOutOfStock_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 1
            };

            var variant = new ProductVariant()
            {
                Number = 0
            };
            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns(variant);
            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_RequestVariantNumberBiggerThanCurrentNumber_ReturnsStatus()
        {
            // Arrange            
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 3
            };

            var variant = new ProductVariant()
            {
                Id = Guid.NewGuid(),
                Number = 2
            };
            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns(variant);

            _productVariantCartService.Setup(x => x.GetProductVariantCarts())
                .Returns(Enumerable.Empty<ProductVariantCart>().AsQueryable());

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_TotalRequestVariantNumberRequestBiggerThanCurrentNumber_ReturnsStatus()
        {
            // Arrange            
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 1
            };

            // mock return
            var variant = new ProductVariant { Id = (Guid)request.VariantId, Number = 2 };
            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns(variant);

            var productVariantCart = new ProductVariantCart()
            {
                Id = Guid.NewGuid(),
                CartId = (Guid)request.CartId,
                ProductVariantId = variant.Id,
                ItemType = CartItemType.Product,
                ItemNumber = 2
            };

            // use dbContext
            var dbContext = await GetDbContext(null, null, new List<ProductVariantCart>() { productVariantCart });
            var productVariantCarts = dbContext.Set<ProductVariantCart>().AsQueryable();

            _productVariantCartService.Setup(
                x => x.GetProductVariantCarts())
                .Returns(productVariantCarts);

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_NewCartItemToNewCart_ReturnsStatus()
        {
            // Arrange            
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 1,
                UserId = Guid.NewGuid(),
            };

            var dbContext = await GetDbContext();
            // use dbContext
            // cart
            var cartRepository = new CartRepository(dbContext);

            // product variant cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(
                cartRepository,
                _productService.Object,
                _productVariant.Object,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            // mock return
            var variant = new ProductVariant { Id = (Guid)request.VariantId, Number = 2, Price = 50000000, DiscountRate = 5 };
            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns(variant);

            var productVariantCarts = Enumerable.Empty<ProductVariantCart>().AsQueryable();
            _productVariantCartService.Setup(
                x => x.GetProductVariantCarts())
                .Returns(productVariantCarts);

            _cartRepository.Setup(x => x.GetCart(request.CartId.ToString())).Returns((Cart)null);

            _shopUserService.Setup(x => x.GetShopUser(request.UserId.ToString())).Returns(new ShopUser
            {
                Id = request.UserId.ToString(),
            });

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Success);

            dbContext.Set<ProductVariantCart>().FirstOrDefault(x => x.ProductVariantId == request.VariantId).Should().NotBeNull();
        }

        [TestMethod]
        public async Task AddToCart_ExistCartItemToCart_ReturnsStatus()
        {
            // Arrange            
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                VariantId = Guid.NewGuid(),
                Number = 1,
                UserId = Guid.NewGuid(),
            };

            // init data in dbcontext
            var newCart = new Cart
            {
                Id = (Guid)request.CartId,
                ShopUserId = request.UserId.ToString()
            };

            var newProductVariantCart = new ProductVariantCart
            {
                Id = Guid.NewGuid(),
                ProductVariantId = (Guid)request.VariantId,
                CartId = (Guid)request.CartId,
                ItemNumber = 1,
                ItemType = CartItemType.Product
            };

            var dbContext = await GetDbContext(null, new List<Cart> { newCart }, new List<ProductVariantCart> { newProductVariantCart });

            // use dbContext
            // cart
            var cartRepository = new CartRepository(dbContext);

            // product varint cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(
                cartRepository,
                _productService.Object,
                _productVariant.Object,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            dbContext.ChangeTracker.Clear();

            // mock return
            // variant
            var variant = new ProductVariant { Id = (Guid)request.VariantId, Number = 3, Price = 50000000, DiscountRate = 5 };
            _productVariant.Setup(x => x.GetProductVariant(request.VariantId.ToString(), true)).Returns(variant);

            // user
            _shopUserService.Setup(x => x.GetShopUser(request.UserId.ToString())).Returns(new ShopUser
            {
                Id = request.UserId.ToString(),
            });

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Success);

            var addedItem = dbContext.Set<ProductVariantCart>().FirstOrDefault(x => x.ProductVariantId == request.VariantId && x.CartId == request.CartId);
            addedItem.Should().NotBeNull();
            addedItem?.ItemNumber.Should().Be(2);
        }

        // package
        [TestMethod]
        public async Task AddToCart_WrongPackageId_ReturnsStatus()
        {
            // Arrange            
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                PackageId = Guid.NewGuid(),
                Number = 1
            };

            _productVariantPackageService.Setup(x => x.GetProductVariantPackages()).Returns((Enumerable.Empty<ProductVariantPackage>()));

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_PackageOutOfStock_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                PackageId = Guid.NewGuid(),
                Number = 2
            };

            // init data in dbcontext
            var productVariantId = Guid.NewGuid();
            var newProductVariant = new ProductVariant
            {
                Id = productVariantId,
                Number = 3,
            };

            var dbContext = await GetDbContext(new List<ProductVariant> { newProductVariant });

            // use dbContext
            // product variant
            var productVariantRepository = new ProductVariantRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productVariantStockRepository = new ProductVariantStockRepository(dbContext);
            var productVariantStockService = new ProductVariantStockService(productVariantStockRepository);
            IMapper mapper = null;
            var productVariantService = new ProductVariantService(productVariantRepository, productRepository, productVariantStockService, mapper);

            // product variant cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(_cartRepository.Object,
                _productService.Object,
                productVariantService,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            // mock return
            // package items
            var packages = new List<ProductVariantPackage>()
            {
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 2,
                    }
            };
            _productVariantPackageService.Setup(x => x.GetProductVariantPackages()).Returns(packages);

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_TotalPackageNumberBiggerThanCurrentNumber_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel()
            {
                CartId = Guid.NewGuid(),
                PackageId = Guid.NewGuid(),
                Number = 2
            };

            var productVariantId = Guid.NewGuid();

            // init data in dbcontext
            var newProductVariant = new ProductVariant
            {
                Id = productVariantId,
                Number = 4,
            };

            var newProductVariantCart = new ProductVariantCart
            {
                Id = Guid.NewGuid(),
                CartId = (Guid)request.CartId,
                ProductVariantId = productVariantId,
                ItemNumber = 2,
                PackageId = request.PackageId,
                PackageNumber = 1,
                ItemType = CartItemType.PackageItem,
            };

            var dbContext = await GetDbContext(new List<ProductVariant> { newProductVariant }, null, new List<ProductVariantCart> { newProductVariantCart });

            // use new dbContext
            // product variant
            var productVariantRepository = new ProductVariantRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productVariantStockRepository = new ProductVariantStockRepository(dbContext);
            var productVariantStockService = new ProductVariantStockService(productVariantStockRepository);
            IMapper mapper = null;
            var productVariantService = new ProductVariantService(productVariantRepository, productRepository, productVariantStockService, mapper);

            // product variant cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(_cartRepository.Object,
                _productService.Object,
                productVariantService,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            // mock return
            // package items
            var packages = new List<ProductVariantPackage>()
            {
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 2,
                    }
            };
            _productVariantPackageService.Setup(x => x.GetProductVariantPackages()).Returns(packages);

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Failure);
        }

        [TestMethod]
        public async Task AddToCart_NewPackageToNewCart_ReturnsStatus()
        {
            // Arrange

            var request = new AddToCartRequestViewModel()
            {
                UserId = Guid.NewGuid(),
                PackageId = Guid.NewGuid(),
                Number = 2
            };

            // init data in dbcontext 
            var productVariantId = Guid.NewGuid();
            var productVariantId2 = Guid.NewGuid();
            var newProductVariants = new List<ProductVariant>(){
            new ProductVariant
            {
                Id = productVariantId,
                Number = 4,
                Price = 13000000,
            },
            new ProductVariant
            {
                Id = productVariantId2,
                Number = 10,
                Price = 15000000,
            }
            };

            var dbContext = await GetDbContext(newProductVariants);

            // use new dbContext
            // product variant
            var productVariantRepository = new ProductVariantRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productVariantStockRepository = new ProductVariantStockRepository(dbContext);
            var productVariantStockService = new ProductVariantStockService(productVariantStockRepository);
            IMapper mapper = null;
            var productVariantService = new ProductVariantService(productVariantRepository, productRepository, productVariantStockService, mapper);

            // cart
            var cartRepository = new CartRepository(dbContext);

            // product variant cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(
                cartRepository,
                _productService.Object,
                productVariantService,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            // mock return
            // package items
            var packages = new List<ProductVariantPackage>()
            {
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 2,
                    },
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId2,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 1,
                    }
            };
            _productVariantPackageService.Setup(x => x.GetProductVariantPackages()).Returns(packages);

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Success);

            var packagesResult = dbContext.Set<ProductVariantCart>().Where(x => x.PackageId == request.PackageId);
            packagesResult.Sum(x => x.ItemNumber).Should().Be(6);
            packagesResult.Any(x => x.PackageNumber != 2).Should().BeFalse();
        }

        [TestMethod]
        public async Task AddToCart_ExistPackageCartItemToCart_ReturnsStatus()
        {
            // Arrange
            var request = new AddToCartRequestViewModel()
            {
                UserId = Guid.NewGuid(),
                PackageId = Guid.NewGuid(),
                CartId = Guid.NewGuid(),
                Number = 2
            };

            // init data in dbcontext
            var productVariantId = Guid.NewGuid();
            var productVariantId2 = Guid.NewGuid();

            var newProductVariants = new List<ProductVariant> { new ProductVariant
            {
                Id = productVariantId,
                Number = 15,
                Price = 13000000,
            },
            new ProductVariant
            {
                Id = productVariantId2,
                Number = 10,
                Price = 15000000,
            }};

            var newProductVariantCarts = new List<ProductVariantCart> { new ProductVariantCart
                {
                    Id = Guid.NewGuid(),
                    ProductVariantId = productVariantId,
                    CartId = (Guid)request.CartId,
                    ItemNumber = 4,
                    PackageId = (Guid)request.PackageId,
                    ItemType = CartItemType.PackageItem,
                    PackageNumber = 2,
                },
                new ProductVariantCart
                {
                    Id = Guid.NewGuid(),
                    ProductVariantId = productVariantId2,
                    CartId = (Guid)request.CartId,
                    ItemNumber = 2,
                    PackageId = (Guid)request.PackageId,
                    ItemType = CartItemType.PackageItem,
                    PackageNumber = 2,
                }};

            var dbContext = await GetDbContext(newProductVariants, null, newProductVariantCarts);

            // use new dbContext
            // product variant
            var productVariantRepository = new ProductVariantRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productVariantStockRepository = new ProductVariantStockRepository(dbContext);
            var productVariantStockService = new ProductVariantStockService(productVariantStockRepository);
            IMapper mapper = null;
            var productVariantService = new ProductVariantService(productVariantRepository, productRepository, productVariantStockService, mapper);

            // cart
            var cartRepository = new CartRepository(dbContext);

            // product variant cart
            var productVariantCartRepository = new ProductVariantCartRepository(dbContext);
            IProductVariantCartService productVariantCartService = new ProductVariantCartService(productVariantCartRepository);

            _cartService = new CartService(
                cartRepository,
                _productService.Object,
                productVariantService,
                productVariantCartService,
                _addressService.Object,
                _cartDiscountService.Object,
                _shopUserService.Object,
                _stockService.Object,
                _productVariantPackageService.Object,
                _packageService.Object);

            dbContext.ChangeTracker.Clear();

            // mock return
            // product variant package
            var packages = new List<ProductVariantPackage>()
            {
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 2,
                    },
                new ProductVariantPackage()
                    {
                        ProductVariantId = productVariantId2,
                        PackageId = (Guid)request.PackageId,
                        ItemNumber = 1,
                    }
            };
            _productVariantPackageService.Setup(x => x.GetProductVariantPackages()).Returns(packages);

            // Act
            var result = await _cartService.AddToCart(request);

            // Assert
            result.Should().BeOfType<AddToCartResponsetViewModel>();
            result.Status.Should().BeSameAs(CommonStatus.Success);

            var packagesResult = dbContext.Set<ProductVariantCart>().Where(x => x.PackageId == request.PackageId);
            packagesResult.Sum(x => x.ItemNumber).Should().Be(12);
            packagesResult.Any(x => x.PackageNumber != 4).Should().BeFalse();
        }
    }
}