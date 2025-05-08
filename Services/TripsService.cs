using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        const string sql_command = @"SELECT t.IdTrip, t.Name,
                                     t.Description, t.StartDate, t.EndDate, t.MaxPeople, c.Name 
                                     FROM Trip T 
                                     JOIN Country c ON t.IdCountry = c.CountryId";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sql_command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        DateFrom = reader.GetDataTime(3),
                        DateTo = reader.GetDataTime(4),
                        MaxPeople = reader.GetInt32(5),
                    });
                }
                return trips;
            }
        }
        

        return trips;
    }
}