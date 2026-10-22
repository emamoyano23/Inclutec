using INCLUTEC.Services.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INCLUTEC.Panel.Helpers
{
    public class DropdownService : IDropdownService
    {
        private readonly IAulaService _aulaService;
        private readonly IResponsableAulaService _responsableAulaService;

        public DropdownService(IAulaService aulaService, IResponsableAulaService responsableAulaService)
        {
            _aulaService = aulaService;
            _responsableAulaService = responsableAulaService;
        }
        public async Task<IEnumerable<SelectListItem>> GetAulasAsync()
        {
            var apiResult = await _aulaService.GetListAsync("api/v1/aula");

            var defaultOption = new SelectListItem { Value = "", Text = "Seleccione una aula" };

            var result = apiResult?.Response?.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Nombre,
            }).ToList() ?? new List<SelectListItem>();

            result.Insert(0, defaultOption);
            return result;
        }
        public async Task<IEnumerable<SelectListItem>> GetResponsableAulaAsync()
        {
            var apiResult = await _responsableAulaService.GetListAsync("api/v1/ResponsableAula");
            var defaultOption = new SelectListItem { Value = "", Text = "Seleccione una responsable" };
            var result = apiResult?.Response?.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Nombre} {x.Apellido}",
            }).ToList() ?? new List<SelectListItem>();
            result.Insert(0, defaultOption);
            return result;
        }
    }
}

