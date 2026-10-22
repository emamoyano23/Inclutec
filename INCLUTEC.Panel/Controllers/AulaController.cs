using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Helpers;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INCLUTEC.Panel.Controllers
{
    public class AulaController : Controller
    {
        private readonly IAulaService _aulaService;
        private readonly IDropdownService _dropdownService;

        public AulaController(
            IAulaService aulaService, IDropdownService dropdownService)
        {
            _aulaService = aulaService;
            _dropdownService = dropdownService;
        }


        public async Task<IActionResult> Aula(string name, PaginatedRequest paginated)
        {
            var result = await _aulaService.GetPaginatedAsync(name, paginated);
            ViewBag.PageNumber = paginated.PageNumber;
            ViewBag.PageSize = paginated.PageSize;
            return View(result.Response);
        }

        public async Task<IActionResult> AulaDetails(int Id)
        {
            AulaDto model = new();
            ViewBag.Text = "Nueva Aula";
            ViewBag.SelectedResponsable = await _dropdownService.GetResponsableAulaAsync();

            if (Id != 0)
            {
                var result = await _aulaService.GetByIdAsync(Id);
                if (!result.Error)
                {
                    ViewBag.Text = "Modificando Aula";
                    ViewBag.SelectedResponsable = await _dropdownService.GetResponsableAulaAsync();
                    model = result.Response ?? new AulaDto();
                }

            }
            return View(model);

        }

        public async Task<IActionResult> SaveChanges(AulaDto model)
        {
            HttpResponseWrapper<object> result;
            if (model.Id == 0)
            {
                result = await _aulaService.CreateAsync(model);
            }
            else
            {
                result = await _aulaService.UpdateAsync(model);
            }
            if (!result.Error)
            {
                if (model.Id == 0)
                {
                    TempData["Success"] = "¡Aula creado con éxito!";
                }
                else
                {
                    TempData["Warning"] = "¡Aula modificada con éxito!";
                }
                return RedirectToAction("Aula");
            }
            else
            {
                TempData["Error"] = "¡Ocurrió un error al guardar los cambios!";
                return RedirectToAction("AulaDetails", model);
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
    }
}
