using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System.Net.Http.Json;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Services.Services
{
    public class EstudianteService : IEstudianteService
    {
        private const string url = "api/v1/Estudiante";
        private readonly HttpClient _httpClient;

        public EstudianteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseWrapper<List<EstudianteDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated)
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

        public async Task<HttpResponseWrapper<List<EstudianteDto>>> GetListAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
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

        public async Task<HttpResponseWrapper<object>> UpdateAsync(EstudianteDto estudianteDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{url}/{estudianteDto.Id}", estudianteDto);
            return await BuildResponseAsync<object>(response);
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
