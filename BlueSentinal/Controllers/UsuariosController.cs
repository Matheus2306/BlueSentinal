using BlueSentinal.Data;
using BlueSentinal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueSentinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        //dependencias
        private readonly UserManager<Usuario> _userManager;
        private readonly APIContext _context;


        public UsuariosController(UserManager<Usuario> userManager, APIContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> createUser(Usuario usuario)
        {

            //coleta o token do usuário logado
            var userBearer = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userBearer))
                return BadRequest("Usuario sem bearer");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (existingUser != null)
                return BadRequest("Este usuário já está vinculado.");

            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Users.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
