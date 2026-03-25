using eShop.Domain.Entities;
using eShop.Infrastructure.Interfaces;
using eShopMVC.App.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShopMVC.App.Controllers;

public class CategoryController : BaseController
{
    public CategoryController(IUnitOfWork unitOfWork): base(unitOfWork) { }

    public IActionResult Index()
    {

        var categories = _unitOfWork.Category.GetAll().Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            DisplayOrder = c.DisplayOrder
        }).OrderBy(x => x.DisplayOrder).ToList();

        return View(categories);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View(new CreateCategoryViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (model.Name == model.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Назва не може бути такою ж як порядок виведення категорії.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name.Trim(),
            DisplayOrder = model.DisplayOrder
        };

        _unitOfWork.Category.Add(category);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Edit(int? id)
    {
        if (id is null || id == 0)
        {
            return NotFound();
        }

        var category = _unitOfWork.Category.Get(x => x.Id == id.Value);

        if (category is null)
        {
            return NotFound();
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            DisplayOrder = category.DisplayOrder
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var categoryFromDb = _unitOfWork.Category.Get(x => x.Id == model.Id);

        if (categoryFromDb is null)
        {
            return NotFound();
        }

        categoryFromDb.Name = model.Name.Trim();
        categoryFromDb.DisplayOrder = model.DisplayOrder;
        _unitOfWork.Category.Update(categoryFromDb);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int? id)
    {
        if (id is null || id == 0)
        {
            return NotFound();
        }

        var category = _unitOfWork.Category.Get(x => x.Id == id.Value);

        if (category is null)
        {
            return NotFound();
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            DisplayOrder = category.DisplayOrder
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        if (id is null || id == 0)
        {
            return NotFound();
        }

        var category = _unitOfWork.Category.Get(x => x.Id == id.Value);

        if (category is null)
        {
            return NotFound();
        }

        _unitOfWork.Category.Remove(category);
        await _unitOfWork.SaveAsync();

        return RedirectToAction(nameof(Index));
    }
}