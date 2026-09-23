using INCLUTEC.Api.Infrastructure.Data;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INCLUTEC.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ResponsableAulaController : Controller
    {
        private readonly InclutecbdContext _dbcontext;


        public ResponsableAulaController(InclutecbdContext dbcontext)
        {
            _dbcontext = dbcontext;

        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<ResponsableAulaDto>>> Get([FromQuery] PaginatedRequest paginated)
        {
            var obtener = await _dbcontext.ResponsableAulas
                .AsNoTracking()
                .OrderByDescending(r => r.Id)
                   .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultadoDto = obtener.Select(r => new ResponsableAulaDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Apellido = r.Apellido,
                Rol = r.Rol,
                Email = r.Email,
            }).ToList();
            return Ok(resultadoDto);
        }


        // Busco profe por su nombre
        [HttpGet()]
        [Route("search")]
        public async Task<ActionResult<List<ResponsableAulaDto>>> Search([FromQuery] string? nombre, [FromQuery] PaginatedRequest paginated)
        {
            var query = await _dbcontext.ResponsableAulas
                .AsNoTracking()
                .Where(r => string.IsNullOrEmpty(nombre) || r.Nombre.Contains(nombre))  
                .OrderByDescending(r => r.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            var resultDto = query.Select(r => new ResponsableAulaDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Apellido = r.Apellido,
                Rol = r.Rol,
                Email = r.Email,

            }).ToList();


            return Ok(resultDto);
        }


        // Busco profe por id 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponsableAulaDto>> GetById(int id)
        {
            
            var query = await _dbcontext.ResponsableAulas
                .FirstOrDefaultAsync(r => r.Id == id);

            if (query == null)
            {
                return NotFound($"El profesor/responsable {id} no existe");
            }

            var resultDto = new ResponsableAulaDto
            {
                Id = query.Id,
                Nombre = query.Nombre,
                Apellido = query.Apellido,
                Rol = query.Rol,
                Email = query.Email,
            };
            return Ok(resultDto);

        }


        // creamos un nuevo profe o repssable (revisar!!)
        [HttpPost()]
        [Route("")]
        public async Task<ActionResult<ResponsableAulaDto>> Create([FromBody] ResponsableAulaDto responsableAulaDto)
        {
            var query = await _dbcontext.ResponsableAulas
                .FirstOrDefaultAsync(r => r.Nombre == responsableAulaDto.Nombre);

            if (query != null)
            {
                return BadRequest($"Ya existe un profe con ese nombre!");
            }

            var ResponsableAula = new ResponsableAula
            {

                Nombre = responsableAulaDto.Nombre,
                Apellido = responsableAulaDto.Apellido,
                Rol = responsableAulaDto.Rol,
                Email = responsableAulaDto.Email,

            };

            _dbcontext.ResponsableAulas.Add(ResponsableAula);

            await _dbcontext.SaveChangesAsync();

            return Ok(ResponsableAula);
        }


        // Editamos o actualizamos un profe
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<ResponsableAulaDto>> Update(int id, [FromBody] ResponsableAulaDto responsableAulaDto)
        {
            var query = await _dbcontext.ResponsableAulas
                .FirstOrDefaultAsync(r => r.Id == id);

            if (query == null)
            {
                return NotFound($"El profesor {id} no se encontro");
            }

            query.Nombre = responsableAulaDto.Nombre;
            query.Apellido = responsableAulaDto.Apellido;
            query.Rol = responsableAulaDto.Rol;
            query.Email = responsableAulaDto.Email;



            await _dbcontext.SaveChangesAsync();

            return Ok();
        }


        // borro un profesor  
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var query = await _dbcontext.ResponsableAulas
                .FirstOrDefaultAsync(r => r.Id == id);


            if (query == null)
            {
                return NotFound($"El prfe {id} no existe");
            }

            _dbcontext.Remove(query);
            await _dbcontext.SaveChangesAsync();

            return Ok();

        }



    }
}
