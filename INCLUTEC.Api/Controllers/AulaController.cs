using INCLUTEC.Api.Code;
using INCLUTEC.Api.Infrastructure.Data;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Controllers
{
    public class AulaController : ServiceControllerBase
    {
        private readonly InclutecbdContext _dbContext;

        public AulaController(InclutecbdContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<AulaDto>>> Get([FromQuery] PaginatedRequest paginated)
        {
            var query = await _dbContext.Aulas
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultDto = query.Select(a => new AulaDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                AñoLectivo = a.AñoLectivo,
                ResponsableId = a.ResponsableId,
                EstadoActivo = a.EstadoActivo

            }).ToList();

            return Ok(resultDto);
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<List<AulaDto>>> Search(
            [FromQuery] string? nombre,
            [FromQuery] PaginatedRequest paginated)
        {
            var query = await _dbContext.Aulas
                .AsNoTracking()
                .Where(a => string.IsNullOrEmpty(nombre) || a.Nombre.Contains(nombre))
                .OrderByDescending(a => a.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultDto = query.Select(a => new AulaDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                AñoLectivo = a.AñoLectivo,
                ResponsableId = a.ResponsableId,
                EstadoActivo = a.EstadoActivo

            }).ToList();

            return Ok(resultDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<AulaDto>> GetById(int id)
        {
            var query = await _dbContext.Aulas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (query == null)
            {
                return NotFound();
            }

            var resultDto = new AulaDto
            {
                Id = query.Id,
                Nombre = query.Nombre,
                AñoLectivo = query.AñoLectivo,
                ResponsableId = query.ResponsableId,
                EstadoActivo = query.EstadoActivo
            };

            return Ok(resultDto);
        }

        [HttpPost]
        public async Task<ActionResult<AulaDto>> Create([FromBody] AulaDto aulaDto)
        {
            var query = await _dbContext.Aulas
                .FirstOrDefaultAsync(a => a.Nombre == aulaDto.Nombre);

            if (query != null)
            {
                return BadRequest("Ya existe un aula registrada con ese nombre");
            }

            var aula = new Aula
            {
                Id = aulaDto.Id,
                Nombre = aulaDto.Nombre!,
                AñoLectivo = aulaDto.AñoLectivo,
                ResponsableId = aulaDto.ResponsableId,
                EstadoActivo = aulaDto.EstadoActivo
            };

            _dbContext.Aulas.Add(aula);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<AulaDto>> Update(
            int id,
            [FromBody] AulaDto aulaDto)
        {
            var query = await _dbContext.Aulas
                .FirstOrDefaultAsync(a => a.Id == id);

            if (query == null)
            {
                return NotFound();
            }

            query.Nombre = aulaDto.Nombre!;
            query.AñoLectivo = aulaDto.AñoLectivo;
            query.ResponsableId = aulaDto.ResponsableId;
            query.EstadoActivo = aulaDto.EstadoActivo;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var query = await _dbContext.Aulas
                .FirstOrDefaultAsync(a => a.Id == id);

            if (query == null)
            {
                return NotFound();
            }

            _dbContext.Aulas.Remove(query);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}