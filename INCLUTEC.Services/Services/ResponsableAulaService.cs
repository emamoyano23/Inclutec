using INCLUTEC.Entities.Dtos;
using INCLUTEC.Panel.Infrastructure.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace INCLUTEC.Services.Services
{

    internal class ResponsableAulaService : IResponsableAulaService
    {

        private const string url = "api/ResponsableAula";
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
            var response = await _httpclient.GetAsync($"{url}/{id}");
            return await BuildResponseAsync<ResponsableAulaDto>(response);
        }

        public async Task<HttpResponseWrapper<List<ResponsableAulaDto>>> GetListAsync(string url)
        {

            var response = await _httpclient.GetAsync(url);
            return await BuildResponseAsync<List<ResponsableAulaDto>>(response);
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
                baseUrl = $"{url}/search?nombre={name}&PageNumber={paginated.PageNumber}&PageSize={paginated.PageSize}";
            }
            return await GetListAsync(baseUrl);
        }

        public async Task<HttpResponseWrapper<object>> Update(ResponsableAulaDto dto)
        {
            var response = await _httpclient.PutAsJsonAsync($"{url}/{dto.Id}", dto);
            return await BuildResponseAsync<object>(response);
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
