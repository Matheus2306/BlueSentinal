using System.ComponentModel.DataAnnotations;

namespace BlueSentinal.Models
{
    public class DroneFabri
    {
        public Guid DroneFabriId { get; set; }
        [Required(ErrorMessage = "Modelo de drone é obrigatório")]
        public string? Modelo { get; set; }
        [Required(ErrorMessage = "MAC é obrigatório")]
        public string? Mac { get; set; }
        [Required(ErrorMessage = "Status é obrigatório")]
        public bool? Status { get; set; } = false;

    }
}
