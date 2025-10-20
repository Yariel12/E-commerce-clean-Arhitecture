using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Token inválido o usuario no autenticado." });

            int userId = int.Parse(userIdClaim);

            var addresses = await _addressService.GetAllByUserIdAsync(userId);
            if (addresses == null || !addresses.Any())
                return NotFound(new { message = "No addresses found for this user." });

            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDto>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid address ID." });

            var address = await _addressService.GetByIdAsync(id);
            if (address == null)
                return NotFound(new { message = $"Address with ID {id} was not found." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (address.UserId.ToString() != userIdClaim)
                return Forbid("You do not have permission to access this address.");

            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Token inválido o usuario no autenticado." });

            int userId = int.Parse(userIdClaim);
            addressDto.UserId = userId; 

            var created = await _addressService.AddAsync(addressDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AddressDto addressDto)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid address ID." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _addressService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Address with ID {id} was not found." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existing.UserId.ToString() != userIdClaim)
                return Forbid("You do not have permission to update this address.");

            addressDto.Id = id;
            addressDto.UserId = existing.UserId;

            var updated = await _addressService.UpdateAsync(addressDto);
            return Ok(new { message = "Address updated successfully.", data = updated });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid address ID." });

            var address = await _addressService.GetByIdAsync(id);
            if (address == null)
                return NotFound(new { message = $"Address with ID {id} was not found." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (address.UserId.ToString() != userIdClaim)
                return Forbid("You do not have permission to delete this address.");

            await _addressService.DeleteAsync(id);
            return Ok(new { message = "Address deleted successfully." });
        }
    }
}
