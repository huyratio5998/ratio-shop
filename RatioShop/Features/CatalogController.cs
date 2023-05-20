using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Services.Abstract;

namespace RatioShop.Features
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService CatalogService)
        {
            _catalogService = CatalogService;
        }

        // GET: CatalogController
        public ActionResult Index()
        {
            var categories = _catalogService.GetCatalogs();
            return View(categories);
        }

        // GET: CatalogController/Details/5
        public ActionResult Details(int id)
        {
            var Catalog = _catalogService.GetCatalog(id);
            return View(Catalog);
        }

        // GET: CatalogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatalogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Catalog Catalog)
        {
            try
            {
                if (Catalog == null) return View();

                _catalogService.CreateCatalog(Catalog);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatalogController/Edit/5
        public ActionResult Edit(int id)
        {
            var Catalog = _catalogService.GetCatalog(id);
            if (Catalog == null) return View();

            return View(Catalog);
        }

        // POST: CatalogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Catalog Catalog)
        {
            try
            {
                if (Catalog == null) return View();

                var result = _catalogService.UpdateCatalog(Catalog);

                if (!result) return View();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatalogController/Delete/5
        public ActionResult Delete(int id)
        {
            var Catalog = _catalogService.GetCatalog(id);
            return Catalog == null ? View() : View(Catalog);
        }

        // POST: CatalogController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Catalog Catalog)
        {
            try
            {
                if (Catalog == null) return View();

                var result = _catalogService.DeleteCatalog(id);

                return result ? RedirectToAction(nameof(Index)) : View();
            }
            catch
            {
                return View();
            }
        }
    }
}
