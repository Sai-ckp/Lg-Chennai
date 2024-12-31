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
using ERP_API.Static;
using Newtonsoft.Json;

namespace ERP_API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseordersController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseordersController> _logger;

        public PurchaseordersController(Lg202324Context context, IMapper mapper, ILogger<PurchaseordersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseorderReadOnly>>> GetPurchaseorders()
        {
            try
            {
                if (_context.Purchaseorders == null)
                {
                    _logger.LogWarning("Attempted to retrieve purchase orders, but the entity set is null.");
                    return NotFound("Purchase orders entity set is null.");
                }

                var purchaseOrders = await _context.Purchaseorders
                    .Include(e => e.PurchaseorderSubs.OrderBy(s => s.SlNo)) // Order subs by SlNo
                    .Select(po => new Purchaseorder
                    {
                        POId = po.POId,
                        Podate = po.Podate,
                        Pono = po.Pono,
                        VendId = po.VendId,
                        PurchaseorderSubs = po.PurchaseorderSubs.Select(sub => new PurchaseorderSub
                        {
                            POSubId = sub.POSubId,
                            POId = sub.POId,
                            ItemId = sub.ItemId,
                            Qty = sub.Qty,
                            SlNo = sub.SlNo
                            // Add other properties you need
                        }).ToList()
                    })
                    .OrderByDescending(e => e.POId)
                    .ToListAsync();

                var purchaseOrderDtos = _mapper.Map<IEnumerable<PurchaseorderReadOnly>>(purchaseOrders);

                _logger.LogInformation("Retrieved all purchase orders successfully.");
                return Ok(purchaseOrderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving purchase orders: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // GET: api/Purchaseorders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseorderReadOnly>> GetPurchaseorder(int id)
        {
            try
            {
                if (_context.Purchaseorders == null)
                {
                    _logger.LogWarning("Attempted to retrieve a purchase order, but the entity set is null.");
                    return NotFound("Purchase orders entity set is null.");
                }

                var purchaseOrder = await _context.Purchaseorders
                    .Include(e => e.PurchaseorderSubs)
                    .FirstOrDefaultAsync(e => e.POId == id); 

                if (purchaseOrder == null)
                {
                    _logger.LogWarning($"Purchase order with ID {id} was not found.");
                    return NotFound($"Purchase order with ID {id} was not found.");
                }

                var purchaseOrderDto = _mapper.Map<PurchaseorderReadOnly>(purchaseOrder);

                _logger.LogInformation($"Retrieved purchase order with ID {id} successfully.");
                return Ok(purchaseOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the purchase order with ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrder(int id, PurchaseorderCreateOnly purchaseorderDto)
        {
            if (id != purchaseorderDto.POId)
            {
                _logger.LogWarning("PUT request failed due to mismatched IDs.");
                return BadRequest();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Load the existing purchase order with its sub-items
                var existingPurchaseOrder = await _context.Purchaseorders
                    .Include(m => m.PurchaseorderSubs)
                    .FirstOrDefaultAsync(m => m.POId == id);

                if (existingPurchaseOrder == null)
                {
                    return NotFound();
                }

                // Update main purchase order properties
                existingPurchaseOrder.Pono = purchaseorderDto.Pono;
                existingPurchaseOrder.Podate = purchaseorderDto.Podate;
                existingPurchaseOrder.VendId = purchaseorderDto.VendId;

                // Remove existing sub-items
                _context.PurchaseorderSubs.RemoveRange(existingPurchaseOrder.PurchaseorderSubs);
                await _context.SaveChangesAsync();

                // Add new sub-items
                if (purchaseorderDto.PurchaseorderSubs != null)
                {
                    var latestSubId = await _context.PurchaseorderSubs
                        .OrderByDescending(s => s.POSubId)
                        .Select(s => s.POSubId)
                        .FirstOrDefaultAsync();

                    int nextSubId = (latestSubId ?? 0) + 1;
                    int slNo = 1;

                    foreach (var dtoSub in purchaseorderDto.PurchaseorderSubs)
                    {
                        var newSub = new PurchaseorderSub
                        {
                            POSubId = nextSubId++,
                            POId = existingPurchaseOrder.POId,
                            ItemId = dtoSub.ItemId,
                            Qty = dtoSub.Qty,
                            SlNo = slNo++
                        };

                        _context.PurchaseorderSubs.Add(newSub);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error updating purchase order: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the purchase order.");
            }
        }

        [HttpPost]
public async Task<ActionResult<PurchaseorderReadOnly>> PostPurchaseorder(PurchaseorderCreateOnly purchaseOrderDto)
{
    try
    {
        // Validate input
        if (purchaseOrderDto == null)
        {
            _logger.LogWarning("Purchase order DTO is null.");
            return BadRequest("Purchase order data is required.");
        }

        // Check if Purchaseorders DbSet is null
        if (_context.Purchaseorders == null)
        {
            _logger.LogWarning("Purchaseorders entity set is null.");
            return Problem("Purchase orders entity set is null.");
        }

        // Start a database transaction to ensure atomicity
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Map DTO to entity with null check
            var purchaseorder = _mapper.Map<Purchaseorder>(purchaseOrderDto);
            if (purchaseorder == null)
            {
                _logger.LogError("Failed to map purchase order DTO to entity.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Mapping error occurred.");
            }

            // Generate POId by fetching the latest one
            var latestPOId = await _context.Purchaseorders
                .OrderByDescending(p => p.POId)
                .Select(p => p.POId)
                .FirstOrDefaultAsync();

            purchaseorder.POId = (latestPOId > 0 ? latestPOId : 0) + 1;  // Increment POId from the latest one
            _logger.LogDebug($"Generated POId: {purchaseorder.POId}");

            // Initialize PurchaseorderSubs if null
            purchaseorder.PurchaseorderSubs ??= new List<PurchaseorderSub>();

            // Handle PurchaseorderSubs (sub-items) if present
            if (purchaseorder.PurchaseorderSubs.Any())
            {
                var latestSubId = await _context.PurchaseorderSubs
                    .OrderByDescending(s => s.POSubId)
                    .Select(s => s.POSubId)
                    .FirstOrDefaultAsync();

                int nextSubId = (latestSubId ?? 0) + 1;
                int slNo = 1;

                foreach (var sub in purchaseorder.PurchaseorderSubs.Where(s => s != null))
                {
                    sub.POSubId = nextSubId++;   // Increment POSubId
                    sub.POId = purchaseorder.POId; // Link sub-item to main purchase order
                    sub.SlNo = slNo++;            // Set the serial number (SlNo)
                    sub.ItemId = sub.ItemId > 0 ? sub.ItemId : 0;  // Default ItemId to 0 if invalid
                    sub.Qty = sub.Qty > 0 ? sub.Qty : 0;            // Default Qty to 0 if invalid

                    _logger.LogDebug($"Processed sub-item: POSubId={sub.POSubId}, POId={sub.POId}, SlNo={sub.SlNo}");
                }
            }
            else
            {
                _logger.LogInformation("No sub-items provided.");
            }

            // Add the main purchase order to the context
            _context.Purchaseorders.Add(purchaseorder);
            _logger.LogInformation("Purchase order added to DbContext.");

            // Save changes to the database
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Purchase order with POId={purchaseorder.POId} saved successfully.");

            // Commit the transaction to finalize the changes
            await transaction.CommitAsync();
            _logger.LogInformation($"Transaction committed for POId: {purchaseorder.POId}");

            // Retrieve the created purchase order, including its sub-items
            var createdPurchaseOrder = await _context.Purchaseorders
                .Include(p => p.PurchaseorderSubs)
                .FirstOrDefaultAsync(p => p.POId == purchaseorder.POId);

            if (createdPurchaseOrder == null)
            {
                _logger.LogError($"Failed to retrieve created purchase order with POId={purchaseorder.POId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create purchase order.");
            }

            // Map the created purchase order to the read-only DTO
            var createdOrderDto = _mapper.Map<PurchaseorderReadOnly>(createdPurchaseOrder);
            if (createdOrderDto == null)
            {
                _logger.LogError("Failed to map created purchase order to DTO.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Mapping error occurred.");
            }

            // Return the created purchase order with a 201 Created response
            return CreatedAtAction(nameof(GetPurchaseorder), new { id = purchaseorder.POId }, createdOrderDto);
        }
        catch (Exception ex)
        {
            // Rollback the transaction in case of an error
            await transaction.RollbackAsync();
            _logger.LogError($"Transaction error: {ex.Message}, Stack Trace: {ex.StackTrace}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while processing the transaction.");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Unexpected error: {ex.Message}, Stack Trace: {ex.StackTrace}");
        return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
    }
}





        // DELETE: api/Purchaseorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseorder(int id)
        {
            try
            {
                if (_context.Purchaseorders == null)
                {
                    _logger.LogWarning("Attempted to delete a purchase order, but the entity set is null.");
                    return NotFound("Purchase orders entity set is null.");
                }

                var purchaseOrder = await _context.Purchaseorders.
                    Include(e => e.PurchaseorderSubs)
                  .FirstOrDefaultAsync(e => e.POId == id);
                if (purchaseOrder == null)
                {
                    _logger.LogWarning($"Purchase order with ID {id} was not found.");
                    return NotFound($"Purchase order with ID {id} was not found.");
                }

                _context.Purchaseorders.Remove(purchaseOrder);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted purchase order with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the purchase order with ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private bool PurchaseorderExists(int id)
        {
            return (_context.Purchaseorders?.Any(e => e.POId == id)).GetValueOrDefault();
        }
    }
}
