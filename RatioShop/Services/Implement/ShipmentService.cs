using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.ViewModels.User;
using RatioShop.Enums;
using RatioShop.Services.Abstract;

namespace RatioShop.Services.Implement
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _ShipmentRepository;
        private readonly IShopUserService _shopUserService;

        public ShipmentService(IShipmentRepository shipmentRepository, IShopUserService shopUserService)
        {
            _ShipmentRepository = shipmentRepository;
            _shopUserService = shopUserService;
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

        public List<Guid>? GetInprogressOrderIdsByShipperId(string? shipperId)
        {
            if (string.IsNullOrEmpty(shipperId)) return null;

            var shipments = GetShipments()
                .Where(x => x.ShipperId == shipperId
                && x.UpdateStatus == true)
                .GroupBy(x => x.OrderId)
                .Select(x => x.OrderByDescending(c => c.CreatedDate).FirstOrDefault()).ToList();

            if (shipments == null || !shipments.Any()) return null;

            List<Shipment> validShipments = new List<Shipment>();

            // validate if that shipment being assign to another shipper
            foreach (var item in shipments)
            {
                if (item == null) continue;
                var isAssignToAnotherShipper = GetShipments()
                    .Where(x => x.UpdateStatus == true && x.OrderId == item.OrderId)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefault()?.ShipperId != item.ShipperId;

                if (!isAssignToAnotherShipper) validShipments.Add(item);
            }

            var orderIds = validShipments.Where(x =>
                x.ShipmentStatus == CommonStatus.ShipmentStatus.Pending
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Delivering
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Failure
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Canceled
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Returning)
                .Select(x => x.OrderId)
                .ToList();

            return orderIds;
        }

        public List<Guid>? GetFinishedOrderIdsByShipperId(string? shipperId)
        {
            if (string.IsNullOrEmpty(shipperId)) return null;

            var shipments = GetShipments()
                .Where(x => x.ShipperId == shipperId
                && x.UpdateStatus == true)
                .GroupBy(x => x.OrderId)
                .Select(x => x.OrderByDescending(c => c.CreatedDate).FirstOrDefault()).ToList();

            if (shipments == null || !shipments.Any()) return null;

            var orderIds = shipments.Where(x => x != null
            && (x.ShipmentStatus == CommonStatus.ShipmentStatus.Closed
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Expired
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Delivered
                || x.ShipmentStatus == CommonStatus.ShipmentStatus.Returned
                ))
                .Select(x => x.OrderId)
                .ToList();

            return orderIds;
        }

        public List<Guid>? GetAllUnAssignedOrderIds()
        {
            var shipments = GetShipments()
                .Where(x => x.UpdateStatus == true)
                .GroupBy(x => x.OrderId)
                .Select(x => x.OrderByDescending(c => c.CreatedDate).FirstOrDefault()).ToList();

            if (shipments == null || !shipments.Any()) return null;

            var orderIds = shipments.Where(x => x != null && string.IsNullOrEmpty(x.ShipperId) && x.ShipmentStatus == CommonStatus.ShipmentStatus.Pending)
                .Select(x => x.OrderId)
                .ToList();

            return orderIds;
        }

        public async Task<List<UserResponseViewModel>> GetAvailableShippers()
        {
            // should get by role later            
            var users = _shopUserService.GetShopUsers().ToList();
            var shippers = new List<UserResponseViewModel>();

            if (users == null || !users.Any()) return shippers;

            foreach (var user in users)
            {
                var roles = await _shopUserService.GetUserRoles(user);
                if (roles != null && roles.Contains(UserRole.Shipper.ToString()))
                    shippers.Add(
                        new UserResponseViewModel
                        {
                            FullName = string.IsNullOrEmpty(user.EmployeeName) ? user.FullName : user.EmployeeName,
                            PhoneNumber = user.PhoneNumber,
                            ShipperId = Guid.Parse(user.Id)
                        }
                );
            }

            if (!shippers.Any()) return shippers;

            foreach (var user in shippers)
            {
                var totalShipmentInprogress = GetInprogressOrderIdsByShipperId(user.ShipperId.ToString())?.Count();
                user.TotalAssignedOrders = totalShipmentInprogress ?? 0;
            }

            return shippers.OrderBy(x => x.TotalAssignedOrders).ToList();
        }

        public string? GetShipperNameById(string shipperId)
        {
            if (string.IsNullOrEmpty(shipperId)) return null;

            return _shopUserService.GetShopUser(shipperId)?.EmployeeName ?? _shopUserService.GetShopUser(shipperId)?.FullName;
        }
    }
}
