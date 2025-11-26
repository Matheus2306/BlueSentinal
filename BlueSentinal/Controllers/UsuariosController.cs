using BlueSentinal.Data;
using BlueSentinal.Models;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Roles = "Admin")]
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

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {
            // Coleta o ID do usuário logado a partir do token (bearer)
            var userBearer = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userBearer))
                return Unauthorized("Usuário não autenticado.");

            // Busca o usuário no banco de dados
            var usuario = await _userManager.FindByIdAsync(userBearer);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Remove o usuário
            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
            {
                var erros = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Erro ao excluir o usuário: {erros}");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adicionarRole")]
        public async Task<IActionResult> AdicionarRoleAoUsuario(string userId, string role)
        {
            var roleManager = HttpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
                return StatusCode(500, "RoleManager não disponível.");

            if (!await roleManager.RoleExistsAsync(role))
            {
                var resultRole = await roleManager.CreateAsync(new IdentityRole(role));
                if (!resultRole.Succeeded)
                    return BadRequest("Erro ao criar a role.");
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Verifica se o usuário já está na role
            if (await _userManager.IsInRoleAsync(usuario, role))
                return BadRequest($"Usuário já está na role '{role}'.");

            var result = await _userManager.AddToRoleAsync(usuario, role);
            if (!result.Succeeded)
            {
                var erros = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Erro ao adicionar a role ao usuário: {erros}");
            }

            return Ok($"Role '{role}' adicionada ao usuário '{usuario.UserName}'.");
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Usuario>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não autenticado.");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var roles = await _userManager.GetRolesAsync(usuario);


            return Ok(new
            {
                usuario.Id,
                usuario.UserName,
                usuario.Email,
                usuario.Nome,
                usuario.Nascimento,
                Roles = roles


            });
        }

    }
}
