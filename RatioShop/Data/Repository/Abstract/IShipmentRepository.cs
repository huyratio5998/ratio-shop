using RatioShop.Data.Models;

namespace RatioShop.Data.Repository.Abstract
{
    public interface IShipmentRepository
    {
        IQueryable<Shipment> GetShipments();
        Shipment? GetShipment(string id);
        Task<Shipment> CreateShipment(Shipment Shipment);
        bool UpdateShipment(Shipment Shipment);        
        bool DeleteShipment(string id);
    }
}
