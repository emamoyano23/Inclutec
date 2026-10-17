using Microsoft.AspNetCore.Mvc;

namespace INCLUTEC.Panel.Helpers
{
    public static class AvatarHelper
    {
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
