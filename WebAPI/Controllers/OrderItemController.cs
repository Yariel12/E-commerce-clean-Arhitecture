using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetById(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(int orderId)
        {
            var items = await _orderItemService.GetByOrderIdAsync(orderId);
            return Ok(items);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] OrderItemDto orderItemDto)
        {
            await _orderItemService.AddAsync(orderItemDto);
            return CreatedAtAction(nameof(GetById), new { id = orderItemDto.Id }, orderItemDto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] OrderItemDto orderItemDto)
        {
            if (id != orderItemDto.Id) return BadRequest("ID mismatch");

            await _orderItemService.UpdateAsync(orderItemDto);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _orderItemService.DeleteAsync(id);
            return NoContent();
        }
    }
}
