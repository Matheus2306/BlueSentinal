using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSentinal.Data;
using BlueSentinal.Models;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            return await _context.Drones
                .Include(d => d.DroneFabri)
                .Include(d => d.Usuario)
                .ToListAsync();
        }

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

        // GET: api/Drones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Drone>> GetDrone(Guid id)
        {
            var drone = await _context.Drones.FindAsync(id);

            if (drone == null)
            {
                return NotFound();
            }

            return drone;
        }

        // PUT: api/Drones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDrone(Guid id, Drone drone)
        {
            if (id != drone.DroneId)
            {
                return BadRequest();
            }

            _context.Entry(drone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DroneExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Drones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        // DELETE: api/Drones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrone(Guid id)
        {
            var drone = await _context.Drones.FindAsync(id);
            if (drone == null)
            {
                return NotFound();
            }

            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DroneExists(Guid id)
        {
            return _context.Drones.Any(e => e.DroneId == id);
        }
        // POST: api/Drones/vincular
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
    }
}
