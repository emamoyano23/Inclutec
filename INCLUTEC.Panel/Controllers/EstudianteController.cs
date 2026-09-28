using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly IEstudianteService _estudianteService;

        public EstudianteController(IEstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        public async Task<IActionResult> Index(string? nombre, int? aulaId, [FromQuery] PaginatedRequest paginated)
        {
            var result = await _estudianteService.GetPaginatedAsync(nombre, aulaId, paginated);

            if (result.Error)
            {
                TempData["Error"] = await result.GetErrorMessage();
            }

            ViewBag.Nombre = nombre;
            ViewBag.AulaId = aulaId;
            ViewBag.PageNumber = paginated.PageNumber;
            ViewBag.PageSize = paginated.PageSize;

            return View(result.Response ?? new List<EstudianteDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _estudianteService.GetByIdAsync(id);

            if (result.Error || result.Response == null)
            {
                TempData["Error"] = await result.GetErrorMessage();

                return RedirectToAction(nameof(Index));
            }

            return View(result.Response);
        }
    }
}
