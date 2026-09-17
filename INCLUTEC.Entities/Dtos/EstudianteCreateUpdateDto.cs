using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    internal class EstudianteCreateUpdateDto
    {
        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? AvatarUrlPictogramPath { get; set; }

        public int AulaId { get; set; }

        public bool EstadoActivo { get; set; }

    }
}
