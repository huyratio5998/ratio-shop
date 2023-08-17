using RatioShop.Data.ViewModels;

namespace RatioShop.Areas.Admin.Models.User
{
    public class ListUsersViewModel<T> : BaseListingPageViewModel where T : BaseUserViewModel
    {
        public ListUsersViewModel()
        {
            if (Users == null) Users = new List<T>();
        }
        public List<T> Users { get; set; }
    }
}
