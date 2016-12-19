using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaPOS.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }
    }
}