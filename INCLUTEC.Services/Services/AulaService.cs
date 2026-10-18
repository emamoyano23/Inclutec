using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace INCLUTEC.Services.Services
{
    public class AulaService : IAulaService
    {
        private const string url = "Aula";
        private readonly HttpClient _httpClient;

        public AulaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseWrapper<List<AulaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated)
        {
            string baseUrl;
            if (string.IsNullOrWhiteSpace(name))
            {
                baseUrl = $"{url}?PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }
            else
            {
                baseUrl = $"{url}/search?nombre={Uri.EscapeDataString(name.Trim())}&PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }

            return await GetListAsync(baseUrl);
        }

        public async Task<HttpResponseWrapper<List<AulaDto>>> GetListAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await BuildResponseAsync<List<AulaDto>>(response);
        }

        public async Task<HttpResponseWrapper<AulaDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{url}/{id}");
            return await BuildResponseAsync<AulaDto>(response);
        }

        public async Task<HttpResponseWrapper<object>> CreateAsync(AulaDto aulaDto)
        {
            var response = await _httpClient.PostAsJsonAsync(url, aulaDto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<object>> UpdateAsync(AulaDto aulaDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{url}/{aulaDto.Id}", aulaDto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<string>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{id}");
            return await BuildResponseAsync<string>(response);
        }

        public Task<HttpResponseWrapper<AulaDto>> GetID(int id)
        {
            return GetByIdAsync(id);
        }

        public Task<HttpResponseWrapper<object>> Update(AulaDto aulaDto)
        {
            return UpdateAsync(aulaDto);
        }

        private async Task<HttpResponseWrapper<T>> BuildResponseAsync<T>(HttpResponseMessage response)
        {
            var contenido = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResponseWrapper<T>(default, response, contenido) { Error = true };
            }

            
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return new HttpResponseWrapper<T>(default, response) { Error = false };
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(contenido, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return new HttpResponseWrapper<T>(data, response) { Error = false };
            }
            catch (JsonException)
            {
                return new HttpResponseWrapper<T>(default, response, "La respuesta de la API no tiene el formato esperado.") { Error = true };
            }
        }
    }
}
