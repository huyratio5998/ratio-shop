using RatioShop.Data.Models;

namespace RatioShop.Services.Abstract
{
    public interface IShipmentService
    {
        IQueryable<Shipment> GetShipments();
        Shipment? GetShipment(string id);
        Task<Shipment> CreateShipment(Shipment Shipment);
        bool UpdateShipment(Shipment Shipment);
        bool DeleteShipment(string id);        
    }
}
