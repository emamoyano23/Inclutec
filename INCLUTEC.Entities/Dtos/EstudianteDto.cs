using INCLUTEC.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    internal class EstudianteDto
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? AvatarUrlPictogramaPath { get; set; }

        public int AulaId { get; set; }

        public string? NombreAula { get; set; }


        public bool EstadoActivo { get; set; }


    }
}
