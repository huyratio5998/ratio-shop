namespace RatioShop.Data.ViewModels.SearchViewModel
{
    public interface IBasePagingRequest
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }        
        bool IsSelectPreviousItems { get; set; }
    }
}
