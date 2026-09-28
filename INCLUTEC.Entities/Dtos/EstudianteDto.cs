using System.ComponentModel.DataAnnotations;

namespace INCLUTEC.Entities.Dtos
{
    public class EstudianteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estudiante es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del estudiante es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre {2} y {1} caracteres.")]
        public string? Apellido { get; set; }

        [StringLength(1000, ErrorMessage = "La ruta del avatar o pictograma no puede superar los {1} caracteres.")]
        public string? AvatarUrlPictogramaPath { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el aula a la que pertenece el estudiante.")]
        public int AulaId { get; set; }

        public string? NombreAula { get; set; }

        public bool EstadoActivo { get; set; } = true;
    }
}
