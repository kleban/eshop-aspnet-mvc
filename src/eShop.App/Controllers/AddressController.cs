using eShop.Domain.Entities.UserData;
using eShop.Infrastructure.Interfaces;
using eShopMVC.App.ViewModels.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eShopMVC.App.Controllers
{
    [Authorize]
    public class AddressController : BaseController
    {
        public AddressController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User is not authenticated.");

        private bool BelongsToCurrentUser(Address address) =>
            address.UserId == CurrentUserId;

        // ─── Index ────────────────────────────────────────────────────────────────

        public IActionResult Index()
        {
            var addresses = _unitOfWork.Address
                .GetAll(a => a.UserId == CurrentUserId)
                .Select(a => new AddressViewModel
                {
                    Id          = a.Id,
                    FirstName   = a.FirstName,
                    LastName    = a.LastName,
                    PhoneNumber = a.PhoneNumber,
                    AddressLine = a.AddressLine,
                    City        = a.City,
                    Region      = a.Region,
                    PostalCode  = a.PostalCode,
                    Country     = a.Country,
                    IsDefault   = a.IsDefault
                })
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.City)
                .ToList();

            return View(addresses);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateAddressViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.IsDefault)
                _unitOfWork.Address.UnsetDefault(CurrentUserId);

            var address = new Address
            {
                UserId      = CurrentUserId,
                FirstName   = model.FirstName.Trim(),
                LastName    = model.LastName.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                AddressLine = model.AddressLine.Trim(),
                City        = model.City.Trim(),
                Region      = model.Region.Trim(),
                PostalCode  = model.PostalCode.Trim(),
                Country     = model.Country.Trim(),
                IsDefault   = model.IsDefault
            };

            _unitOfWork.Address.Add(address);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Адресу додано.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null or 0)
                return NotFound();

            var address = _unitOfWork.Address.Get(a => a.Id == id.Value);

            if (address is null || !BelongsToCurrentUser(address))
                return NotFound();

            var model = new AddressViewModel
            {
                Id          = address.Id,
                FirstName   = address.FirstName,
                LastName    = address.LastName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,
                City        = address.City,
                Region      = address.Region,
                PostalCode  = address.PostalCode,
                Country     = address.Country,
                IsDefault   = address.IsDefault
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddressViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var address = _unitOfWork.Address.Get(a => a.Id == model.Id, tracked: true);

            if (address is null || !BelongsToCurrentUser(address))
                return NotFound();

            if (model.IsDefault && !address.IsDefault)
                _unitOfWork.Address.UnsetDefault(CurrentUserId);

            address.FirstName   = model.FirstName.Trim();
            address.LastName    = model.LastName.Trim();
            address.PhoneNumber = model.PhoneNumber.Trim();
            address.AddressLine = model.AddressLine.Trim();
            address.City        = model.City.Trim();
            address.Region      = model.Region.Trim();
            address.PostalCode  = model.PostalCode.Trim();
            address.Country     = model.Country.Trim();
            address.IsDefault   = model.IsDefault;

            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Адресу оновлено.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null or 0)
                return NotFound();

            var address = _unitOfWork.Address.Get(a => a.Id == id.Value);

            if (address is null || !BelongsToCurrentUser(address))
                return NotFound();

            var model = new AddressViewModel
            {
                Id          = address.Id,
                FirstName   = address.FirstName,
                LastName    = address.LastName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,
                City        = address.City,
                Region      = address.Region,
                PostalCode  = address.PostalCode,
                Country     = address.Country,
                IsDefault   = address.IsDefault
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id is null or 0)
                return NotFound();

            var address = _unitOfWork.Address.Get(a => a.Id == id.Value);

            if (address is null || !BelongsToCurrentUser(address))
                return NotFound();

            _unitOfWork.Address.Remove(address);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Адресу видалено.";
            return RedirectToAction(nameof(Index));
        }

        // ─── SetDefault ───────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var address = _unitOfWork.Address.Get(a => a.Id == id, tracked: true);

            if (address is null || !BelongsToCurrentUser(address))
                return NotFound();

            _unitOfWork.Address.UnsetDefault(CurrentUserId);
            address.IsDefault = true;

            await _unitOfWork.SaveAsync();

            TempData["Success"] = $"Адресу «{address.City}, {address.AddressLine}» встановлено основною.";
            return RedirectToAction(nameof(Index));
        }
    }
}
