using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlueSentinal.Models
{
    public class Usuario : IdentityUser
    {

        [MinLength(2, ErrorMessage = "Nome deve ter no mínimo 2 caracteres"), MaxLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string? Nome { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime? Nascimento { get; set; }
    }
}
