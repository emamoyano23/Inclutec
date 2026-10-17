using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;

namespace INCLUTEC.Panel.Infrastructure
{
    public interface IEstudianteService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(EstudianteDto estudianteDto);
        Task<HttpResponseWrapper<string>> DeleteAsync(int id);
        Task<HttpResponseMessage> GetAvatarAsync(string fileName);
        Task<HttpResponseWrapper<EstudianteDto>> GetByIdAsync(int id);
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetListAsync();
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetPaginatedAsync(string? nombre, int? aulaId, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> UpdateAsync(int id, EstudianteDto estudianteDto);
        Task<HttpResponseWrapper<AvatarSubidoDto>> UploadAvatarAsync(Stream contenido, string nombreArchivo, string contentType);
    }
}