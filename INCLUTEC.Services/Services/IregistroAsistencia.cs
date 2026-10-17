using INCLUTEC.Entities.Dtos;
using INCLUTEC.Entities.Models;
using INCLUTEC.Panel.Infrastructure.Api.Services;

namespace INCLUTEC.Panel.Infrastructure
{
    public interface IRegistroAsistenciaService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(RegistroAsistenciaDto dto);
        Task<HttpResponseWrapper<string>> DeleteAsync(int id);
        Task<HttpResponseWrapper<RegistroAsistenciaDto>> GetID(int id);
        Task<HttpResponseWrapper<List<RegistroAsistenciaDto>>> GetListAsync(string url);
        Task<HttpResponseWrapper<List<RegistroAsistenciaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> Update(RegistroAsistenciaDto dto);
        Task<HttpResponseWrapper<List<RegistroAsistenciaResumenDto>>> GetResumenAsync(PaginatedRequest paginated);
        Task<HttpResponseWrapper<List<RegistroAsistenciaDto>>> GetAlumnosAulaAsync(int aulaId, DateTime fecha);
        Task<HttpResponseWrapper<object>> CreateBulkAsync(List<RegistroAsistenciaDto> lista);
        Task<HttpResponseWrapper<string>> DeleteGrupoAsync(DateTime fecha, int aulaId);
    }
}