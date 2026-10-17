using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class RegistroAsistenciaResumenDto
    {
        public DateTime Fecha { get; set; }
        public int AulaId { get; set; }
        public string NombreAula { get; set; } = "SIN AULA";
        public int TotalAlumnos { get; set; }
        public int TotalPresentes { get; set; }
        public int TotalAusentes { get; set; }
        public List<AlumnoAsistenciaDto> Presentes { get; set; } = new();
        public List<AlumnoAsistenciaDto> Ausentes { get; set; } = new();
    }
}
