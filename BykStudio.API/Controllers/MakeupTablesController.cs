using BykStudio.data;
using BykStudio.data.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MakeupTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/makeuptables (returns the single table or list)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MakeupTableDto>>> GetMakeupTables()
        {
            var tables = await _context.MakeupTables
                .Select(t => new MakeupTableDto
                {
                    MakeupTableId = t.MakeupTableId,
                    Name = t.Name,
                    PricePerHour = t.PricePerHour,
                    Description = t.Description,
                    MainImageUrl = t.MainImageUrl,
                    IsAvailable = t.IsAvailable
                })
                .ToListAsync();

            return Ok(tables);
        }
    }
}
