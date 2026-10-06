using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INCLUTEC.Entities.Models;

public partial class ResponsableAula
{
    public int Id { get; set; }

    
    public string Nombre { get; set; } = string.Empty;

    
    public string Apellido { get; set; } = string.Empty;

    
    public string Rol { get; set; } = string.Empty;
    
    public string? Email { get; set; }

    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();
}
