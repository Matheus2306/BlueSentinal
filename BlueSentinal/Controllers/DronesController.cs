using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSentinal.Data;
using BlueSentinal.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlueSentinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {
        private readonly APIContext _context;

        public DronesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Drones
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            return await _context.Drones
                .Include(d => d.DroneFabri)
                .Include(d => d.Usuario)
                .ToListAsync();
        }

        [Authorize]
        // GET: api/Drones/getDroneUser
        [HttpGet("getDroneUser")]
        public async Task<ActionResult<IEnumerable<Drone>>> GetUserDrone()
        {
            // Coleta o id do usuário logado pelo bearer
            var userBearer = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userBearer))
                return BadRequest("Usuário sem login");


            var userId = new Guid(userBearer);

            // Busca todos os drones do usuário logado, incluindo dados relacionados
            var drones = await _context.Drones
                .Where(d => d.UsuarioId == userId)
                .Include(d => d.DroneFabri)
                .Include(d => d.Usuario)
                .ToListAsync();



            return Ok(drones);
        }



        // PUT: api/Drones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        // POST: api/Drones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        // DELETE: api/Drones/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrone(Guid id)
        {
            var drone = await _context.Drones.FindAsync(id);
            if (drone == null)
            {
                return NotFound();
            }
            var dronefabri = await _context.DroneFabris.FindAsync(drone.DroneFabriId);
            dronefabri.Status = false;


            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DroneExists(Guid id)
        {
            return _context.Drones.Any(e => e.DroneId == id);
        }
        // POST: api/Drones/vincular
        [Authorize]
        [HttpPost("vincular")]
        public async Task<IActionResult> VincularDrone(string mac, Drone drone)
        {
            var droneFabri = await _context.DroneFabris.FirstOrDefaultAsync(df => df.Mac == mac);
            // Busca o DroneFabri pelo MAC
            if (droneFabri == null)
                return NotFound("DroneFabri não encontrado");

            //coleta o token do usuário logado
            var userBearer = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userBearer))
                return BadRequest("Usuário sem login");

            //verifica se o drone já está vinculado ao usuário
            var existingDrone = await _context.Drones
                .FirstOrDefaultAsync(d => d.DroneFabriId == droneFabri.DroneFabriId && d.UsuarioId == drone.UsuarioId);
            if (existingDrone != null)
                return BadRequest("Este drone já está vinculado ao usuário.");

            drone.DroneFabri = droneFabri;
            drone.DroneFabri.Status = true;

            //adiciona o ID do usuario logado ao drone
            drone.UsuarioId = new Guid(userBearer);

            //adiciona o usuário ao drone pelo Id do bearer
            drone.Usuario = await _context.Users.FindAsync(userBearer);


            _context.Drones.Add(drone);
            await _context.SaveChangesAsync();

            return Ok(drone);
        }
        // PUT: api/Drones/tempo/{id}
        [HttpPut("tempo/{id}")]
        public async Task<IActionResult> AtualizarTempo(Guid id, [FromBody] AtualizarTempoStatusDto dto)
        {
            var drone = await _context.Drones.FindAsync(id);
            if (drone == null)
                return NotFound("Drone não encontrado.");

            drone.TempoEmMili = dto.Tempo;
            drone.tempoEmHoras = Math.Round((decimal)(dto.Tempo / 1000.0 / 60 / 60), 2);
            drone.Status = dto.Status;

            _context.Entry(drone).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(drone);
        }

        [Authorize(Roles = "Admin")]
        // GET: api/Drones/getByUserName/{userName}
        [HttpGet("getByUserName/{userName}")]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDronesByUserName(string userName)
        {
            // Verifica se o nome do usuário foi fornecido
            if (string.IsNullOrEmpty(userName))
                return BadRequest("Nome do usuário não fornecido.");

            // Busca o usuário pelo nome
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            // Busca os drones associados ao usuário
            var drones = await _context.Drones
                .Where(d => d.UsuarioId == new Guid(user.Id))
                .Include(d => d.DroneFabri)
                .Include(d => d.Usuario)
                .ToListAsync();

            return Ok(drones);
        }
    }
    public class AtualizarTempoStatusDto
    {
        public long Tempo { get; set; }
        public bool Status { get; set; }
    }
}
