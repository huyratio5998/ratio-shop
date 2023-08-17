using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;
using System.Data;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Manager,Admin,ContentEditor")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            var categories = _categoryService.GetCategoriesWithParentData().OrderBy(x => x.ParentId).ToList();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            var category = _categoryService.GetCategory(id);

            return View(category);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            var model = new CategoryViewModel();
            model.AvailableCategory = _categoryService.GetCategories().ToDictionary(x => x.Id.ToString(), x => x.DisplayName);
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
            model.AvailableCategory = _categoryService.GetCategories().Where(x => x.Id != id).OrderBy(x => x.ParentId).ToDictionary(x => x.Id.ToString(), x => x.DisplayName);

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
