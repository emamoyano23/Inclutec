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
        public async Task<ActionResult<RegistroAsistenciaDto>> GetById(int id)
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
            if (errores.Count > 0)
            {
                return BadRequest(errores);
            }

            var existeEstudiante = await _dbcontext.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
            if (!existeEstudiante)
            {
                return BadRequest($"El estudiante con ID {dto.EstudianteId} no existe en la base de datos");
            }

            _dbcontext.RegistroAsistencia.Add(registro);

            await _dbcontext.SaveChangesAsync();

            dto.Id = registro.Id;
            return CreatedAtAction(nameof(GetById), new { id = registro.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] RegistroAsistenciaDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("El id no puede ser 0");
            }
            var buscar = await _dbcontext.RegistroAsistencia.FirstOrDefaultAsync(c => c.Id == id);
            if (buscar is null)
            {
                return NotFound("No existe el registro, imposible actualizar");
            }
            buscar.Observaciones = dto.Observaciones;
            buscar.Fecha = dto.Fecha;
            buscar.EstaPresente = dto.EstaPresente;
            buscar.EstudianteId = dto.EstudianteId;

            var errores = Validar(buscar);
            if (errores.Count > 0)
            {
                return BadRequest(errores);
            }

            var existeEstudiante = await _dbcontext.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
            if (!existeEstudiante)
            {
                return BadRequest($"El estudiante con ID {dto.EstudianteId} no existe en la base de datos");
            }

            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
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

        [HttpGet("search")]
        public async Task<ActionResult<List<RegistroAsistenciaDto>>> Search([FromQuery] string? nombre, [FromQuery] PaginatedRequest paginated)
        {
            var query = await _dbcontext.RegistroAsistencia
                .AsNoTracking()
                .Include(r => r.Estudiante)
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
                NombreCompletoEstudiante = r.Estudiante.Nombre

            }).ToList();

            return Ok(resultDto);
        }
    }
}