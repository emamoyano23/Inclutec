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
        private readonly IWebHostEnvironment _env;

        public EstudianteController(InclutecbdContext dbcontext, IWebHostEnvironment env)
        {
            _dbcontext = dbcontext;
            _env = env;
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

        //En este endpoint se sube la imagen del avatar y se guarda en una carpeta privada fuera de wwwroot,
        //y se devuelve el nombre de archivo que se guardará en la base de datos.
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ProblemDetails { Detail = "No se envió ningún archivo." });

            if (file.Length > 2097152) // maximo tamaño permitido 2 MB
                return BadRequest(new ProblemDetails { Detail = "La imagen excede el límite de 2 MB." });

            string ext = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" }; //formatos permitidos
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new ProblemDetails { Detail = "Formato de imagen no permitido." });

            string folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Avatares"); //creacion de la carpeta fuera de wwwroot
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName });
        }

        //en este endpoint se obtiene la imagen del avatar desde la carpeta privada fuera de wwwroot,
        //y si no existe se devuelve una imagen por defecto.
        [HttpGet("avatar/{fileName}")]
        public IActionResult GetAvatar(string fileName)
        {
            string fullPath = Path.Combine(_env.ContentRootPath, "App_Data", "Avatares", fileName);

            if (!System.IO.File.Exists(fullPath))
            {
                // Avatar por defecto si no existe el archivo
                string defaultPath = Path.Combine(_env.ContentRootPath, "wwwroot", "img", "user.png");
                if (System.IO.File.Exists(defaultPath))
                    return PhysicalFile(defaultPath, "image/png");

                return NotFound();
            }

            string ext = Path.GetExtension(fullPath).ToLower();
            string contentType = ext switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            return PhysicalFile(fullPath, contentType);
        }
        [HttpGet("aula/{aulaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EstudianteDto>))]
        public async Task<ActionResult<List<EstudianteDto>>> GetByAula(int aulaId)
        {
            var estudiantes = await _dbcontext.Estudiantes
                .AsNoTracking()
                .Include(a => a.Aula)
                .Where(e => e.AulaId == aulaId && e.EstadoActivo)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            var resultDto = estudiantes.Select(e => new EstudianteDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                AvatarUrlPictogramaPath = e.AvatarUrlPictogramaPath,
                AulaId = e.AulaId,
                EstadoActivo = e.EstadoActivo,
                NombreAula = e.Aula != null ? e.Aula.Nombre : string.Empty
            }).ToList();

            return Ok(resultDto);
        }
    }
}
