using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class ResponsableAulaDto
    {
        public int Id { get; set; }
      
        public string? Nombre { get; set; }
     
        public string? Apellido { get; set; }

        public string? Rol {  get; set; }

        public string? Email { get; set; }

    }
}
