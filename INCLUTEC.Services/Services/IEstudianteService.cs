using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Services.Services
{
   public interface IEstudianteService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(EstudianteDto estudianteDto);
        Task<HttpResponseWrapper<EstudianteDto>> GetByIdAsync(int id);
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetListAsync(string url);
        Task<HttpResponseWrapper<List<EstudianteDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> UpdateAsync(EstudianteDto estudianteDto);
    }
}
