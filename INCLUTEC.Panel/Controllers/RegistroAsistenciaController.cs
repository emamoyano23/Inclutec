using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Controllers
{
    public class RegistroAsistenciaController : Controller
    {
        private readonly IRegistroAsistenciaService _service;
        public RegistroAsistenciaController(IRegistroAsistenciaService service)
        {
            _service = service;
        }

        [Route("RegistroAsistencia")]
        public async Task<IActionResult> RegistroAsistencia(string? busqueda)
        {
            var pagination = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                busqueda = busqueda ?? string.Empty
            };
            var result = await _service.GetPaginatedAsync(busqueda, pagination);
            var modelo = result?.Response ?? new List<RegistroAsistenciaDto>();

            return View("RegistroAsistencia", modelo);
        }
        [HttpGet]
        public async Task<IActionResult> RegistroAsistenciaDetails(int id)
        {
            if (id == 0)
            {
                return View(new RegistroAsistenciaDto
                {
                    Fecha = DateTime.Now,
                    EstaPresente = true
                });
                
            }
            var result = await _service.GetID(id);
            return View( result.Response ?? new RegistroAsistenciaDto());
        }
        [HttpPost]
        public async Task<IActionResult> RegistroAsistenciaDetails(RegistroAsistenciaDto registroAsistenciaDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registroAsistenciaDto);
            }
            if (registroAsistenciaDto.Id == 0)
            {
                await _service.CreateAsync(registroAsistenciaDto);
            }
            else
            {
                await _service.Update(registroAsistenciaDto);
            }
            return RedirectToAction(nameof(RegistroAsistencia));
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(RegistroAsistencia));
        }
    }
}
