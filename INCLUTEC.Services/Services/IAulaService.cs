using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;

namespace INCLUTEC.Services.Services
{
    public interface IAulaService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(AulaDto aulaDto);
        Task<HttpResponseWrapper<string>> DeleteAsync(int id);
        Task<HttpResponseWrapper<AulaDto>> GetByIdAsync(int id);
        Task<HttpResponseWrapper<List<AulaDto>>> GetListAsync(string url);
        Task<HttpResponseWrapper<List<AulaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> UpdateAsync(AulaDto aulaDto);

        Task<HttpResponseWrapper<AulaDto>> GetID(int id);
        Task<HttpResponseWrapper<object>> Update(AulaDto aulaDto);
    }
}
