using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class RegistroAsistenciaDto
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int EstudianteId { get; set; }

        public string? NombreCompletoEstudiante { get; set; }

        public bool EstaPresente { get; set; }

        public string? Observaciones { get; set; }
    }
}
