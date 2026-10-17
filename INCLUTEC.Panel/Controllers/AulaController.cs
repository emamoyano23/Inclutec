using INCLUTEC.Entities.Dtos;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;







namespace INCLUTEC.Panel.Controllers
{
    public class AulaController : Controller
    {
        private readonly IAulaService _aulaService;
        private readonly IResponsableAulaService _responsableAulaService;

        public AulaController(
            IAulaService aulaService,
            IResponsableAulaService responsableAulaService)
        {
            _aulaService = aulaService;
            _responsableAulaService = responsableAulaService;
        }

        public async Task<IActionResult> Index(string? name)
        {
            var paginated = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _aulaService.GetPaginatedAsync(name, paginated);

            return View("Aula", result.Response);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            AulaDto aulaDto;

            if (id == 0)
            {
                aulaDto = new AulaDto
                {
                    EstadoActivo = true
                };
            }
            else
            {
                var result = await _aulaService.GetID(id);

                if (result.Error || result.Response == null)
                {
                    return NotFound();
                }

                aulaDto = result.Response;
            }

            await CargarResponsablesAsync(aulaDto.ResponsableId);

            return View("AulaDetails", aulaDto);
        }

        [HttpPost]
        public async Task<IActionResult> Details(AulaDto aulaDto)
        {
            if (!ModelState.IsValid)
            {
                await CargarResponsablesAsync(aulaDto.ResponsableId);

                return View("AulaDetails", aulaDto);
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

        private async Task CargarResponsablesAsync(int? responsableSeleccionado = null)
        {
            var paginated = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 1000
            };

            var result = await _responsableAulaService.GetPaginatedAsync(
                null,
                paginated
            );

            var responsables = result.Error
                ? new List<ResponsableAulaDto>()
                : result.Response ?? new List<ResponsableAulaDto>();

            ViewBag.Responsables = new SelectList(
                responsables.Select(r => new
                {
                    r.Id,
                    NombreCompleto = $"{r.Nombre} {r.Apellido}"
                }),
                "Id",
                "NombreCompleto",
                responsableSeleccionado
            );

            if (result.Error)
            {
                ViewBag.ErrorResponsables =
                    "No se pudieron cargar los responsables. Verificá que la API esté iniciada.";
            }
        }
    }
}
