namespace RatioShop.Data.ViewModels.SearchViewModel
{
    public interface IFacetFilter
    {
        IEnumerable<FacetFilterItem> FilterItems { get; set; }        
    }
}
