
namespace BlueSentinal.Models
{
    public class UsuarioRegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nome { get; set; }
        public DateTime Nascimento { get; set; }
    }
}