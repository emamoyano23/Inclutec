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


        [HttpGet("ResponsableAulaDetails/{id?}")]
        public async Task<IActionResult> ResponsableAulaDetails(int id = 0)
        {
            ResponsableAulaDto model = new();
            ViewBag.Text = "Nuevo Responsable";

            if (id != 0)
            {
                var result = await _responsableAulaService.GetByIdAsync(id);
                if (!result.Error)
                {
                    ViewBag.Text = "Modificando el Responsable";
                    model = result.Response ?? new ResponsableAulaDto();
                }
            }

            return View(model);
        }



        [HttpPost("SaveChanges")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChanges(ResponsableAulaDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Text = model.Id != 0 ? "Modificando el Responsable" : "Nuevo Responsable";
                return View("ResponsableAulaDetails", model);
            }

            HttpResponseWrapper<object> result;
            if (model.Id != 0)
            {
                result = await _responsableAulaService.UpdateAsync(model);
            }
            else
            {
                result = await _responsableAulaService.CreateAsync(model);
            }

            if (!result.Error)
            {
                return RedirectToAction(nameof(ResponsableAula));
            }

            return NotFound();
        }
