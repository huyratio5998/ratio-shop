using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels
{
    public class AddressResponseViewModel
    {
        public AddressResponseViewModel()
        {

        }
        public AddressResponseViewModel(Address address)
        {
            Address1 = address.Address1;
            Address2 = address.Address2;
            Address3 = address.Address3;
            Address4 = address.Address4;
            Address5 = address.Address5;
        }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? Address5 { get; set; }
        public string? AddressDetail { get; set; }
    }
}
