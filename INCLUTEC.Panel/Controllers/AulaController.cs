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

        public AulaController(IAulaService aulaService, IResponsableAulaService responsableAulaService)
        {
            _aulaService = aulaService;
            _responsableAulaService = responsableAulaService;
        }

        // El menú de INCLUTEC ya apunta a Index. Lo conservamos como acceso al listado.
        [HttpGet]
        public IActionResult Index(string? name, PaginatedRequest paginated)
        {
            return RedirectToAction(nameof(Aula), new { name, paginated.PageNumber, paginated.PageSize });
        }

        [HttpGet]
        public async Task<IActionResult> Aula(string? name, PaginatedRequest paginated)
        {
            paginated.PageNumber = Math.Max(1, paginated.PageNumber);
            paginated.PageSize = Math.Clamp(paginated.PageSize, 1, 100);
            ViewBag.Name = name;
            ViewBag.PageNumber = paginated.PageNumber;
            ViewBag.PageSize = paginated.PageSize;

            try
            {
                var result = await _aulaService.GetPaginatedAsync(name, paginated);
                if (result.Error) TempData["Error"] = await MensajeErrorAsync(result);
                await CargarResponsablesAsync();
                return View(result.Response ?? new List<AulaDto>());
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                TempData["Error"] = "No se pudo conectar con la API. Verificá que esté iniciada.";
                return View(new List<AulaDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AulaDetails(int id)
        {
            if (id < 0) return NotFound();
            await CargarResponsablesAsync();

            if (id == 0)
            {
                return View(new AulaDto { AñoLectivo = DateOnly.FromDateTime(DateTime.Today), EstadoActivo = true });
            }

            try
            {
                var result = await _aulaService.GetByIdAsync(id);
                if (result.Error || result.Response == null)
                {
                    TempData["Error"] = await MensajeErrorAsync(result);
                    return RedirectToAction(nameof(Aula));
                }
                return View(result.Response);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                TempData["Error"] = "No se pudo obtener el aula. Verificá la conexión con la API.";
                return RedirectToAction(nameof(Aula));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AulaDetails([FromRoute] int id, AulaDto aulaDto)
        {
            if (id != aulaDto.Id || id < 0) return BadRequest();

            aulaDto.Nombre = aulaDto.Nombre?.Trim();
            if (string.IsNullOrWhiteSpace(aulaDto.Nombre))
                ModelState.AddModelError(nameof(aulaDto.Nombre), "El nombre del aula es obligatorio.");
            else if (aulaDto.Nombre.Length > 50)
                ModelState.AddModelError(nameof(aulaDto.Nombre), "El nombre no puede superar los 50 caracteres.");

            if (aulaDto.AñoLectivo == DateOnly.MinValue)
                ModelState.AddModelError(nameof(aulaDto.AñoLectivo), "Ingresá una fecha lectiva válida.");

            if (!await CargarResponsablesAsync(aulaDto.ResponsableId))
                ModelState.AddModelError(string.Empty, "No se pueden verificar los responsables. Revisá la API antes de guardar.");
            else
            {
                var responsables = (List<ResponsableAulaDto>)ViewBag.ListaResponsables;
                if (!responsables.Any(r => r.Id == aulaDto.ResponsableId))
                    ModelState.AddModelError(nameof(aulaDto.ResponsableId), "Seleccioná un responsable existente.");
            }

            if (!ModelState.IsValid) return View(aulaDto);

            try
            {
                var result = aulaDto.Id == 0
                    ? await _aulaService.CreateAsync(aulaDto)
                    : await _aulaService.UpdateAsync(aulaDto);

                if (result.Error)
                {
                    ModelState.AddModelError(string.Empty, await MensajeErrorAsync(result));
                    return View(aulaDto);
                }

                TempData["Success"] = aulaDto.Id == 0 ? "El aula se creó correctamente." : "El aula se actualizó correctamente.";
                return RedirectToAction(nameof(Aula));
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar el aula. Verificá la conexión con la API.");
                return View(aulaDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return NotFound();
            try
            {
                var result = await _aulaService.GetByIdAsync(id);
                if (result.Error || result.Response == null)
                {
                    TempData["Error"] = await MensajeErrorAsync(result);
                    return RedirectToAction(nameof(Aula));
                }
                return View("AulaDelete", result.Response);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                TempData["Error"] = "No se pudo obtener el aula. Verificá la API.";
                return RedirectToAction(nameof(Aula));
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0) return BadRequest();
            try
            {
                var result = await _aulaService.DeleteAsync(id);
                TempData[result.Error ? "Error" : "Success"] = result.Error
                    ? await MensajeErrorAsync(result)
                    : "El aula se eliminó correctamente.";
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                TempData["Error"] = "No se pudo eliminar el aula. Verificá la conexión con la API.";
            }
            return RedirectToAction(nameof(Aula));
        }

        // Mantiene acceso a enlaces Details existentes, sin otra implementación del formulario.
        [HttpGet]
        public IActionResult Details(int id)
        {
            return RedirectToAction(nameof(AulaDetails), new { id });
        }

        private async Task<bool> CargarResponsablesAsync(int? seleccionado = null)
        {
            var responsables = new List<ResponsableAulaDto>();
            var paginated = new PaginatedRequest { PageNumber = 1, PageSize = 100 };
            bool disponibles = true;

            while (true)
            {
                var result = await _responsableAulaService.GetPaginatedAsync(null, paginated);
                if (result.Error || result.Response == null)
                {
                    disponibles = false;
                    TempData["Warning"] = "No se pudieron cargar los responsables. Revisá la API.";
                    break;
                }
                responsables.AddRange(result.Response);
                if (result.Response.Count < paginated.PageSize) break;
                paginated.PageNumber++;
            }

            ViewBag.ListaResponsables = responsables;
            ViewBag.Responsables = new SelectList(responsables.Select(r => new
            {
                r.Id,
                NombreCompleto = $"{r.Apellido}, {r.Nombre} ({r.Rol})"
            }), "Id", "NombreCompleto", seleccionado);

            if (disponibles && responsables.Count == 0)
                TempData["Warning"] = "Primero debe existir un responsable de aula cargado por el equipo correspondiente.";

            return disponibles;
        }

        private static async Task<string> MensajeErrorAsync<T>(HttpResponseWrapper<T> result)
        {
            if ((int)result.HttpResponseMessage.StatusCode >= 500)
                return "La API no pudo completar la operación. Si el aula tiene registros asociados, podés desactivarla en lugar de eliminarla.";

            var mensaje = result.ErrorMessage;
            if (!string.IsNullOrWhiteSpace(mensaje) && mensaje.Length <= 600 &&
                !mensaje.TrimStart().StartsWith('{') && !mensaje.TrimStart().StartsWith('<'))
                return mensaje.Trim('"');

            var general = await result.GetErrorMessage();
            return string.IsNullOrWhiteSpace(general) ? "No se pudo obtener el recurso solicitado." : general;
        }
    }
}