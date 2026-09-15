using System;
using System.Collections.Generic;

namespace INCLUTEC.Entities.Models;

public partial class Estudiante
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string AvatarUrlPictogramaPath { get; set; } = null!;

    public int AulaId { get; set; }

    public bool EstadoActivo { get; set; }

    public virtual Aula IdNavigation { get; set; } = null!;

    public virtual ICollection<RegistroAsistencium> RegistroAsistencia { get; set; } = new List<RegistroAsistencium>();
}
