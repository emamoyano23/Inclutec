using System.ComponentModel.DataAnnotations;
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

        private static List<string> Validar(RegistroAsistencium registro)
        {
            var errores = new List<ValidationResult>();
            var contexto = new ValidationContext(registro);
            Validator.TryValidateObject(registro, contexto, errores, validateAllProperties: true);
            return errores.Select(e => e.ErrorMessage ?? "Dato inválido").ToList();
        }


        [HttpGet]
        public async Task<ActionResult<List<RegistroAsistenciaDto>>> Get([FromQuery] PaginatedRequest paginated)
        {
            var obtener = await _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .Include(c => c.Estudiante)
                    .ThenInclude(e => e.Aula)
                .OrderByDescending(c => c.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultadoDto = obtener.Select(c => new RegistroAsistenciaDto
            {
                Id = c.Id,
                Fecha = c.Fecha,
                EstudianteId = c.EstudianteId,
                EstaPresente = c.EstaPresente,
                Observaciones = c.Observaciones,
                NombreCompletoEstudiante = c.Estudiante.Nombre,
                NombreAula = c.Estudiante.Aula?.Nombre ?? "SIN AULA"
            }).ToList();

            return Ok(resultadoDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegistroAsistenciaDto>> GetById(int id)
        {
            if (id <= 0) return BadRequest();

            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null) return NotFound();

            return Ok(new RegistroAsistenciaDto
            {
                EstaPresente = buscar.EstaPresente,
                Id = buscar.Id,
                EstudianteId = buscar.EstudianteId,
                Fecha = buscar.Fecha,
                Observaciones = buscar.Observaciones
            });
        }

        [HttpPost]
        public async Task<ActionResult<RegistroAsistenciaDto>> Create([FromBody] RegistroAsistenciaDto dto)
        {
            var registro = new RegistroAsistencium
            {
                EstaPresente = dto.EstaPresente,
                EstudianteId = dto.EstudianteId,
                Fecha = dto.Fecha,
                Observaciones = dto.Observaciones
            };

            var errores = Validar(registro);
            if (errores.Count > 0) return BadRequest(errores);

            var existeEstudiante = await _dbcontext.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
            if (!existeEstudiante)
                return BadRequest($"El estudiante con ID {dto.EstudianteId} no existe en la base de datos");

            _dbcontext.RegistroAsistencia.Add(registro);
            await _dbcontext.SaveChangesAsync();

            dto.Id = registro.Id;
            return CreatedAtAction(nameof(GetById), new { id = registro.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroAsistenciaDto dto)
        {
            if (id <= 0) return BadRequest("El id no puede ser 0");

            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null) return NotFound("No existe el registro, imposible actualizar");

            var existeEstudiante = await _dbcontext.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
            if (!existeEstudiante)
                return BadRequest($"El estudiante con ID {dto.EstudianteId} no existe en la base de datos");

            buscar.Observaciones = dto.Observaciones;
            buscar.Fecha = dto.Fecha;
            buscar.EstaPresente = dto.EstaPresente;
            buscar.EstudianteId = dto.EstudianteId;

            var errores = Validar(buscar);
            if (errores.Count > 0) return BadRequest(errores);

            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("El id no puede ser 0");

            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null) return NotFound("No se encontro la asistencia");

            _dbcontext.RegistroAsistencia.Remove(buscar);
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<RegistroAsistenciaDto>>> Search([FromQuery] string? nombre, [FromQuery] PaginatedRequest paginated)
        {
            var query = await _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .Include(r => r.Estudiante)
                    .ThenInclude(e => e.Aula)
                .Where(r => string.IsNullOrEmpty(nombre) || r.Estudiante.Nombre.Contains(nombre))
                .OrderByDescending(r => r.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultDto = query.Select(r => new RegistroAsistenciaDto
            {
                Id = r.Id,
                EstaPresente = r.EstaPresente,
                EstudianteId = r.EstudianteId,
                Fecha = r.Fecha,
                Observaciones = r.Observaciones,
                NombreCompletoEstudiante = r.Estudiante.Nombre,
                NombreAula = r.Estudiante.Aula?.Nombre ?? "SIN AULA"
            }).ToList();

            return Ok(resultDto);
        }


        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateBulk([FromBody] List<RegistroAsistencium> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Lista vacía",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "No se enviaron registros de asistencia para guardar."
                });
            }

            var entidades = dtoList.Select(dto => new RegistroAsistencium
            {
                Fecha = dto.Fecha,
                EstudianteId = dto.EstudianteId,
                EstaPresente = dto.EstaPresente,
                Observaciones = dto.Observaciones
            }).ToList();

            await _dbcontext.RegistroAsistencia.AddRangeAsync(entidades);
            await _dbcontext.SaveChangesAsync();

            return Ok(new { Message = $"Se registraron {entidades.Count} asistencias correctamente." });
        }
    

[HttpGet("resumen")]
        public async Task<ActionResult<List<RegistroAsistenciaResumenDto>>> GetResumen(
      [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] PaginatedRequest paginated)
        {
            var registros = _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .Include(r => r.Estudiante)
                    .ThenInclude(e => e.Aula)
                .AsQueryable();

            if (desde.HasValue)
            {
                registros = registros.Where(r => r.Fecha >= desde.Value.Date);
            }
            if (hasta.HasValue)
            {
                registros = registros.Where(r => r.Fecha < hasta.Value.Date.AddDays(1));
            }

            var lista = await registros.OrderByDescending(r => r.Fecha).ToListAsync();

            var resultDto = lista
                .GroupBy(r => new { Fecha = r.Fecha.Date, r.Estudiante.AulaId })
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .Select(g => new RegistroAsistenciaResumenDto
                {
                    Fecha = g.Key.Fecha,
                    AulaId = g.Key.AulaId,
                    NombreAula = g.First().Estudiante.Aula != null ? g.First().Estudiante.Aula.Nombre : "SIN AULA",
                    TotalAlumnos = g.Count(),
                    TotalPresentes = g.Count(r => r.EstaPresente),
                    TotalAusentes = g.Count(r => !r.EstaPresente),
                    Presentes = g.Where(r => r.EstaPresente).Select(r => new AlumnoAsistenciaDto
                    {
                        EstudianteId = r.EstudianteId,
                        NombreCompleto = r.Estudiante.Nombre + " " + r.Estudiante.Apellido,
                        AvatarImg = r.Estudiante.AvatarUrlPictogramaPath,
                        Observaciones = r.Observaciones
                    }).ToList(),
                    Ausentes = g.Where(r => !r.EstaPresente).Select(r => new AlumnoAsistenciaDto
                    {
                        EstudianteId = r.EstudianteId,
                        NombreCompleto = r.Estudiante.Nombre + " " + r.Estudiante.Apellido,
                        AvatarImg = r.Estudiante.AvatarUrlPictogramaPath,
                        Observaciones = r.Observaciones
                    }).ToList()
                }).ToList();

            return Ok(resultDto);
        }


        [HttpGet("alumnos-aula")]
        public async Task<ActionResult<List<RegistroAsistenciaDto>>> GetAlumnosAula([FromQuery] int aulaId, [FromQuery] DateTime fecha)
        {
            var alumnos = await _dbcontext.Estudiantes
                .AsNoTracking()
                .Where(e => e.AulaId == aulaId && e.EstadoActivo)
                .ToListAsync();

            var registros = await _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .Where(r => r.Fecha.Date == fecha.Date)
                .ToListAsync();

            var resultDto = alumnos.Select(a =>
            {
                var registro = registros.FirstOrDefault(r => r.EstudianteId == a.Id);
                return new RegistroAsistenciaDto
                {
                    Id = registro != null ? registro.Id : 0,
                    Fecha = fecha.Date,
                    EstudianteId = a.Id,
                    NombreCompletoEstudiante = a.Nombre + " " + a.Apellido,
                    AvatarImg = a.AvatarUrlPictogramaPath,
                    EstaPresente = registro != null ? registro.EstaPresente : true,
                    Observaciones = registro?.Observaciones
                };
            }).ToList();

            return Ok(resultDto);
        }


        [HttpDelete("grupo")]
        public async Task<ActionResult> DeleteGrupo([FromQuery] DateTime fecha, [FromQuery] int aulaId)
        {
            var registros = await _dbcontext.RegistroAsistencia
                .Where(r => r.Fecha.Date == fecha.Date && r.Estudiante.AulaId == aulaId)
                .ToListAsync();

            if (!registros.Any())
            {
                return NotFound();
            }

            _dbcontext.RegistroAsistencia.RemoveRange(registros);
            await _dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}