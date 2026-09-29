using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INCLUTEC.Entities.Models;

public partial class Estudiante
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El pictograma del avatar es obligatorio")]
    [StringLength(500, ErrorMessage = "La ruta del pictograma no puede superar los 500 caracteres")]
    public string AvatarUrlPictogramaPath { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un aula válida")]
    public int AulaId { get; set; }

    public bool EstadoActivo { get; set; }

    public virtual Aula IdNavigation { get; set; } = null!;

    public virtual ICollection<RegistroAsistencium> RegistroAsistencia { get; set; } = new List<RegistroAsistencium>();
}
