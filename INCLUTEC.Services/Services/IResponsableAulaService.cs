using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Services.Services
{
    internal interface IResponsableAulaService
    {
        Task<HttpResponseWrapper<object>> CreateAsync(ResponsableAulaDto responsableAulaDto);
        Task<HttpResponseWrapper<string>> DeleteAsync(int id);
        Task<HttpResponseWrapper<ResponsableAulaDto>> GetID(int id);
        Task<HttpResponseWrapper<List<ResponsableAulaDto>>> GetListAsync(string url);
        Task<HttpResponseWrapper<List<ResponsableAulaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated);
        Task<HttpResponseWrapper<object>> Update(ResponsableAulaDto dto);
    }
}
