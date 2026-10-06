using INCLUTEC.Api.Code;
using INCLUTEC.Api.Infrastructure.Data;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/estudiantes")]
    public class EstudianteController : ServiceControllerBase
    {
        private readonly InclutecbdContext _dbContext;

        public EstudianteController(InclutecbdContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<EstudianteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EstudianteDto>>> Get([FromQuery] PaginatedRequest paginated)
        {
            var resultDto = await _dbContext.Estudiantes
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    AvatarUrlPictogramaPath = e.AvatarUrlPictogramaPath,
                    AulaId = e.AulaId,
                    NombreAula = _dbContext.Aulas
                        .Where(a => a.Id == e.AulaId)
                        .Select(a => a.Nombre)
                        .FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .ToListAsync();

            return Ok(resultDto);
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(List<EstudianteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EstudianteDto>>> Search(
            [FromQuery] string? nombre,
            [FromQuery] int? aulaId,
            [FromQuery] bool? estadoActivo,
            [FromQuery] PaginatedRequest paginated)
        {
            var resultDto = await _dbContext.Estudiantes
                .AsNoTracking()
                .Where(e => string.IsNullOrEmpty(nombre)
                            || e.Nombre.Contains(nombre)
                            || e.Apellido.Contains(nombre))
                .Where(e => aulaId == null || e.AulaId == aulaId)
                .Where(e => estadoActivo == null || e.EstadoActivo == estadoActivo)
                .OrderByDescending(e => e.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    AvatarUrlPictogramaPath = e.AvatarUrlPictogramaPath,
                    AulaId = e.AulaId,
                    NombreAula = _dbContext.Aulas
                        .Where(a => a.Id == e.AulaId)
                        .Select(a => a.Nombre)
                        .FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .ToListAsync();

            return Ok(resultDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstudianteDto>> GetById(int id)
        {
            var resultDto = await _dbContext.Estudiantes
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    AvatarUrlPictogramaPath = e.AvatarUrlPictogramaPath,
                    AulaId = e.AulaId,
                    NombreAula = _dbContext.Aulas
                        .Where(a => a.Id == e.AulaId)
                        .Select(a => a.Nombre)
                        .FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .FirstOrDefaultAsync();

            if (resultDto == null)
            {
                return NotFound($"No existe un estudiante con el Id {id}.");
            }

            return Ok(resultDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstudianteDto>> Create([FromBody] EstudianteDto estudianteDto)
        {
            var existeAula = await _dbContext.Aulas.AnyAsync(a => a.Id == estudianteDto.AulaId);

            if (!existeAula)
            {
                return BadRequest($"No existe un aula con el Id {estudianteDto.AulaId}.");
            }

            var duplicado = await _dbContext.Estudiantes.AnyAsync(e =>
                e.AulaId == estudianteDto.AulaId &&
                e.Nombre == estudianteDto.Nombre &&
                e.Apellido == estudianteDto.Apellido);

            if (duplicado)
            {
                return BadRequest("Ya existe un estudiante con ese nombre y apellido en esa aula.");
            }

            var estudiante = new Estudiante
            {
                Nombre = estudianteDto.Nombre!,
                Apellido = estudianteDto.Apellido!,
                AvatarUrlPictogramaPath = estudianteDto.AvatarUrlPictogramaPath!,
                AulaId = estudianteDto.AulaId,
                EstadoActivo = estudianteDto.EstadoActivo
            };

            _dbContext.Estudiantes.Add(estudiante);

            await _dbContext.SaveChangesAsync();

            estudianteDto.Id = estudiante.Id;

            estudianteDto.NombreAula = await _dbContext.Aulas
                .Where(a => a.Id == estudianteDto.AulaId)
                .Select(a => a.Nombre)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, estudianteDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] EstudianteDto estudianteDto)
        {
            var estudiante = await _dbContext.Estudiantes.FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound($"No existe un estudiante con el Id {id}.");
            }

            var existeAula = await _dbContext.Aulas.AnyAsync(a => a.Id == estudianteDto.AulaId);

            if (!existeAula)
            {
                return BadRequest($"No existe un aula con el Id {estudianteDto.AulaId}.");
            }

            var duplicado = await _dbContext.Estudiantes.AnyAsync(e =>
                e.Id != id &&
                e.AulaId == estudianteDto.AulaId &&
                e.Nombre == estudianteDto.Nombre &&
                e.Apellido == estudianteDto.Apellido);

            if (duplicado)
            {
                return BadRequest("Ya existe otro estudiante con ese nombre y apellido en esa aula.");
            }

            estudiante.Nombre = estudianteDto.Nombre!;
            estudiante.Apellido = estudianteDto.Apellido!;
            estudiante.AvatarUrlPictogramaPath = estudianteDto.AvatarUrlPictogramaPath!;
            estudiante.AulaId = estudianteDto.AulaId;
            estudiante.EstadoActivo = estudianteDto.EstadoActivo;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete(int id)
        {
            var estudiante = await _dbContext.Estudiantes.FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound($"No existe un estudiante con el Id {id}.");
            }

            var tieneAsistencias = await _dbContext.RegistroAsistencia.AnyAsync(r => r.EstudianteId == id);

            if (tieneAsistencias)
            {
                return Conflict("No se puede eliminar el estudiante porque tiene registros de asistencia asociados. Desactivelo con EstadoActivo en false.");
            }

            _dbContext.Estudiantes.Remove(estudiante);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
