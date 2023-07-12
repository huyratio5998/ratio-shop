using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.User;

namespace RatioShop.Data.ViewModels.MyAccount
{
    public class MyAccountViewModel
    {
        public string SelectedTab { get; set; }
        public UserViewModel? UserData { get; set; }
        public ListOrderViewModel? OrderHistory { get; set; }
        public List<string> ListCities { get; set; }
        public List<string>? ListDistrict { get; set; }
    }
}
