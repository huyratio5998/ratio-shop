using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _ShipmentRepository;

        public ShipmentService(IShipmentRepository ShipmentRepository)
        {
            _ShipmentRepository = ShipmentRepository;
        }

        public Task<Shipment> CreateShipment(Shipment Shipment)
        {
            Shipment.CreatedDate = DateTime.UtcNow;
            Shipment.ModifiedDate = DateTime.UtcNow;
            return _ShipmentRepository.CreateShipment(Shipment);
        }

        public bool DeleteShipment(string id)
        {
            return _ShipmentRepository.DeleteShipment(id);
        }

        public IQueryable<Shipment> GetShipments()
        {
            return _ShipmentRepository.GetShipments();
        }

        public Shipment? GetShipment(string id)
        {
            return _ShipmentRepository.GetShipment(id);
        }

        public bool UpdateShipment(Shipment Shipment)
        {
            Shipment.ModifiedDate = DateTime.UtcNow;
            return _ShipmentRepository.UpdateShipment(Shipment);
        }
    }
}
