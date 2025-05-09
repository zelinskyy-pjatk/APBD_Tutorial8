using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private const string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    // -- 'GetTrips' Method to get information about all trips (also with countries associated to them) -- // 
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();
    
        // -- SQL to retrieve all trip basic info -- //
        const string trip_sql_command = 
            """
                SELECT t.IdTrip, t.Name,
                       t.Description, t.DateFrom, t.DateTo, 
                       t.MaxPeople
                FROM Trip t
            """;
        // -- SQL to retrieve all countries associated with trips -- //
        const string countries_sql_command =
            """
                SELECT ct.IdTrip, c.IdCountry, c.Name
                FROM Country_Trip ct
                JOIN Country c ON ct.IdCountry = c.IdCountry;
            """;
        
        // -- Fetch all trips -- //
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(trip_sql_command, conn))
        {
            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripDTO()
                    {
                        IdTrip = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                    });
                }
            }
        }
        
        // -- Fetch all countries associated with trips -- //
        var countriesMap = new Dictionary<int, List<CountryDTO>>();
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(countries_sql_command, conn))
        {
            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(0);
                    var country = new CountryDTO()
                    {
                        IdCountry = reader.GetInt32(1),
                        Name = reader.GetString(2)
                    };

                    if (!countriesMap.ContainsKey(tripId))
                        countriesMap[tripId] = new List<CountryDTO>();
                    countriesMap[tripId].Add(country);
                }
            }
        }
        
        foreach (var trip in trips) 
            if (countriesMap.TryGetValue(trip.IdTrip, out var countries))
                trip.Countries = countries;
        
        return trips;
    }
}