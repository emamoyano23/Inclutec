using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Services.Services
{
    public interface IEstudianteService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(EstudianteDto dto);
        Task<HttpResponseWrapper<string>> DeleteAsync(int id);
        Task<HttpResponseWrapper<EstudianteDto>> GetID(int id);
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetListAsync(string url);
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> Update(EstudianteDto dto);
    }
}
