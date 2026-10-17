using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class AlumnoAsistenciaDto
    {
        public int EstudianteId { get; set; }
        public string? NombreCompleto { get; set; } = "";
        public string? AvatarImg { get; set; }
        public string? Observaciones { get; set; }
    }
}
