namespace Tutorial8.Models.DTOs;

/* -- TripDTO -- */
/*
    Purpose:
        Used to return trip details, including countries.
    Used in: 
        - GET /api/trips
        - GET /api/trips/{id}
    Aggregates data from Trip and Country_Trip tables.
 */
public class TripDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO> Countries { get; set; }
}

