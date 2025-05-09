﻿namespace Tutorial8.Models.DTOs;

/* -- ClientTripDTO -- */
/*
    Purpose:
        Used to return trip registration details for a given client.

    Used in:
        - GET /api/clients/{id}/trips
 */
public class ClientTripDTO
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public string TripName { get; set; }
    public string? Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int RegisteredAt { get; set; } 
    public int? PaymentDate { get; set; }
}