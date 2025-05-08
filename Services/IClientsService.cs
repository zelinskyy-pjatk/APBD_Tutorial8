using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientTripDTO>> GetClientTrips(int clientId);
    Task<int> CreateClient(CreateClientDTO dto);
    Task RegisterClientToTrip(int clientId, int tripId);
    Task UnregisterClientFromTrip(int clientId, int tripId);
}