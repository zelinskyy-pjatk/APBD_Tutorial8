﻿using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientTripDTO>> GetAllClientTrips(int clientId);
    Task<int> CreateClient(CreateClientDTO dto);
    Task RegisterClientForTrip(int clientId, int tripId);
    Task UnregisterClientFromTrip(int clientId, int tripId);
}