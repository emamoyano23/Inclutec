using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace INCLUTEC.Entities.Dtos
{
    public class ResponsableAulaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string? Nombre { get; set; }


        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
        public string? Apellido { get; set; }


        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El rol no puede superar los 50 caracteres")]
        public string? Rol {  get; set; }


        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato valido")]
        [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
        public string? Email { get; set; }

    }
}
