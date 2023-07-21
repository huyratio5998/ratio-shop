namespace RatioShop.Data.ViewModels
{
    public interface IBasePaging
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        int TotalCount { get; set; }
        int TotalPage { get; set; }
        bool IsSelectPreviousItems { get; set; }
    }
}
