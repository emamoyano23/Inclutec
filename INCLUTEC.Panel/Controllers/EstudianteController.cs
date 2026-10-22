using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Helpers;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INCLUTEC.Panel.Controllers
{

    public class EstudianteController : Controller
{
    private readonly IEstudianteService _estudianteService;
    private readonly IDropdownService _dropdownService;

    public EstudianteController(
        IEstudianteService estudianteService, IDropdownService dropdownService)
    {
        _estudianteService = estudianteService;
        _dropdownService = dropdownService;
    }

    public async Task<IActionResult> Estudiante(string name, PaginatedRequest paginated)
    {
        var result = await _estudianteService.GetPaginatedAsync(name, paginated);
        ViewBag.PageNumber = paginated.PageNumber;
        ViewBag.PageSize = paginated.PageSize;
        return View(result.Response);
    }
    public async Task<IActionResult> EstudianteDetails(int Id)
    {
        EstudianteDto model = new();
        ViewBag.Text = "Nuevo Estudiante";
        ViewBag.SelectedAula = await _dropdownService.GetAulasAsync();

        if (Id != 0)
        {
            var result = await _estudianteService.GetByIdAsync(Id);
            if (!result.Error)
            {
                ViewBag.Text = "Modificando Estudiante";
                ViewBag.SelectedAula = await _dropdownService.GetAulasAsync();
                model = result.Response ?? new EstudianteDto();
            }
        }
        return View(model);
    }
    [HttpPost] 
    
    public async Task<IActionResult> SaveChanges(EstudianteDto model)
    {
        HttpResponseWrapper<object> result;
        if (model.Id == 0)
        {
            result = await _estudianteService.CreateAsync(model);
        }
        else
        {
            result = await _estudianteService.UpdateAsync(model);
        }
        if (!result.Error)
        {
            if (model.Id == 0)
            {
                TempData["Success"] = "¡Estudiante creado con éxito!";
            }
            else
            {
                TempData["Warning"] = "¡Estudiante modificado con éxito!";
            }
            return RedirectToAction("Estudiante");
        }
        else
        {
            TempData["Error"] = "¡Ocurrió un error al guardar los cambios!";
            return RedirectToAction("EstudianteDetails", model);
        }
    }
}
}


