using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System.Net.Http.Json;

namespace INCLUTEC.Services.Services
{
    public class AulaService : IAulaService
    {
        private const string url = "api/Aula";
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
                baseUrl = $"{url}/search?nombre={name}&PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }

            return await GetListAsync(baseUrl);
        }

        public async Task<HttpResponseWrapper<List<AulaDto>>> GetListAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await BuildResponseAsync<List<AulaDto>>(response);
        }

        public async Task<HttpResponseWrapper<AulaDto>> GetID(int id)
        {
            var response = await _httpClient.GetAsync($"{url}/{id}");
            return await BuildResponseAsync<AulaDto>(response);
        }

        public async Task<HttpResponseWrapper<object>> CreateAsync(AulaDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(url, dto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<object>> Update(AulaDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{url}/{dto.Id}", dto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<string>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{id}");
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