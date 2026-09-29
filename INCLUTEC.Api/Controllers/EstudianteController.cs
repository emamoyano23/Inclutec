using System.ComponentModel.DataAnnotations;
using INCLUTEC.Api.Code;
using INCLUTEC.Api.Infrastructure.Data;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Controllers
{
    public class EstudianteController : ServiceControllerBase
    {
        private readonly InclutecbdContext _dbcontext;

        public EstudianteController(InclutecbdContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        private static List<string> Validar(Estudiante estudiante)
        {
            var errores = new List<ValidationResult>();
            var contexto = new ValidationContext(estudiante);
            Validator.TryValidateObject(estudiante, contexto, errores, validateAllProperties: true);
            return errores.Select(e => e.ErrorMessage ?? "Dato inválido").ToList();
        }

        [HttpGet]
        public async Task<ActionResult<List<EstudianteDto>>> Get([FromQuery] PaginatedRequest paginated)
        {
            var resultadoDto = await _dbcontext.Estudiantes
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
                    NombreAula = _dbcontext.Aulas.Where(a => a.Id == e.AulaId).Select(a => a.Nombre).FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .ToListAsync();

            return Ok(resultadoDto);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<EstudianteDto>>> Search([FromQuery] string? nombre, [FromQuery] PaginatedRequest paginated)
        {
            var resultDto = await _dbcontext.Estudiantes
                .AsNoTracking()
                .Where(e => string.IsNullOrEmpty(nombre) || e.Nombre.Contains(nombre) || e.Apellido.Contains(nombre))
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
                    NombreAula = _dbcontext.Aulas.Where(a => a.Id == e.AulaId).Select(a => a.Nombre).FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .ToListAsync();

            return Ok(resultDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstudianteDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El id no puede ser 0");
            }

            var result = await _dbcontext.Estudiantes
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EstudianteDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    AvatarUrlPictogramaPath = e.AvatarUrlPictogramaPath,
                    AulaId = e.AulaId,
                    NombreAula = _dbcontext.Aulas.Where(a => a.Id == e.AulaId).Select(a => a.Nombre).FirstOrDefault(),
                    EstadoActivo = e.EstadoActivo
                })
                .FirstOrDefaultAsync();

            if (result is null)
            {
                return NotFound($"El estudiante {id} no existe");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EstudianteDto>> Create([FromBody] EstudianteDto dto)
        {
            var estudiante = new Estudiante
            {
                Nombre = dto.Nombre!,
                Apellido = dto.Apellido!,
                AvatarUrlPictogramaPath = dto.AvatarUrlPictogramaPath!,
                AulaId = dto.AulaId,
                EstadoActivo = dto.EstadoActivo
            };

            var errores = Validar(estudiante);
            if (errores.Count > 0)
            {
                return BadRequest(errores);
            }

            var aula = await _dbcontext.Aulas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == dto.AulaId);
            if (aula is null)
            {
                return BadRequest($"El aula con ID {dto.AulaId} no existe en la base de datos");
            }

            _dbcontext.Estudiantes.Add(estudiante);

            await _dbcontext.SaveChangesAsync();

            dto.Id = estudiante.Id;
            dto.NombreAula = aula.Nombre;
            return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] EstudianteDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("El id no puede ser 0");
            }

            var buscar = await _dbcontext.Estudiantes.FirstOrDefaultAsync(e => e.Id == id);
            if (buscar is null)
            {
                return NotFound("No existe el estudiante, imposible actualizar");
            }

            buscar.Nombre = dto.Nombre!;
            buscar.Apellido = dto.Apellido!;
            buscar.AvatarUrlPictogramaPath = dto.AvatarUrlPictogramaPath!;
            buscar.AulaId = dto.AulaId;
            buscar.EstadoActivo = dto.EstadoActivo;

            var errores = Validar(buscar);
            if (errores.Count > 0)
            {
                return BadRequest(errores);
            }

            var existeAula = await _dbcontext.Aulas.AnyAsync(a => a.Id == dto.AulaId);
            if (!existeAula)
            {
                return BadRequest($"El aula con ID {dto.AulaId} no existe en la base de datos");
            }

            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El id no puede ser 0");
            }

            var buscar = await _dbcontext.Estudiantes.FirstOrDefaultAsync(e => e.Id == id);
            if (buscar is null)
            {
                return NotFound("No se encontro el estudiante");
            }

            var tieneAsistencias = await _dbcontext.RegistroAsistencia.AnyAsync(r => r.EstudianteId == id);
            if (tieneAsistencias)
            {
                return BadRequest("No se puede eliminar el estudiante porque tiene registros de asistencia");
            }

            _dbcontext.Estudiantes.Remove(buscar);
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }
    }
}
