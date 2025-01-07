using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ERP_API.Data;

using ERP_API.Moduls;

namespace ERP_API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RmLabelGenerationsController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RmLabelGenerationsController> _logger;

        public RmLabelGenerationsController(Lg202324Context context, IMapper mapper, ILogger<RmLabelGenerationsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/RmLabelGenerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RmLabelGenerationReadOnlyDto>>> GetRmLabelGenerations()
        {
            try
            {
                if (_context.RmLabelGenerations == null)
                {
                    _logger.LogWarning("Entity set RmLabelGenerations is null.");
                    return NotFound();
                }

                var rmLabelGenerations = await _context.RmLabelGenerations
                    .OrderByDescending(r => r.LabelId)
                    .ToListAsync();

                var rmLabelGenerationsDto = _mapper.Map<IEnumerable<RmLabelGenerationReadOnlyDto>>(rmLabelGenerations);

                _logger.LogInformation("Retrieved all RM Label Generations.");
                return Ok(rmLabelGenerationsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving RM Label Generations.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // GET: api/RmLabelGenerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RmLabelGenerationReadOnlyDto>> GetRmLabelGeneration(int id)
        {
            try
            {
                var rmLabelGeneration = await _context.RmLabelGenerations.FindAsync(id);

                if (rmLabelGeneration == null)
                {
                    _logger.LogWarning($"RM Label Generation with ID {id} was not found.");
                    return NotFound();
                }

                var rmLabelGenerationDto = _mapper.Map<RmLabelGenerationReadOnlyDto>(rmLabelGeneration);

                _logger.LogInformation($"Retrieved RM Label Generation with ID {id}.");
                return Ok(rmLabelGenerationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving RM Label Generation with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // PUT: api/RmLabelGenerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmLabelGeneration(int id, RmLabelGenerationCreateDto rmLabelGenerationDto)
        {
            if (id != rmLabelGenerationDto.LabelId)
            {
                _logger.LogWarning("PUT request failed due to mismatched IDs.");
                return BadRequest();
            }

            try
            {
                var rmLabelGeneration = _mapper.Map<RmLabelGeneration>(rmLabelGenerationDto);
                _context.Entry(rmLabelGeneration).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated RM Label Generation with ID {id}.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RmLabelGenerationExists(id))
                {
                    _logger.LogWarning($"RM Label Generation with ID {id} does not exist.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Concurrency error occurred while updating RM Label Generation with ID {id}.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating RM Label Generation with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // POST: api/RmLabelGenerations
        [HttpPost]
        public async Task<ActionResult<RmLabelGenerationReadOnlyDto>> PostRmLabelGeneration(RmLabelGenerationCreateDto rmLabelGenerationDto)
        {
            try
            {
                var rmLabelGeneration = _mapper.Map<RmLabelGeneration>(rmLabelGenerationDto);
                _context.RmLabelGenerations.Add(rmLabelGeneration);
                await _context.SaveChangesAsync();

                var createdRmLabelGeneration = await _context.RmLabelGenerations
                    .FirstOrDefaultAsync(r => r.LabelId == rmLabelGeneration.LabelId);

                var createdRmLabelGenerationDto = _mapper.Map<RmLabelGenerationReadOnlyDto>(createdRmLabelGeneration);

                _logger.LogInformation($"Created a new RM Label Generation with ID {rmLabelGeneration.LabelId}.");
                return CreatedAtAction(nameof(GetRmLabelGeneration), new { id = rmLabelGeneration.LabelId }, createdRmLabelGenerationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an RM Label Generation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // DELETE: api/RmLabelGenerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmLabelGeneration(int id)
        {
            try
            {
                var rmLabelGeneration = await _context.RmLabelGenerations.FindAsync(id);

                if (rmLabelGeneration == null)
                {
                    _logger.LogWarning($"RM Label Generation with ID {id} was not found.");
                    return NotFound();
                }

                _context.RmLabelGenerations.Remove(rmLabelGeneration);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted RM Label Generation with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting RM Label Generation with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private bool RmLabelGenerationExists(int id)
        {
            return (_context.RmLabelGenerations?.Any(r => r.LabelId == id)).GetValueOrDefault();
        }
    }
}
