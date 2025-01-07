using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper; // Include AutoMapper
using ERP_API.Data;
using ERP_API.Moduls; // Assuming DTOs are in this namespace

namespace ERP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PmLabelGenerationsController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;

        public PmLabelGenerationsController(Lg202324Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PmLabelGenerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PmLabelGenerationReadOnlyDto>>> GetPmLabelGenerations()
        {
            try
            {
                if (_context.PmLabelGenerations == null)
                {
                    return NotFound();
                }

                var pmLabelGenerations = await _context.PmLabelGenerations.ToListAsync();
                return Ok(_mapper.Map<IEnumerable<PmLabelGenerationReadOnlyDto>>(pmLabelGenerations));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/PmLabelGenerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PmLabelGenerationReadOnlyDto>> GetPmLabelGeneration(int id)
        {
            try
            {
                if (_context.PmLabelGenerations == null)
                {
                    return NotFound();
                }

                var pmLabelGeneration = await _context.PmLabelGenerations.FindAsync(id);

                if (pmLabelGeneration == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<PmLabelGenerationReadOnlyDto>(pmLabelGeneration));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/PmLabelGenerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPmLabelGeneration(int id, PmLabelGenerationCreateDto pmLabelGenerationDto)
        {
            try
            {
                if (id != pmLabelGenerationDto.PmLabelId)
                {
                    return BadRequest("ID mismatch.");
                }

                var pmLabelGeneration = await _context.PmLabelGenerations.FindAsync(id);
                if (pmLabelGeneration == null)
                {
                    return NotFound();
                }

                _mapper.Map(pmLabelGenerationDto, pmLabelGeneration);
                _context.Entry(pmLabelGeneration).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PmLabelGenerationExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/PmLabelGenerations
        [HttpPost]
        public async Task<ActionResult<PmLabelGenerationReadOnlyDto>> PostPmLabelGeneration(PmLabelGenerationCreateDto pmLabelGenerationDto)
        {
            try
            {
                if (_context.PmLabelGenerations == null)
                {
                    return Problem("Entity set 'Lg202324Context.PmLabelGenerations' is null.");
                }

                var pmLabelGeneration = _mapper.Map<PmLabelGeneration>(pmLabelGenerationDto);
                _context.PmLabelGenerations.Add(pmLabelGeneration);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<PmLabelGenerationReadOnlyDto>(pmLabelGeneration);
                return CreatedAtAction("GetPmLabelGeneration", new { id = result.PmLabelId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/PmLabelGenerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePmLabelGeneration(int id)
        {
            try
            {
                if (_context.PmLabelGenerations == null)
                {
                    return NotFound();
                }

                var pmLabelGeneration = await _context.PmLabelGenerations.FindAsync(id);
                if (pmLabelGeneration == null)
                {
                    return NotFound();
                }

                _context.PmLabelGenerations.Remove(pmLabelGeneration);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool PmLabelGenerationExists(int id)
        {
            return (_context.PmLabelGenerations?.Any(e => e.PmLabelId == id)).GetValueOrDefault();
        }
    }
}
