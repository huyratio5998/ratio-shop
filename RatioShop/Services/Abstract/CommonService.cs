using Newtonsoft.Json;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Services.Implement;

namespace RatioShop.Services.Abstract
{
    public class CommonService : ICommonService
    {
        private readonly IProductService _productService;

        public CommonService(IProductService productService)
        {
            _productService = productService;
        }

        public IEnumerable<BreadcrumbItemViewModel> GetBreadCrumbsByProductId(Guid productId)
        {
            var result = new List<BreadcrumbItemViewModel>();
            if (productId == Guid.Empty) return result;

            var product = _productService.GetProduct(productId);
            var firstCategory = product.ProductCategories?.FirstOrDefault();

            if (firstCategory == null) return result;

            var home = new BreadcrumbItemViewModel
            {
                DisplayName = "Home",
                Url = "/",
            };

            var categoryQuery = JsonConvert.SerializeObject(
                new List<FacetFilterItem>() { 
                    new FacetFilterItem { 
                        FieldName = FieldNameFilter.Category.ToString(), 
                        Type = FilterType.Text.ToString(), 
                        Value = firstCategory?.Id.ToString() 
                    } 
                });
            var category = new BreadcrumbItemViewModel
            {
                DisplayName = firstCategory?.DisplayName,
                Url = $"/products?filterItems={categoryQuery}",

            };

            string productDisplayName = string.IsNullOrEmpty(product.Product?.ProductFriendlyName) ? product.Product?.Name : product.Product?.ProductFriendlyName;
            var currentProduct = new BreadcrumbItemViewModel
            {
                DisplayName = productDisplayName,
                IsActive = true,
            };
            result.Add(home);
            result.Add(category);
            result.Add(currentProduct);

            return result;
        }

    }
}
