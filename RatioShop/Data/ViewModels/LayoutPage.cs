namespace RatioShop.Data.ViewModels
{
    public class LayoutPage<T> : LayoutViewModel
    {
        public LayoutPage(T pageModel, string title) : base(title)
        {
            PageModel = pageModel;
        }
        public T PageModel { get; set; }       
    }
}
