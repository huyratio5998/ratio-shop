using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.User;

namespace RatioShop.Services.Abstract
{
    public interface IShipmentService
    {
        IQueryable<Shipment> GetShipments();
        Shipment? GetShipment(string id);
        Task<Shipment> CreateShipment(Shipment Shipment);
        bool UpdateShipment(Shipment Shipment);
        bool DeleteShipment(string id);

        List<Guid>? GetInprogressOrderIdsByShipperId(string? shipperId);
        List<Guid>? GetFinishedOrderIdsByShipperId(string? shipperId);
        List<Guid>? GetAllUnAssignedOrderIds();
        List<UserResponseViewModel> GetAvailableShippers();
        string? GetShipperNameById(string shipperId);
    }
}
