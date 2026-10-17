using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Controllers
{
    public class RegistroAsistenciaController : Controller
    {
        private readonly IRegistroAsistenciaService _registroAsistenciaService;
        private readonly IAulaService _aulaService;

        public RegistroAsistenciaController(IRegistroAsistenciaService registroAsistenciaService, IAulaService aulaService)
        {
            _registroAsistenciaService = registroAsistenciaService;
            _aulaService = aulaService;
        }

        [Route("RegistroAsistencia")]
        public async Task<IActionResult> RegistroAsistencia( [FromQuery] PaginatedRequest paginated)
        {
            var result = await _registroAsistenciaService.GetResumenAsync(paginated);
            ViewBag.PageNumber = paginated.PageNumber;
            ViewBag.PageSize = paginated.PageSize;
            return View(result.Response ?? new List<RegistroAsistenciaResumenDto>());
        }

        public async Task<IActionResult> RegistroAsistenciaDetails(DateTime? fecha, int aulaId, PaginatedRequest paginated)
        {
            var fechaSel = (fecha ?? DateTime.Today).Date;

            var aulas = await _aulaService.GetPaginatedAsync(null, paginated);
            ViewBag.Aulas = aulas?.Response ?? new List<AulaDto>();
            ViewBag.AulaId = aulaId;
            ViewBag.Fecha = fechaSel;
            ViewBag.Text = "Tomar Asistencia";

            var model = new List<RegistroAsistenciaDto>();
            if (aulaId != 0)
            {
                var result = await _registroAsistenciaService.GetAlumnosAulaAsync(aulaId, fechaSel);
                if (!result.Error)
                {
                    model = result.Response ?? model;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsistenciasMasivas(List<RegistroAsistenciaDto> model)
        {
            var result = await _registroAsistenciaService.CreateBulkAsync(model);

            if (!result.Error)
            {
                return RedirectToAction("RegistroAsistencia");
            }
            else
            {
                TempData["Error"] = "Error al guardar: " + result.HttpResponseMessage?.StatusCode;
                return RedirectToAction("RegistroAsistenciaDetails");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DateTime fecha, int aulaId)
        {
            await _registroAsistenciaService.DeleteGrupoAsync(fecha, aulaId);
            return RedirectToAction("RegistroAsistencia");
        }
    }
}