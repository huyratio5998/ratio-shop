using RatioShop.Data.ViewModels;

namespace RatioShop.Services.Implement
{
    public interface ICommonService
    {
        IEnumerable<BreadcrumbItemViewModel> GetBreadCrumbsByProductId(Guid productId);
    }
}
