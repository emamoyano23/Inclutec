using System.Net.Http.Json;
using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;

namespace INCLUTEC.Panel.Infrastructure
{
    public class EstudianteService : IEstudianteService
    {
        private const string url = "api/v1/estudiantes";

        private readonly HttpClient _httpClient;

        public EstudianteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseWrapper<List<EstudianteDto>>> GetListAsync()
        {
            var response = await _httpClient.GetAsync(url);

            return await BuildResponseAsync<List<EstudianteDto>>(response);
        }

        public async Task<HttpResponseWrapper<List<EstudianteDto>>> GetPaginatedAsync(string? nombre, int? aulaId, PaginatedRequest paginated)
        {
            var requestUrl = $"{url}?PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";

            if (!string.IsNullOrWhiteSpace(nombre) || aulaId.HasValue)
            {
                requestUrl = $"{url}/search?PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    requestUrl += $"&nombre={Uri.EscapeDataString(nombre)}";
                }

                if (aulaId.HasValue)
                {
                    requestUrl += $"&aulaId={aulaId.Value}";
                }
            }

            var response = await _httpClient.GetAsync(requestUrl);

            return await BuildResponseAsync<List<EstudianteDto>>(response);
        }

        public async Task<HttpResponseWrapper<EstudianteDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{url}/{id}");

            return await BuildResponseAsync<EstudianteDto>(response);
        }

        public async Task<HttpResponseWrapper<object>> CreateAsync(EstudianteDto estudianteDto)
        {
            var response = await _httpClient.PostAsJsonAsync(url, estudianteDto);

            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<object>> UpdateAsync(int id, EstudianteDto estudianteDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{url}/{id}", estudianteDto);

            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<string>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{id}");

            return await BuildResponseAsync<string>(response);
        }

        private static async Task<HttpResponseWrapper<T>> BuildResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var mensaje = await response.Content.ReadAsStringAsync();

                return new HttpResponseWrapper<T>(default, response, mensaje) { Error = true };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                response.Content.Headers.ContentLength is null or 0)
            {
                return new HttpResponseWrapper<T>(default, response) { Error = false };
            }

            var data = await response.Content.ReadFromJsonAsync<T>();

            return new HttpResponseWrapper<T>(data, response) { Error = false };
        }
    }
}
