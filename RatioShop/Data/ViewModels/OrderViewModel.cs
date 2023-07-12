using RatioShop.Data.Models;
using System.ComponentModel;

namespace RatioShop.Data.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {

        }
        public OrderViewModel(Order order)
        {
            Order = order;
        }
        public Order? Order { get; set; }
        //
        [DisplayName("Order Number")]
        public string? OrderNumber { get; set; }

        [DisplayName("Order Status")]
        public string? Status { get; set; }

        [DisplayName("Total Price")]
        public decimal? TotalMoney { get; set; }

        [DisplayName("Total Items")]
        public int TotalItems { get; set; }
        public bool IsRefund { get; set; }
        
        [DisplayName("Shipment Status")]
        public string? ShipmentStatus { get; set; }
        public decimal? ShipmentFee { get; set; }

        public Guid CartId { get; set; }
        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public List<Shipment>? Shipments { get; set; }
    }
}
