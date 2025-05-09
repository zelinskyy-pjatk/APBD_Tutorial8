using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    // -- Configuration String -- //
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    // -- 'GetAllClientTrips' Method returns a list of trips a given client is registered for -- //
    // -- Trip Details and Registration/Payment Info are also included -- //
    public async Task<List<ClientTripDTO>> GetAllClientTrips(int clientId)
    {
        var trips = new List<ClientTripDTO>();

        // -- Check if client exists -- //
        const string client_check_sql_command = "SELECT COUNT(1) FROM Client WHERE IdClient = @idClient;"; 
        
        using (var conn = new SqlConnection(_connectionString))
        using (var checkCmd = new SqlCommand(client_check_sql_command, conn))
        {
            checkCmd.Parameters.AddWithValue("@idClient", clientId);
            var clientExists = (int) await checkCmd.ExecuteScalarAsync();
            
            if (clientExists == 0)
                throw new KeyNotFoundException($"Client {clientId} does not exist.");
        }
        
        // -- Get all client's trips -- //
        const string sql_command = 
            """
                SELECT t.IdTrip, t.Name,
                       t.Description, t.DateFrom, 
                       t.DateTo, ct.RegisteredAt, ct.PaymentDate
                FROM Client_Trip ct
                JOIN Trip t ON ct.IdTrip = t.IdTrip
                WHERE ct.IdClient = @idClient;
            """;
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql_command, conn))
        {
            await conn.OpenAsync();
          
            cmd.Parameters.AddWithValue("@idClient", clientId);
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new ClientTripDTO()
                    {
                        IdClient = clientId,
                        IdTrip = reader.GetInt32(0),
                        TripName = reader.GetString(1),
                        DateFrom = reader.GetDateTime(2),
                        DateTo = reader.GetDateTime(3),
                        RegisteredAt = reader.GetInt32(4),
                        PaymentDate = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                    });
                }
            }
        }
        return trips;
    }
    
    // -- 'CreateClient' Method creates a new client based on given details -- //
    public async Task<int> CreateClient(CreateClientDTO clientDto)
    {
        const string sql_command = 
            """
                INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                VALUES (@firstName, @lastName, @email, @telephone, @pesel)
                SELECT CAST(SCOPE_IDENTITY() AS int);
            """;
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql_command, conn))
        {
            cmd.Parameters.AddWithValue("@firstName", clientDto.FirstName);
            cmd.Parameters.AddWithValue("@lastName", clientDto.LastName);
            cmd.Parameters.AddWithValue("@email", clientDto.Email);
            cmd.Parameters.AddWithValue("@telephone", clientDto.Telephone);
            cmd.Parameters.AddWithValue("@pesel", clientDto.Pesel);
            
            await conn.OpenAsync();
            
            try
            {
                return (int) await cmd.ExecuteScalarAsync();
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                throw new InvalidOperationException("A client with such PESEL already exists", ex);
            }
        }
    }

    // -- 'RegisterClientForTrip' Method to register client with given clientId to trip with given tripId -- // 
    public async Task RegisterClientForTrip(int clientId, int tripId)
    {
        const string sql_command = 
            """
                IF NOT EXISTS (SELECT 1 FROM Client WHERE IdClient = @idClient)
                    RAISERROR('CLIENT_NOT_FOUND', 16, 1);
                
                IF NOT EXISTS (SELECT 1 FROM Trip WHERE IdTrip = @idTrip)
                    RAISERROR('TRIP_NOT_FOUND', 16, 1);
                
                IF EXISTS (SELECT 1 FROM Client_Trip 
                           WHERE IdClient = @idClient AND IdTrip = @idTrip)
                    RAISERROR('ALREADY_REGISTERED', 16, 1);
                
                IF (SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @idTrip) >= 
                   (SELECT MaxPeople FROM Trip WHERE IdTrip = @idTrip)
                    RAISERROR('TRIP_FULL', 16, 1);
                
                INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                VALUES (@idClient, @idTrip, 
                        CONVERT(INT, FORMAT(GETDATE(), 'yyyyMMdd')), 
                        NULL
                        );
            """;

        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql_command, conn))
        {
            cmd.Parameters.AddWithValue("@idClient", clientId);
            cmd.Parameters.AddWithValue("@idTrip", tripId);
            
            await conn.OpenAsync();
            
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) // --> Check if exception returned message with specific content and throw appropriate exceptions
            {
                if (ex.Message.Contains("CLIENT_NOT_FOUND"))
                    throw new KeyNotFoundException("Client does not exist.");
                
                if (ex.Message.Contains("TRIP_NOT_FOUND"))
                    throw new KeyNotFoundException("Trip does not exist."); 
                
                if (ex.Message.Contains("ALREADY_REGISTERED"))
                    throw new InvalidOperationException("Client is already registered for this trip.");

                if (ex.Message.Contains("TRIP_FULL"))
                    throw new InvalidOperationException(
                        "Trip cannot register any more people. It reached its maximum capacity.");

                throw;
            }
        }
    }
    
    // -- 'UnregisterClientFromTrip' Method  -- // 
    public async Task UnregisterClientFromTrip(int clientId, int tripId)
    {
        const string reg_check_sql_command = 
            """
                SELECT COUNT(1) FROM Client_Trip 
                WHERE IdClient = @idClient AND IdTrip = @idtrip;
            """;
        const string delete_sql_command = 
            """
                DELETE FROM Client_Trip
                WHERE IdClient = @idClient AND IdTrip = @idTrip;
            """;
        
        // -- Check if registration actually exists -- //
        using (var conn = new SqlConnection(_connectionString))
        using (var checkCmd = new SqlCommand(reg_check_sql_command, conn))
        {
            await conn.OpenAsync();
            checkCmd.Parameters.AddWithValue("@idClient", clientId);
            checkCmd.Parameters.AddWithValue("@idTrip", tripId);
            var exists = (int) await checkCmd.ExecuteScalarAsync();
            if (exists == 0) 
                throw new KeyNotFoundException($"Registration for client {clientId} and trip {tripId} was not found.");
        }
        
        // -- Unregister client from trip -- //
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(delete_sql_command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@idClient", clientId);
            cmd.Parameters.AddWithValue("@idTrip", tripId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}