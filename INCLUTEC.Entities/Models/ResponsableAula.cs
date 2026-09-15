using System;
using System.Collections.Generic;

namespace INCLUTEC.Entities.Models;

public partial class ResponsableAula
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();
}
