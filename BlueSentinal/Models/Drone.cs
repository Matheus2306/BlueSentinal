using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace BlueSentinal.Models
{
    public class Drone
    {
        public Guid DroneId { get; set; }

        //chave estrangeira
        public Guid? DroneFabriId { get; set; }
        public DroneFabri? DroneFabri { get; set; }
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        [Required(ErrorMessage = "Localização é obrigatório")]
        [Display(Name = "Localização")]
        public string? Localizacao { get; set; }
        public long TempoAtividade { get; set; } = 0;

    }
}
