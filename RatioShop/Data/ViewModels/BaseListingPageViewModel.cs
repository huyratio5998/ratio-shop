using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;

namespace RatioShop.Data.ViewModels
{
    public class BaseListingPageViewModel : IBasePaging
    {
        //sorting
        public SortingEnum SortBy { get; set; }
        //paging
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public IEnumerable<FacetFilterItem>? FilterItems { get; set; }
        public bool IsSelectPreviousItems {get;set;}
    }
}
