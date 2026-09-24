namespace INCLUTEC.Panel.Infrastructure.Api.Services
{
    public class HttpResponseWrapper<T>
    {
        public bool Error { get; set; }
        public T? Response { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public HttpResponseWrapper(T? response, HttpResponseMessage httpres, string? errorMessage = null)
        {
            Response = response;
            HttpResponseMessage = httpres;
            ErrorMessage = errorMessage;
        }
        public async Task<string> GetErrorMessage()
        {
            if (!Error) return string.Empty;
            var statusCode = HttpResponseMessage.StatusCode;
            if (statusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "Recurso no encontrado";
            }
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
                return "Acceso no autorizado";
            if (statusCode == System.Net.HttpStatusCode.OK)
                return "ok";

            return "Ha ocurrido un error inesperado";
        }
    }
}
