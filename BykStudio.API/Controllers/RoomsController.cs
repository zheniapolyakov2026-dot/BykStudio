using BykStudio.data;
using BykStudio.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rooms/byName/{name}
        [HttpGet("byName/{name}")]
        public async Task<ActionResult<Room>> GetRoomByName(string name)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());

            if (room == null)
                return NotFound();

            return room;
        }

        // Optional: get all rooms (if you need it later)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }
    }
}
