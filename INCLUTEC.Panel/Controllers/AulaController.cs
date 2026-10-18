using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
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


        public async Task<IActionResult> Aula(string name, PaginatedRequest paginated)
        {
            var result = await _aulaService.GetPaginatedAsync(name, paginated);
            return View(result.Response);
        }

        public async Task<IActionResult> AulaDetails(int Id)
        {
            AulaDto model = new();
            ViewBag.Text = "Nueva Aula";

            if (Id != 0)
            {
                var result = await _aulaService.GetByIdAsync(Id);
                if (!result.Error)
                {
                    ViewBag.Text = "Modificando Aula";
                    model = result.Response ?? new AulaDto();
                }

            }
            await CargarResponsablesAsync(model.ResponsableId);
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges(AulaDto model)
        {
            HttpResponseWrapper<object> result;
            if (model.Id != 0)
            {
                result = await _aulaService.UpdateAsync(model);
            }
            else
            {
                result = await _aulaService.CreateAsync(model);

            }

            if (!result.Error)
            {
                return RedirectToAction("Aula");
            }
            else
            {
                return NotFound();
            }
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _aulaService.GetByIdAsync(id);

            if (result.Error || result.Response == null)
            {
                return NotFound();
            }

            return View("AulaDelete", result.Response);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _aulaService.DeleteAsync(id);

            if (!result.Error)
            {
                return RedirectToAction(nameof(Aula));
            }

            return NotFound();
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
