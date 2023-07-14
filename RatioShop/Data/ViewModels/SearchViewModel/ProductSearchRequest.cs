using Microsoft.AspNetCore.Mvc;
using RatioShop.Enums;

namespace RatioShop.Data.ViewModels.SearchViewModel
{
    [BindProperties]
    public class ProductSearchRequest : IBaseSearchArgs
    {
        [BindProperty(Name = "filterItems")]        
        public string? FilterItems { get; set; }

        [BindProperty(Name = "sortSelectedValue")]
        public string? SortSelectedValue { get; set; }

        [BindProperty(Name = "sortType")]
        public SortingEnum SortType { get; set; }

        [BindProperty(Name = "isSelectPreviousItems")]
        public bool IsSelectPreviousItems { get; set; }

        [BindProperty(Name ="page")]
        public int PageIndex { get; set; }

        [BindProperty(Name = "pageSize")]
        public int PageSize { get; set; }
    }
}
