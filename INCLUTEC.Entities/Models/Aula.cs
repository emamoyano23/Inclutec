using System;
using System.Collections.Generic;

namespace INCLUTEC.Entities.Models;

public partial class Aula
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string AñoLectivo { get; set; } =string.Empty;

    public int ResponsableId { get; set; }

    public bool EstadoActivo { get; set; }

    public virtual Estudiante? Estudiante { get; set; }

    public virtual ResponsableAula Responsable { get; set; } = null!;
}
