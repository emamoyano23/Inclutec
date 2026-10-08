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
        private readonly IWebHostEnvironment _env;

        public EstudianteController(InclutecbdContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
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
        [HttpGet("aula/{aulaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EstudianteDto>))]
        public async Task<ActionResult<List<EstudianteDto>>> GetByAula(int aulaId)
        {
            var estudiantes = await _dbContext.Estudiantes
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
