using Microsoft.AspNetCore.Identity;

namespace BlueSentinal.Models
{
    public class Usuario : IdentityUser
    {
        public Guid UsuarioId { get; set; }
        public Guid? UserId { get; set; }
        public IdentityUser? User { get; set; }
        public string Nome { get; set; }
        public string Nascimento { get; set; }
    }
}
