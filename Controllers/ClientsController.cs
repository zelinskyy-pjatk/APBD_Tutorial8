using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        public ClientsController(IClientsService clientsService) => _clientsService = clientsService;

        // GET: /api/clients/{id}/trips
        // Gets all trips the client is registered for //
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            try
            {
                var list = await _clientsService.GetAllClientTrips(id);
                return list.Count == 0 ? Ok("Client has no registered trips.") : Ok(list);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Client {id} was not found.");
            }
        }
    
        // POST: /api/clients
        // Creates a new client and then returns its ID //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDTO dto)
        {
            try
            {
                var newId = await _clientsService.CreateClient(dto);
                return Ok($"Client was successfully created with id {newId}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
        
        // PUT: /api/clients/{id}/trips/{tripId}
        // Registers a client for a specific trip //
        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> Register(int id, int tripId)
        {
            try
            {
                await _clientsService.RegisterClientForTrip(id, tripId);
                return Ok($"Client successfully registered for the trip {tripId}.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Client {id} or Trip {tripId} was not found.");
            }
        }
        
        // DELETE: /api/clients/{id}/trips/{tripId}
        // Unregisters a client from a specific trip // 
        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> Unregister(int id, int tripId)
        {
            try
            {
                await _clientsService.UnregisterClientFromTrip(id, tripId);
                return Ok("Registration successfully removed.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}