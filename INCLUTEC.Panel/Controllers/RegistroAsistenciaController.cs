using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure;
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
        [HttpPut]
        public async Task<IActionResult> Update(RegistroAsistenciaDto dto)
        {
             var update = await _service.Update(dto);
            return View(update);
        }
        public async Task<IActionResult> Index(string? name)
        {
            var pagination = new PaginatedRequest
            {
                PageNumber = 1,
                PageSize = 10
            };
            var result = await _service.GetPaginatedAsync(name, pagination);
            return View(result.Response);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return View(new RegistroAsistenciaDto
                {
                    EstaPresente = true
                });
            }
            var result = await _service.GetID(id);
            return View(result.Response);
        }
        [HttpPost]
        public async Task<IActionResult> Details(RegistroAsistenciaDto registroAsistenciaDto)
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
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
