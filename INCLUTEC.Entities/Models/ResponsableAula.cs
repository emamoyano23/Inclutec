using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INCLUTEC.Entities.Models;

public partial class ResponsableAula
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El rol no puede superar los 50 caracteres")]
    public string Rol { get; set; } = string.Empty;
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato valido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    public string? Email { get; set; }

    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();
}
