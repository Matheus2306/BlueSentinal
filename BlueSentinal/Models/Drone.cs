using System.ComponentModel.DataAnnotations;

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


    }
}
