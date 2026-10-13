using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Helpers
{
    public static class AvatarHelper
    {
        // Los pictogramas subidos se guardan como nombre de archivo y los sirve el Panel.
        // Si el valor ya es una ruta o una direccion (datos viejos), se usa tal cual.
        public static string? Src(IUrlHelper url, string? avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar))
            {
                return null;
            }

            if (avatar.StartsWith("/") || avatar.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return avatar;
            }

            return url.Action("Avatar", "Estudiante", new { fileName = avatar });
        }
    }
}
