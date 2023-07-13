using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IProductVariantService _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICatalogService _catalogService;
        private readonly IProductVariantStockService _productVariantStockService;
        private readonly IStockService _stockService;

        public ProductService(IProductRepository productRepository, IProductVariantService productVariantService, ICategoryService categoryService, IProductCategoryService productCategoryService, ICatalogService catalogService, IProductVariantStockService productVariantStockService, IStockService stockService)
        {
            this.productRepository = productRepository;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _catalogService = catalogService;
            _productVariantStockService = productVariantStockService;
            _stockService = stockService;
        }

        public async Task<bool> AddProduct(Product product)
        {
            product.CreatedDate = DateTime.UtcNow;
            product.ModifiedDate = DateTime.UtcNow;

            return await productRepository.AddProduct(product);
        }        

        public bool DeleteProduct(Guid productId)
        {
            return productRepository.DeleteProduct(productId);
        }

        public IEnumerable<ProductViewModel> GetAllProductsByPageNumber(string sortBy, int pageNumber, int pageSize)
        {
            if(pageNumber <= 0) pageNumber = 1;
            IEnumerable<ProductViewModel> products = productRepository.GetAllProductsByPageNumber(sortBy, pageNumber, pageSize); ;            
            return products;
        }        

        public ProductViewModel GetProduct(Guid productId)
        {
            var product = productRepository.GetProduct(productId);            
            if (product == null || product.Product == null)
            {
                var productVariant = _productVariantService.GetProductVariant(productId.ToString());
                if (productVariant != null)
                {                    
                    productId = productVariant.ProductId;
                    product = productRepository.GetProduct(productId);
                    product.SelectedVariant = productVariant;
                }
            }            
            
            var productVariants = _productVariantService.GetProductVariantsByProductId(productId).ToList();
            //product variants stock
            if(productVariants != null && productVariants.Any())
            {
                foreach (var item in productVariants)
                {
                    var productVariantStocks = _productVariantStockService.GetProductVariantStocksByVariantId(item.Id).ToList();                    
                    item.ProductVariantStocks = productVariantStocks;
                }
            }
            product.Product.Variants = productVariants;
            if(product.SelectedVariant == null) product.SelectedVariant = product.Product.Variants?.FirstOrDefault();

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
            return productRepository.GetProducts();
        }

        public IEnumerable<ProductViewModel> GetProducts(int pageNumber, int pageSize)
        {
            return productRepository.GetProducts(pageNumber,pageSize);            
        }

        public IEnumerable<ProductViewModel> GetProducts(string sortBy, int pageNumber, int pageSize)
        {
            IEnumerable<ProductViewModel> products = null;
            if (string.IsNullOrEmpty(sortBy)) products = GetProducts(pageNumber, pageSize);
            products = productRepository.GetProducts(sortBy, pageNumber, pageSize);
            //

            //
            return products;
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable()
        {
            return productRepository.GetProducts().Where(p=> !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(int pageNumber, int pageSize)
        {
            return productRepository.GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public IEnumerable<ProductViewModel> GetProductsAvailable(string sortBy, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(sortBy)) return GetProducts(pageNumber, pageSize).Where(p => !p.Product.IsDelete);

            return productRepository.GetProducts(sortBy, pageNumber, pageSize).Where(p => !p.Product.IsDelete);
        }

        public List<ProductViewModel> GetProductsRelatedInformation(List<ProductViewModel> products)
        {
            if(products == null || !products.Any()) return products;
            
            foreach (var product in products)
            {
                GetProductRelatedInformation(product);
            }
            return products;
        }

        public bool UpdateProduct(Product product)
        {
            product.ModifiedDate = DateTime.UtcNow;

            return productRepository.UpdateProduct(product);
        }

        public ListProductViewModel GetListProducts(string searchText, string orderBy, int pageNumber, int pageSize)
        {
            var products = productRepository.GetListProducts(searchText, orderBy, pageNumber, pageSize);
            products.Products = GetProductsRelatedInformation(products.Products.ToList());

            return products;
        }

        public ListProductViewModel GetAllListProducts(string searchText, string orderBy, int pageNumber, int pageSize)
        {
            var products = productRepository.GetAllListProducts(searchText, orderBy, pageNumber, pageSize);
            products.Products = GetProductsRelatedInformation(products.Products.ToList());
            products.SearchText = searchText;

            return products;
        }
    }
}
