using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INCLUTEC.Entities.Models
{
    public partial class RegistroAsistencium
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El estudiante es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante válido")]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "El estado de asistencia es obligatorio")]
        public bool EstaPresente { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres")]
        public string? Observaciones { get; set; }

        public virtual Estudiante Estudiante { get; set; } = null!;
    }
}