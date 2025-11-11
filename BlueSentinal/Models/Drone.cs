using System.ComponentModel.DataAnnotations;

namespace BlueSentinal.Models
{
    public class Drone
    {
        public Guid DroneId { get; set; }
        [Required(ErrorMessage = "Modelo de drone é obrigatório")]

        public string? Modelo { get; set; }
        public string? Mac { get; set; }
        public bool? Status { get; set; }
    }
}
