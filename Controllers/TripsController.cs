using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;
        public TripsController(ITripsService tripsService) => _tripsService = tripsService;
        
        // GET: /api/trips
        // -- Retrieves all available trips with their basic information -- //
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }
        
        // GET: /api/trips/{id}
        // -- Retrieves a single trip by its ID -- //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            try
            {
                var trips = await _tripsService.GetTrips();
                var trip = trips.FirstOrDefault(t => t.IdTrip == id);
                if (trip == null) return NotFound($"Trip {id} was not found.");
                
                return Ok(trip);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Trip with ID {id} not found.");
            }
        }
    }
}
