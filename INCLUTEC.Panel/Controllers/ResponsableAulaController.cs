using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Panel.Controllers
{
    public class ResponsableAulaController : Controller
    {

        private readonly IResponsableAulaService _responsableAulaService;

        public ResponsableAulaController(IResponsableAulaService responsableAulaService)
        {
            _responsableAulaService = responsableAulaService;
        }


        [Route("ResponsableAula")]
        public async Task<IActionResult> ResponsableAula(string name, PaginatedRequest paginated)
        {

            var result = await _responsableAulaService.GetPaginatedAsync(name, paginated);

            ViewBag.PageNumber = paginated.PageNumber;
            ViewBag.PageSize = paginated.PageSize;
            ViewBag.SearchName = name;

            return View(result.Response);
        }


    }
}

       
