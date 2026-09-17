using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    internal class AulaDto
    {
        public int Id {  get; set; }

        public string? Nombre { get; set; }

        public DateOnly AñoLectivo { get; set; }

        public int ResponsableId { get; set; }

        public bool EstadoActivo { get; set; }

        
    }
}
