using INCLUTEC.Api.Code;
using INCLUTEC.Api.Infrastructure.Data;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Controllers
{

    public class RegistroAsistenciaController : ServiceControllerBase
    {
        private readonly InclutecbdContext _dbcontext;


        public RegistroAsistenciaController(InclutecbdContext dbcontext)
        {
            _dbcontext = dbcontext;

        }
        [HttpGet]
        public async Task<ActionResult<List<AulaDto>>> GetRegistros([FromQuery] PaginatedRequest paginated)
        {
            var obtener = await _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                   .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultadoDto = obtener.Select(c => new RegistroAsistenciaDto
            {
                Id = c.Id,
                Fecha = c.Fecha,
                EstudianteId = c.EstudianteId,
                EstaPresente = c.EstaPresente,
                Observaciones = c.Observaciones
            }).ToList();
            return Ok(resultadoDto);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegistroAsistencium>> ObtenerporID([FromQuery]int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null)
            {
                return NotFound();
            }
            var result = new RegistroAsistenciaDto
            {
                EstaPresente = buscar.EstaPresente,
                Id = buscar.Id,
                EstudianteId = buscar.EstudianteId,
                Fecha = buscar.Fecha,
                Observaciones = buscar.Observaciones

            };
            return Ok(result);

        }
        [HttpPost]
        public async Task<ActionResult<RegistroAsistencium>> Crear([FromBody] RegistroAsistenciaDto dto)
        {
            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (buscar != null)
            {
                return BadRequest("Ya existe un registro con ese id");
            }
            var registro = new RegistroAsistencium
            {
                EstaPresente = dto.EstaPresente,
                EstudianteId = dto.EstudianteId,
                Fecha = dto.Fecha,
                Observaciones = dto.Observaciones
            };
            dto.Id = registro.Id;
            _dbcontext.RegistroAsistencia.Add(registro);
            await _dbcontext.SaveChangesAsync();
            return Created();
            


        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RegistroAsistenciaDto>> Actualizar(int id, [FromBody] RegistroAsistenciaDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("El id no puede ser 0");
            }
            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id ==id);
            if (buscar is null)
            {
                return BadRequest("No existe El registro,Imposible actualizar");
            }
            buscar.Observaciones = dto.Observaciones;
            buscar.Fecha = dto.Fecha;
            buscar.EstaPresente = dto.EstaPresente;
            buscar.EstudianteId = dto.EstudianteId;
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar([FromQuery]int id)
        {
            if (id <= 0)
                return BadRequest("El id no puede ser 0");
            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null)
            {
                return NotFound("No se encontro la asistencia");
            }
            _dbcontext.RegistroAsistencia.Remove(buscar);
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }
        public async Task<ActionResult<List<RegistroAsistenciaDto>>> Search( [FromQuery] bool? estaPresente)
        {
            var query = _dbcontext.RegistroAsistencia.AsNoTracking().AsQueryable();
            
            if (estaPresente.HasValue)
            {
                query = query.Where(a => a.EstaPresente == estaPresente.Value);
            }

            var resultados = await query.Select(c => new RegistroAsistenciaDto
            {
                Id = c.Id,
                Fecha = c.Fecha,
                EstudianteId = c.EstudianteId,
                EstaPresente = c.EstaPresente,
                Observaciones = c.Observaciones
            }).ToListAsync();

            return Ok(resultados);
        }
    }
}
