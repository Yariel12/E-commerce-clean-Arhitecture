using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            return Ok(role);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] Role role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
                return BadRequest(new { message = "El nombre del rol es obligatorio." });

            try
            {
                var newRole = await _roleService.AddAsync(role);
                return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
