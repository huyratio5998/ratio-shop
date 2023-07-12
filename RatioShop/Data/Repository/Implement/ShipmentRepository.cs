using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;

namespace RatioShop.Data.Repository.Implement
{
    public class ShipmentRepository : BaseProductRepository<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Shipment> CreateShipment(Shipment Shipment)
        {
            return await Create(Shipment);
        }

        public bool DeleteShipment(string id)
        {
            return Delete(id);
        }

        public IQueryable<Shipment> GetShipments()
        {
            return GetAll();
        }

        public Shipment? GetShipment(string id)
        {
            return GetById(id);
        }

        public bool UpdateShipment(Shipment Shipment)
        {
            return Update(Shipment);
        }
    }
}
