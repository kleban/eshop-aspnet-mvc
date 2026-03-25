using eShop.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShopMVC.App.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
