using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System.Net.Http.Json;

namespace INCLUTEC.Panel.Infrastructure
{
    public class RegistroAsistenciaService : IRegistroAsistenciaService
    {
      
        private const string url = "api/RegistroAsistencia";
        private readonly HttpClient _httpclient;
        public RegistroAsistenciaService(HttpClient http)
        {
               _httpclient = http;
        }
    
        public async Task<HttpResponseWrapper<List<RegistroAsistenciaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated)
        {
            string baseUrl;
            if (string.IsNullOrWhiteSpace(name))
            {
                baseUrl = $"{url}?PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }
            else
            {
                baseUrl = $"{url}/search?nombre={name}&PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }
      return await GetListAsync(baseUrl);
        }
        public async Task<HttpResponseWrapper<List<RegistroAsistenciaDto>>> GetListAsync(string url)
        {
            var response = await _httpclient.GetAsync(url);
            return await BuildResponseAsync<List<RegistroAsistenciaDto>>(response);
        }
        public async Task<HttpResponseWrapper<object>> Update(RegistroAsistenciaDto dto)
        {
            var response = await _httpclient.PutAsJsonAsync($"{url}/{dto.Id}", dto);
            return await BuildResponseAsync<object>(response);
        }
        public async Task<HttpResponseWrapper<RegistroAsistenciaDto>> GetID(int id)
        {
            var response = await _httpclient.GetAsync($"{url}/{id}");
            return await BuildResponseAsync<RegistroAsistenciaDto>(response);
        }

        public async Task<HttpResponseWrapper<object>> CreateAsync(RegistroAsistenciaDto dto)
        {
            var response = await _httpclient.PostAsJsonAsync(url, dto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<string>> DeleteAsync(int id)
        {
            var response = await _httpclient.DeleteAsync($"{url}/{id}");
            return await BuildResponseAsync<string>(response);
        }

        private async Task<HttpResponseWrapper<T>> BuildResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentLength == 0)
                {
                    return new HttpResponseWrapper<T>(default, response);
                }

                var data = await response.Content.ReadFromJsonAsync<T>();

                return new HttpResponseWrapper<T>(data, response);
            }

            return new HttpResponseWrapper<T>(
                default,
                response,
                await response.Content.ReadAsStringAsync());
        }

    }
}
