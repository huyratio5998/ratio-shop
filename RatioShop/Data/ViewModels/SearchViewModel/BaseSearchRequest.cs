using RatioShop.Enums;

namespace RatioShop.Data.ViewModels.SearchViewModel
{
    public class BaseSearchRequest : IFacetFilter, IBasePagingRequest
    {
        public IEnumerable<FacetFilterItem> FilterItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }        
        public bool IsSelectPreviousItems { get; set; }
        public SortingEnum SortType { get; set; }
    }
}
