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
    public class DroneFabrisController : ControllerBase
    {
        private readonly APIContext _context;

        public DroneFabrisController(APIContext context)
        {
            _context = context;
        }

        // GET: api/DroneFabris
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DroneFabri>>> GetDroneFabris()
        {
            return await _context.DroneFabris.ToListAsync();
        }

        // GET: api/DroneFabris/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DroneFabri>> GetDroneFabri(Guid id)
        {
            var droneFabri = await _context.DroneFabris.FindAsync(id);

            if (droneFabri == null)
            {
                return NotFound();
            }

            return droneFabri;
        }

        // PUT: api/DroneFabris/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDroneFabri(Guid id, DroneFabri droneFabri)
        {
            if (id != droneFabri.DroneFabriId)
            {
                return BadRequest();
            }

            _context.Entry(droneFabri).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DroneFabriExists(id))
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

        // POST: api/DroneFabris
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostDroneFabri([FromBody] DroneFabri droneFabri)
        {

            // verifica se o MAC escrito já existe
            var existingDrone = await _context.DroneFabris.FirstOrDefaultAsync(d => d.Mac == droneFabri.Mac);
            if (existingDrone != null)
            {
                return BadRequest("Este MAC já está vinculado a outro drone.");
            }


            // adiciona o drone ao contexto
            _context.DroneFabris.Add(droneFabri);

            // salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDroneFabri", new { id = droneFabri.DroneFabriId }, droneFabri);
        }

        // DELETE: api/DroneFabris/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDroneFabri(Guid id)
        {
            var droneFabri = await _context.DroneFabris.FindAsync(id);
            if (droneFabri == null)
            {
                return NotFound();
            }

            _context.DroneFabris.Remove(droneFabri);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DroneFabriExists(Guid id)
        {
            return _context.DroneFabris.Any(e => e.DroneFabriId == id);
        }
    }
}
