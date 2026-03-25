using eShop.Domain.Entities.ProductData;
using eShop.Infrastructure.Interfaces;
using eShopMVC.App.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eShopMVC.App.Controllers;

public class ProductController : BaseController
{
    public ProductController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    // ── Public catalog ────────────────────────────────────────────────────────

    public IActionResult Index(int? categoryId, string? search)
    {
        IEnumerable<Product> products = _unitOfWork.Product
            .GetAll(includeProperties: "Category,BaseUnit");

        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        var cards = products.Select(p => new ProductCardViewModel
        {
            Id           = p.Id,
            Name         = p.Name,
            Description  = p.Description,
            Price        = p.Price,
            CategoryName = p.Category?.Name,
            TypeDisplay  = p.Type.ToDisplayName()
        }).ToList();

        var model = new ProductCatalogViewModel
        {
            Products           = cards,
            Categories         = _unitOfWork.Category.GetAll().OrderBy(c => c.DisplayOrder),
            SelectedCategoryId = categoryId,
            Search             = search
        };

        return View(model);
    }

    public IActionResult Details(int? id)
    {
        if (id is null || id == 0) return NotFound();

        var product = _unitOfWork.Product.Get(
            p => p.Id == id.Value,
            includeProperties: "Category,BaseUnit");

        if (product is null) return NotFound();

        var model = new ProductViewModel
        {
            Id             = product.Id,
            Name           = product.Name,
            Description    = product.Description,
            Price          = product.Price,
            Type           = product.Type,
            TypeDisplay    = product.Type.ToDisplayName(),
            CategoryId     = product.CategoryId,
            CategoryName   = product.Category?.Name,
            BaseUnitId     = product.BaseUnitId,
            BaseUnitSymbol = product.BaseUnit?.Symbol
        };

        return View(model);
    }

    // ── Admin management ──────────────────────────────────────────────────────

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Manage()
    {
        var products = _unitOfWork.Product
            .GetAll(includeProperties: "Category")
            .Select(p => new ProductViewModel
            {
                Id           = p.Id,
                Name         = p.Name,
                Price        = p.Price,
                TypeDisplay  = p.Type.ToDisplayName(),
                CategoryName = p.Category?.Name
            })
            .OrderBy(p => p.Name)
            .ToList();

        return View(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View(BuildCreateModel(new CreateProductViewModel()));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(BuildCreateModel(model));

        var product = new Product
        {
            Name        = model.Name.Trim(),
            Description = model.Description?.Trim(),
            Price       = model.Price,
            Type        = model.Type,
            CategoryId  = model.CategoryId,
            BaseUnitId  = model.BaseUnitId
        };

        _unitOfWork.Product.Add(product);
        await _unitOfWork.SaveAsync();

        TempData["Success"] = "Товар додано успішно.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Edit(int? id)
    {
        if (id is null || id == 0) return NotFound();

        var product = _unitOfWork.Product.Get(p => p.Id == id.Value);
        if (product is null) return NotFound();

        var model = new EditProductViewModel
        {
            Id          = product.Id,
            Name        = product.Name,
            Description = product.Description,
            Price       = product.Price,
            Type        = product.Type,
            CategoryId  = product.CategoryId,
            BaseUnitId  = product.BaseUnitId
        };

        return View(BuildEditModel(model));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(BuildEditModel(model));

        var product = _unitOfWork.Product.Get(p => p.Id == model.Id);
        if (product is null) return NotFound();

        product.Name        = model.Name.Trim();
        product.Description = model.Description?.Trim();
        product.Price       = model.Price;
        product.Type        = model.Type;
        product.CategoryId  = model.CategoryId;
        product.BaseUnitId  = model.BaseUnitId;

        _unitOfWork.Product.Update(product);
        await _unitOfWork.SaveAsync();

        TempData["Success"] = "Товар оновлено успішно.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int? id)
    {
        if (id is null || id == 0) return NotFound();

        var product = _unitOfWork.Product.Get(
            p => p.Id == id.Value,
            includeProperties: "Category");

        if (product is null) return NotFound();

        var model = new ProductViewModel
        {
            Id           = product.Id,
            Name         = product.Name,
            Description  = product.Description,
            Price        = product.Price,
            TypeDisplay  = product.Type.ToDisplayName(),
            CategoryName = product.Category?.Name
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        if (id is null || id == 0) return NotFound();

        var product = _unitOfWork.Product.Get(p => p.Id == id.Value);
        if (product is null) return NotFound();

        _unitOfWork.Product.Remove(product);
        await _unitOfWork.SaveAsync();

        TempData["Success"] = "Товар видалено.";
        return RedirectToAction(nameof(Manage));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private CreateProductViewModel BuildCreateModel(CreateProductViewModel model)
    {
        model.CategoryList = _unitOfWork.Category
            .GetAll()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

        model.UnitList = GetUnitSelectList();
        return model;
    }

    private EditProductViewModel BuildEditModel(EditProductViewModel model)
    {
        model.CategoryList = _unitOfWork.Category
            .GetAll()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

        model.UnitList = GetUnitSelectList();
        return model;
    }

    private IEnumerable<SelectListItem> GetUnitSelectList() =>
        _unitOfWork.Unit
            .GetAll()
            .Select(u => new SelectListItem(u.Name, u.Id.ToString()))
            .ToList();
}
