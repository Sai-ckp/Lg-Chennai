using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP_API.Data;
using ERP_API.Moduls;

namespace ERP_API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FgLabelGenerationsController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;

        public FgLabelGenerationsController(Lg202324Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/FgLabelGenerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FglabelgenerationCreateDto>>> GetFgLabelGenerations()
        {
            try
            {
                if (_context.FgLabelGenerations == null)
                {
                    return NotFound("Entity set is null.");
                }

                var fgLabelGenerations = await _context.FgLabelGenerations.ToListAsync();
                var fgLabelGenerationDtos = _mapper.Map<IEnumerable<FglabelgenerationReadOnlyDto>>(fgLabelGenerations);

                return Ok(fgLabelGenerationDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/FgLabelGenerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FglabelgenerationReadOnlyDto>> GetFgLabelGeneration(int id)
        {
            try
            {
                if (_context.FgLabelGenerations == null)
                {
                    return NotFound("Entity set is null.");
                }

                var fgLabelGeneration = await _context.FgLabelGenerations.FindAsync(id);

                if (fgLabelGeneration == null)
                {
                    return NotFound();
                }

                var fgLabelGenerationDto = _mapper.Map<FglabelgenerationReadOnlyDto>(fgLabelGeneration);
                return Ok(fgLabelGenerationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/FgLabelGenerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFgLabelGeneration(int id, FglabelgenerationCreateDto fgLabelGenerationDto)
        {
            try
            {
                if (id != fgLabelGenerationDto.FgLabelId)
                {
                    return BadRequest("ID mismatch.");
                }

                var fgLabelGeneration = _mapper.Map<FgLabelGeneration>(fgLabelGenerationDto);
                _context.Entry(fgLabelGeneration).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FgLabelGenerationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<ActionResult<FglabelgenerationReadOnlyDto>> PostFgLabelGeneration(FglabelgenerationCreateDto fgLabelGenerationDto)
        {
            try
            {
                if (_context.FgLabelGenerations == null)
                {
                    return Problem("Entity set is null.");
                }

                var fgLabelGeneration = _mapper.Map<FgLabelGeneration>(fgLabelGenerationDto);

                // Get the count of existing labels with the same batch number
                var batchCount = await _context.FgLabelGenerations
                    .Where(x => x.BatchNo == fgLabelGenerationDto.BatchNo)
                    .CountAsync();

                // Get the batch number from the input
                var BatchNo = fgLabelGenerationDto.BatchNo;
                var sequenceNo = batchCount + 1;

                // Set the serial number in format XX/YY
                fgLabelGeneration.SerialNumber = int.Parse($"{BatchNo}{sequenceNo:D2}");

                // Set the FgLabelId
                var lastFgLabel = await _context.FgLabelGenerations
                    .OrderByDescending(x => x.FgLabelId)
                    .FirstOrDefaultAsync();
                fgLabelGeneration.FgLabelId = (lastFgLabel?.FgLabelId ?? 0) + 1;

                _context.FgLabelGenerations.Add(fgLabelGeneration);
                await _context.SaveChangesAsync();

                var createdDto = _mapper.Map<FglabelgenerationReadOnlyDto>(fgLabelGeneration);
                return CreatedAtAction("GetFgLabelGeneration", new { id = fgLabelGeneration.FgLabelId }, createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/FgLabelGenerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFgLabelGeneration(int id)
        {
            try
            {
                if (_context.FgLabelGenerations == null)
                {
                    return NotFound("Entity set is null.");
                }

                var fgLabelGeneration = await _context.FgLabelGenerations.FindAsync(id);

                if (fgLabelGeneration == null)
                {
                    return NotFound();
                }

                _context.FgLabelGenerations.Remove(fgLabelGeneration);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/FgLabelGenerations/latest
        [HttpGet("latest")]
        public async Task<ActionResult<FglabelgenerationReadOnlyDto>> GetLatestFgLabelGeneration()
        {
            try
            {
                if (_context.FgLabelGenerations == null)
                {
                    return NotFound("Entity set is null.");
                }

                var latestFgLabelGeneration = await _context.FgLabelGenerations
                    .OrderByDescending(fg => fg.FgLabelId)
                    .FirstOrDefaultAsync();

                if (latestFgLabelGeneration == null)
                {
                    return NotFound("No FG Label Generations found.");
                }

                var latestFgLabelGenerationDto = _mapper.Map<FglabelgenerationReadOnlyDto>(latestFgLabelGeneration);
                return Ok(latestFgLabelGenerationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool FgLabelGenerationExists(int id)
        {
            return (_context.FgLabelGenerations?.Any(e => e.FgLabelId == id)).GetValueOrDefault();
        }
    }
}