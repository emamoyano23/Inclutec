using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    internal class ResponsableAulaCreateUpdateDto
    {
        public string? Nombre { get; set; }

        public string?  Apellido { get; set; }

        public string? Rol {  get; set; }

        public string? Email { get; set; }

    }
}
