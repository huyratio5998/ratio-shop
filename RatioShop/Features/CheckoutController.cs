using Microsoft.AspNetCore.Mvc;

namespace RatioShop.Features
{
    public class CheckoutController : Controller
    {
        public IActionResult CheckoutSuccess(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;
            return View();
        }
        public IActionResult CheckoutFailure()
        {            
            return View();
        }
    }
}
