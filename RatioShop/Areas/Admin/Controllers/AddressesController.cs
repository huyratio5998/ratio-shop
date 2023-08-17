using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Services.Abstract;
using System.Data;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,ContentEditor")]
    public class AddressesController : Controller
    {
        private readonly IAddressService _AddressService;

        public AddressesController(IAddressService AddressService)
        {
            _AddressService = AddressService;
        }

        // GET: AddressController
        public ActionResult Index()
        {
            var categories = _AddressService.GetAddresses();
            return View(categories);
        }

        // GET: AddressController/Details/5
        public ActionResult Details(int id)
        {
            var Address = _AddressService.GetAddress(id);
            return View(Address);
        }

        // GET: AddressController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddressController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Address Address)
        {
            try
            {
                if (Address == null) return View();

                _AddressService.CreateAddress(Address);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AddressController/Edit/5
        public ActionResult Edit(int id)
        {
            var Address = _AddressService.GetAddress(id);
            if (Address == null) return View();

            return View(Address);
        }

        // POST: AddressController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Address Address)
        {
            try
            {
                if (Address == null) return View();

                var result = _AddressService.UpdateAddress(Address);

                if (!result) return View();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AddressController/Delete/5
        public ActionResult Delete(int id)
        {
            var Address = _AddressService.GetAddress(id);
            return Address == null ? View() : View(Address);
        }

        // POST: AddressController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Address Address)
        {
            try
            {
                if (Address == null) return View();

                var result = _AddressService.DeleteAddress(id);

                return result ? RedirectToAction(nameof(Index)) : View();
            }
            catch
            {
                return View();
            }
        }
    }
}
