using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace INCLUTEC.Panel.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly IEstudianteService _estudianteService;
        private readonly IAulaService _aulaService;

        public EstudianteController(IEstudianteService estudianteService, IAulaService aulaService)
        {
            _estudianteService = estudianteService;
            _aulaService = aulaService;
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

            return View("Estudiante", result.Response ?? new List<EstudianteDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            EstudianteDto estudianteDto;

            if (id == 0)
            {
                estudianteDto = new EstudianteDto
                {
                    EstadoActivo = true
                };
            }
            else
            {
                var result = await _estudianteService.GetByIdAsync(id);

                if (result.Error || result.Response == null)
                {
                    TempData["Error"] = await result.GetErrorMessage();

                    return RedirectToAction(nameof(Index));
                }

                estudianteDto = result.Response;
            }

            await CargarAulasAsync(estudianteDto.AulaId);

            return View("EstudianteDetails", estudianteDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(EstudianteDto estudianteDto, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("EstudianteDetails", estudianteDto);
            }

            if (!await SubirAvatarAsync(avatar, estudianteDto))
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("EstudianteDetails", estudianteDto);
            }

            var esNuevo = estudianteDto.Id == 0;

            var result = esNuevo
                ? await _estudianteService.CreateAsync(estudianteDto)
                : await _estudianteService.UpdateAsync(estudianteDto.Id, estudianteDto);

            if (result.Error)
            {
                TempData["Error"] = await MensajeDeErrorAsync(result);

                await CargarAulasAsync(estudianteDto.AulaId);

                return View("EstudianteDetails", estudianteDto);
            }

            TempData["Success"] = esNuevo
                ? "El estudiante se dio de alta correctamente."
                : "Los datos del estudiante se actualizaron correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estudianteService.GetByIdAsync(id);

            if (result.Error || result.Response == null)
            {
                TempData["Error"] = await result.GetErrorMessage();

                return RedirectToAction(nameof(Index));
            }

            return View("EstudianteDelete", result.Response);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmado(int id)
        {
            var result = await _estudianteService.DeleteAsync(id);

            if (result.Error)
            {
                TempData["Error"] = await MensajeDeErrorAsync(result);
            }
            else
            {
                TempData["Success"] = "El estudiante se elimino correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Avatar(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return NotFound();
            }

            var response = await _estudianteService.GetAvatarAsync(fileName);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var contenido = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";

            return File(contenido, contentType);
        }

        private async Task<bool> SubirAvatarAsync(IFormFile? avatar, EstudianteDto estudianteDto)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return true;
            }

            await using var contenido = avatar.OpenReadStream();

            var result = await _estudianteService.UploadAvatarAsync(contenido, avatar.FileName, avatar.ContentType);

            if (result.Error || string.IsNullOrWhiteSpace(result.Response?.FileName))
            {
                TempData["Error"] = await MensajeDeErrorAsync(result);

                return false;
            }

            estudianteDto.AvatarUrlPictogramaPath = result.Response.FileName;

            return true;
        }

        private static async Task<string> MensajeDeErrorAsync<T>(Infrastructure.Api.Services.HttpResponseWrapper<T> result)
        {
            if (string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                return await result.GetErrorMessage();
            }

            try
            {
                using var json = JsonDocument.Parse(result.ErrorMessage);
                var raiz = json.RootElement;

                if (raiz.ValueKind == JsonValueKind.Object)
                {
                    if (raiz.TryGetProperty("detail", out var detalle) && detalle.ValueKind == JsonValueKind.String)
                    {
                        return detalle.GetString() ?? result.ErrorMessage;
                    }

                    if (raiz.TryGetProperty("errors", out var errores) && errores.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var campo in errores.EnumerateObject())
                        {
                            foreach (var mensaje in campo.Value.EnumerateArray())
                            {
                                return mensaje.GetString() ?? result.ErrorMessage;
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
            }

            return result.ErrorMessage;
        }

        private async Task CargarAulasAsync(int? aulaSeleccionada = null)
        {
            var paginated = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 100,
                busqueda = string.Empty
            };

            var result = await _aulaService.GetPaginatedAsync(null, paginated);

            var aulas = result.Response ?? new List<AulaDto>();

            if (aulas.Count == 0)
            {
                TempData["Warning"] = "Todavia no hay aulas cargadas: primero hay que crear un aula.";
            }

            ViewBag.Aulas = new SelectList(aulas, nameof(AulaDto.Id), nameof(AulaDto.Nombre), aulaSeleccionada);
        }
    }
}
