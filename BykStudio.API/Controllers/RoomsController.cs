using BykStudio.data;
using BykStudio.data.DTOs;
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
            try
            {
                Console.WriteLine($"Requested: {name}");
                var room = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());

                if (room == null)
                    return NotFound();
                return room;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetRoomByName('{name}'): {ex.Message}\n{ex}");
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }

        // Optional: get all rooms (if you need it later)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            try
            {
                var rooms = await _context.Rooms
                    .Select(r => new RoomDto
                    {
                        RoomId = r.RoomId,
                        Name = r.Name,
                        PricePerHour = r.PricePerHour,
                        Description = r.Description,
                        Capacity = r.Capacity,
                        MainImageUrl = r.MainImageUrl,
                        Photos = r.Photos,
                        IsAvailable = r.IsAvailable
                    })
                    .ToListAsync();

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
