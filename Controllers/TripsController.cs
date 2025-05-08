using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            try
            {
                var trip = await _tripsService.GetTrips();
                return Ok(trip);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Trip with ID {id} not found.");
            }
        }

        [HttpGet("TEST")]
        public IActionResult Test() => Ok("API is running.");
    }
}
