using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP_API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP_API.Moduls;

namespace ERP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;

        public CompanyDetailsController(Lg202324Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/companydetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<companydetailsReadOnlyDto>>> GetCompanyDetails()
        {
            try
            {
                if (_context.companydetails == null)
                {
                    return NotFound();
                }
                var companyDetailsList = await _context.companydetails.ToListAsync();

                // Map entity list to DTO list
                var companyDetailsDtoList = _mapper.Map<List<companydetailsReadOnlyDto>>(companyDetailsList);

                return Ok(companyDetailsDtoList);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/companydetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<companydetailsReadOnlyDto>> GetCompanyDetails(int id)
        {
            try
            {
                if (_context.companydetails == null)
                {
                    return NotFound();
                }
                var companyDetails = await _context.companydetails.FindAsync(id);

                if (companyDetails == null)
                {
                    return NotFound();
                }

                // Map entity to DTO
                var companyDetailsDto = _mapper.Map<companydetailsReadOnlyDto>(companyDetails);

                return Ok(companyDetailsDto);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/companydetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyDetails(int id, companydetails companyDetails)
        {
            if (id != companyDetails.CompanyId)
            {
                return BadRequest();
            }

            _context.Entry(companyDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyDetailsExists(id))
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

        // POST: api/companydetails
        [HttpPost]
        public async Task<ActionResult<companydetailsReadOnlyDto>> PostCompanyDetails(companydetails companyDetails)
        {
            try
            {
                if (_context.companydetails == null)
                {
                    return Problem("Entity set 'Lg202324Context.companydetails' is null.");
                }

                _context.companydetails.Add(companyDetails);
                await _context.SaveChangesAsync();

                // Map the created entity to a DTO
                var companyDetailsDto = _mapper.Map<companydetailsReadOnlyDto>(companyDetails);

                return CreatedAtAction("GetCompanyDetails", new { id = companyDetails.CompanyId }, companyDetailsDto);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/companydetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyDetails(int id)
        {
            try
            {
                if (_context.companydetails == null)
                {
                    return NotFound();
                }
                var companyDetails = await _context.companydetails.FindAsync(id);
                if (companyDetails == null)
                {
                    return NotFound();
                }

                _context.companydetails.Remove(companyDetails);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool CompanyDetailsExists(int id)
        {
            return (_context.companydetails?.Any(e => e.CompanyId == id)).GetValueOrDefault();
        }
    }
}
