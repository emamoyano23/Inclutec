using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class AulaDto
    {
        public int Id {  get; set; }

        public string? Nombre { get; set; }

        public string AñoLectivo { get; set; } = string.Empty;

        public int ResponsableId { get; set; }

        public bool EstadoActivo { get; set; }

        public string? ResponsableNombre { get; set; }


    }
}
