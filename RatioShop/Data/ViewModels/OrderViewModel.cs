using RatioShop.Data.Models;
using System.ComponentModel;

namespace RatioShop.Data.ViewModels
{
    public class OrderViewModel
    {        
        public Order? Order { get; set; }

        public bool AssignedShipper { get; set; }
        public string? ShipperName { get; set; }

        [DisplayName("Total Items")]
        public int TotalItems { get; set; }        


    }
}
