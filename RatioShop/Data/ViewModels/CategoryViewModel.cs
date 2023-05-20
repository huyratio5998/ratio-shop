using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {

        }
        public CategoryViewModel(Category category)
        {
            Category = category;
        }
        public Category Category { get; set; }
        public Dictionary<string,string?> AvailableCatalogs { get; set; }
        public string Catalog { get; set; }        
    }
}
