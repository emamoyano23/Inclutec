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
