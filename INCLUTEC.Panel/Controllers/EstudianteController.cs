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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarAulasAsync();

            return View("Form", new EstudianteDto { EstadoActivo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstudianteDto estudianteDto, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            if (!await SubirAvatarAsync(avatar, estudianteDto))
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            var result = await _estudianteService.CreateAsync(estudianteDto);

            if (result.Error)
            {
                TempData["Error"] = await MensajeDeErrorAsync(result);

                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            TempData["Success"] = "El estudiante se dio de alta correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _estudianteService.GetByIdAsync(id);

            if (result.Error || result.Response == null)
            {
                TempData["Error"] = await result.GetErrorMessage();

                return RedirectToAction(nameof(Index));
            }

            await CargarAulasAsync(result.Response.AulaId);

            return View("Form", result.Response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstudianteDto estudianteDto, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            if (!await SubirAvatarAsync(avatar, estudianteDto))
            {
                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            var result = await _estudianteService.UpdateAsync(id, estudianteDto);

            if (result.Error)
            {
                TempData["Error"] = await MensajeDeErrorAsync(result);

                await CargarAulasAsync(estudianteDto.AulaId);

                return View("Form", estudianteDto);
            }

            TempData["Success"] = "Los datos del estudiante se actualizaron correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

        // La API devuelve el motivo en el cuerpo (por ejemplo el 409 al borrar un
        // estudiante con asistencias). Si viene vacio, caigo en el mensaje generico.
        // Muestra el pictograma que la API guarda fuera de wwwroot. El navegador no le pega
        // a la API directo: el Panel lo pide y lo devuelve.
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

        // Si eligieron una imagen nueva la sube a la API y guarda en el DTO el nombre que
        // devuelve. Si no eligieron ninguna, queda la que ya tenia.
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

            // Si la API devolvio un ProblemDetails, muestro el texto y no el JSON entero.
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
                // No era JSON, es un texto plano: lo muestro tal cual.
            }

            return result.ErrorMessage;
        }

        // Carga el desplegable de aulas del formulario. El docente elige el aula por
        // nombre en vez de tener que saberse el Id de memoria.
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
