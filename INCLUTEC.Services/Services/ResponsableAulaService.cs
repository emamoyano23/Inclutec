using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using System.Net;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace INCLUTEC.Services.Services
{

    public class ResponsableAulaService : IResponsableAulaService
    {

        private const string url = "api/v1/ResponsableAula";
        private readonly HttpClient _httpclient;
        public ResponsableAulaService(HttpClient http)
        {
            _httpclient = http;
        }

        public async Task<HttpResponseWrapper<object>> CreateAsync(ResponsableAulaDto responsableAulaDto)
        {
            var response = await _httpclient.PostAsJsonAsync(url, responsableAulaDto);
            return await BuildResponseAsync<object>(response);
        }

        public async Task<HttpResponseWrapper<string>> DeleteAsync(int id)
        {
            var response = await _httpclient.DeleteAsync($"{url}/{id}");
            return await BuildResponseAsync<string>(response);
        }

        public async Task<HttpResponseWrapper<ResponsableAulaDto>> GetID(int id)
        {
            return await GetResponseAsync<ResponsableAulaDto>($"{url}/{id}");
        }

        public async Task<HttpResponseWrapper<List<ResponsableAulaDto>>> GetListAsync(string url)
        {

            return await GetResponseAsync<List<ResponsableAulaDto>>(url);
        }

        public async Task<HttpResponseWrapper<List<ResponsableAulaDto>>> GetPaginatedAsync(string? name, PaginatedRequest paginated)
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

        public async Task<HttpResponseWrapper<object>> Update(ResponsableAulaDto dto)
        {
            var response = await _httpclient.PutAsJsonAsync($"{url}/{dto.Id}", dto);
            return await BuildResponseAsync<object>(response);
        }

        private async Task<HttpResponseWrapper<T>> GetResponseAsync<T>(string requestUrl)
        {
            try
            {
                var response = await _httpclient.GetAsync(requestUrl);
                return await BuildResponseAsync<T>(response);
            }
            catch (HttpRequestException)
            {
                return new HttpResponseWrapper<T>(default, new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
                    "No se pudieron cargar los responsables. Verificá que la API esté iniciada.")
                { Error = true };
            }
            catch (TaskCanceledException)
            {
                return new HttpResponseWrapper<T>(default, new HttpResponseMessage(HttpStatusCode.GatewayTimeout),
                    "La API demoró demasiado al cargar los responsables.")
                { Error = true };
            }
        }

        private async Task<HttpResponseWrapper<T>> BuildResponseAsync<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResponseWrapper<T>(default, response, body) { Error = true };
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return new HttpResponseWrapper<T>(default, response) { Error = false };
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return new HttpResponseWrapper<T>(data, response) { Error = false };
            }
            catch (JsonException)
            {
                return new HttpResponseWrapper<T>(default, response,
                    "La respuesta de responsables no se pudo interpretar.")
                { Error = true };
            }
        }
    }
}