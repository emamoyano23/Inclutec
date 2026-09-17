using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    internal class RegistroAsistenciaCreateUpdateDto
    {
        public DateTime Fecha { get; set; }

        public int EstudianteId { get; set; }

        public bool EstaPresente { get; set; }

        public string? Observaciones { get; set; }

    }
}
