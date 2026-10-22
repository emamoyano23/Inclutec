using Microsoft.AspNetCore.Mvc.Rendering;

namespace INCLUTEC.Panel.Helpers
{
    public interface IDropdownService
    {
        Task<IEnumerable<SelectListItem>> GetAulasAsync();
        Task<IEnumerable<SelectListItem>> GetResponsableAulaAsync();
    }
}