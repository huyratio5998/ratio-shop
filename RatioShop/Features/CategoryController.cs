using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;

namespace RatioShop.Features
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;

        public CategoryController(ICategoryService categoryService, ICatalogService catalogService)
        {
            _categoryService = categoryService;
            _catalogService = catalogService;
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            var categories = _categoryService.GetCategories().ToList();
            foreach (var item in categories)
            {
                item.Catalog = _catalogService.GetCatalog(item.CatalogId);
            }
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            var category = _categoryService.GetCategory(id);
            category.Catalog = _catalogService.GetCatalog(category.CatalogId);

            return View(category);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            var model = new CategoryViewModel();
            model.AvailableCatalogs = _catalogService.GetCatalogs().ToDictionary(x => x.Id.ToString(), y => y.DisplayName);
            return View(model);
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            try
            {
                if (category == null) return View();

                _categoryService.CreateCategory(category);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null) return View();

            var model = new CategoryViewModel();
            model.Category = category;
            model.AvailableCatalogs = _catalogService.GetCatalogs().ToDictionary(x => x.Id.ToString(), y => y.DisplayName);

            return View(model);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            try
            {
                if (category == null) return View();

                var result = _categoryService.UpdateCategory(category);

                if (!result) return View();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetCategory(id);
            category.Catalog = _catalogService.GetCatalog(category.CatalogId);

            return category == null ? View() : View(category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Category category)
        {
            try
            {
                if (category == null) return View();

                var result = _categoryService.DeleteCategory(id);

                return result ? RedirectToAction(nameof(Index)) : View();
            }
            catch
            {
                return View();
            }
        }
    }
}
