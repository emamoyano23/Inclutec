using System;
using System.Collections.Generic;

namespace INCLUTEC.Entities.Models;

public partial class RegistroAsistencium
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public int EstudianteId { get; set; }

    public bool EstaPresente { get; set; }

    public string? Observaciones { get; set; }

    public virtual Estudiante Estudiante { get; set; } = null!;
}
