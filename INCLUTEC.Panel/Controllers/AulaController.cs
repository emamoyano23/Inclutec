using INCLUTEC.Entities.Dtos;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Controllers
{
    public class AulaController : Controller
    {
        private readonly IAulaService _aulaService;

        public AulaController(IAulaService aulaService)
        {
            _aulaService = aulaService;
        }

        public async Task<IActionResult> Index(string? name)
        {
            var paginated = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _aulaService.GetPaginatedAsync(name, paginated);

            return View(result.Response);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return View(new AulaDto
                {
                    EstadoActivo = true
                });
            }

            var result = await _aulaService.GetID(id);

            return View(result.Response);
        }

        [HttpPost]
        public async Task<IActionResult> Details(AulaDto aulaDto)
        {
            if (!ModelState.IsValid)
            {
                return View(aulaDto);
            }

            if (aulaDto.Id == 0)
            {
                await _aulaService.CreateAsync(aulaDto);
            }
            else
            {
                await _aulaService.Update(aulaDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _aulaService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}