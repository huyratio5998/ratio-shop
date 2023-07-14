using AutoMapper;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Helpers;
using RatioShop.Helpers.QueryableHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantService _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICatalogService _catalogService;
        private readonly IProductVariantStockService _productVariantStockService;
        private readonly IStockService _stockService;

        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IProductVariantService productVariantService, ICategoryService categoryService, IProductCategoryService productCategoryService, ICatalogService catalogService, IProductVariantStockService productVariantStockService, IStockService stockService, IMapper mapper)
        {
            _productRepository = productRepository;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _catalogService = catalogService;
            _productVariantStockService = productVariantStockService;
            _stockService = stockService;
            _mapper = mapper;
        }

        public async Task<bool> AddProduct(Product product)
        {
            product.CreatedDate = DateTime.UtcNow;
            product.ModifiedDate = DateTime.UtcNow;

            return await _productRepository.AddProduct(product);
        }

        public bool DeleteProduct(Guid productId)
        {
            return _productRepository.DeleteProduct(productId);
        }

        public IEnumerable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            IEnumerable<ProductViewModel> products = _productRepository.GetAllProductsByPageNumber(sortBy, pageNumber, pageSize); ;
            return products;
        }

        public ProductViewModel GetProduct(Guid productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null || product.Product == null)
            {
                var productVariant = _productVariantService.GetProductVariant(productId.ToString());
                if (productVariant != null)
                {
                    productId = productVariant.ProductId;
                    product = _productRepository.GetProduct(productId);
                    product.SelectedVariant = productVariant;
                }
            }

            var productVariants = _productVariantService.GetProductVariantsByProductId(productId).ToList();
            //product variants stock
            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    var productVariantStocks = _productVariantStockService.GetProductVariantStocksByVariantId(item.Id).ToList();
                    item.ProductVariantStocks = productVariantStocks;
                }
            }
            product.Product.Variants = productVariants;
            if (product.SelectedVariant == null) product.SelectedVariant = product.Product.Variants?.FirstOrDefault();

            product.AvailableStocks = _stockService.GetStocks().Where(x => x.IsActive).ToDictionary(x => x.Id, y => y.Name);
            //
            var availableCategories = _categoryService.GetCategories().ToList();
            foreach (var item in availableCategories)
            {
                item.Catalog = _catalogService.GetCatalog(item.CatalogId);
            }
            product.AvailableCategories = availableCategories;
            //
            var categories = _productCategoryService.GetCategorysByProductId(productId).ToList();
            foreach (var item in categories)
            {
                item.Catalog = _catalogService.GetCatalog(item.CatalogId);
            }
            product.ProductCategories = categories;

            return product;
        }

        public void GetProductRelatedInformation(ProductViewModel product)
        {
            product.Product.CreatedDate = product.Product.CreatedDate.GetCorrectUTC();
            product.Product.ModifiedDate = product.Product.ModifiedDate.GetCorrectUTC();

            var productId = product.Product.Id;
            var productVariants = _productVariantService.GetProductVariantsByProductId(productId).ToList();
            //product variants stock
            if (productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    var productVariantStocks = _productVariantStockService.GetProductVariantStocksByVariantId(item.Id).ToList();
                    item.ProductVariantStocks = productVariantStocks;
                }
            }
            product.Product.Variants = productVariants;

            product.AvailableStocks = _stockService.GetStocks().Where(x => x.IsActive).ToDictionary(x => x.Id, y => y.Name);
            //
            var availableCategories = _categoryService.GetCategories().ToList();
            foreach (var item in availableCategories)
            {
                item.Catalog = _catalogService.GetCatalog(item.CatalogId);
            }
            product.AvailableCategories = availableCategories;
            //
            var categories = _productCategoryService.GetCategorysByProductId(productId).ToList();
            foreach (var item in categories)
            {
                item.Catalog = _catalogService.GetCatalog(item.CatalogId);
            }
            product.ProductCategories = categories;
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize)
        {
            return _productRepository.GetProducts(pageNumber, pageSize);
        }

        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
        {
            IEnumerable<ProductViewModel> products = null;
            if (string.IsNullOrEmpty(sortBy)) products = GetProducts(pageNumber, pageSize);
            products = _productRepository.GetProducts(sortBy, pageNumber, pageSize);
            //

            //
            return products;
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable()
        {
            return _productRepository.GetProducts().Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(int pageNumber, int pageSize)
        {
            return _productRepository.GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(string sortBy, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(sortBy)) return GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);

            return _productRepository.GetProducts(sortBy, pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public List<ProductViewModel> GetProductsRelatedInformation(List<ProductViewModel> products)
        {
            if (products == null || !products.Any()) return products;

            foreach (var product in products)
            {
                GetProductRelatedInformation(product);
            }
            return products;
        }

        public bool UpdateProduct(Product product)
        {
            product.ModifiedDate = DateTime.UtcNow;

            return _productRepository.UpdateProduct(product);
        }

        public ListProductViewModel GetListProducts(ProductSearchRequest request)
        {
            if (request == null) return new ListProductViewModel();

            var searchRequest = _mapper.Map<BaseSearchRequest>(request);

            var searchProducts = _productRepository.GetAll().Where(x => !x.IsDelete);
            searchProducts = searchProducts.BuildProductFilter(searchRequest);
            searchProducts = searchProducts.SortedProductsGeneric(request.SortType);
            //
            //var variantsOrder = searchProducts
            //    .Join(
            //    _productVariantService.GetProductVariants()
            //    .GroupBy(x=>x.ProductId)
            //    .Select(group =>
            //            new {
            //                ProductId = group.Key,
            //                Variants = group.OrderBy(x => x.Price)
            //            })
            //      .OrderBy(group => group.Variants.First()),                  
            //      x=>x.Id,
            //      y=>y.ProductId,
            //      (x,y) => new {Products = x,Variants = y.Variants})
            //      .sort
            //    ;                                
            //            

            var totalMatch = searchProducts.Count();
            searchProducts = searchProducts.PagingProductsGeneric<Product>(searchRequest);

            var result = new ListProductViewModel
            {
                PageIndex = searchRequest.PageIndex,
                PageSize = searchRequest.PageSize,
                TotalCount = totalMatch,
                TotalPage = totalMatch == 0 ? 1 : (int)Math.Ceiling((double)totalMatch / searchRequest.PageSize),
                Products = GetProductsRelatedInformation(searchProducts.Select(x => new ProductViewModel() { Product = x, ProductImageName = x.ProductImage }).ToList()),
                FilterItems = searchRequest.FilterItems,
            };

            return result;
        }


    }
}
