using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    public async Task<List<ClientTripDTO>> GetAllClientTrips(int clientId)
    {
        var list = new List<ClientTripDTO>();

        const string sql_command = """
                                   SELECT t.IdTrip, t.Name,
                                          t.Description, t.DateFrom, 
                                          t.DateTo, ct.RegisteredAt, ct.PaymentDate
                                          FROM Client_Trip ct
                                          JOIN Trip t ON ct.IdTrip = t.IdTrip
                                          WHERE ct.IdClient = @clientId;
                                   """;
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql_command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@clientId", clientId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var idOrdinal = reader.GetOrdinal("IdTrip");
                    list.Add(new ClientTripDTO()
                    {
                        IdTrip = reader.GetInt32(idOrdinal),
                        TripName = reader.GetString(1),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        RegisteredAt = reader.GetInt32(5),
                        PaymentDate = reader.GetInt32(6)
                    });
                }
            }
        }
        return list;
    }

    public async Task<int> CreateClient(CreateClientDTO dto)
    {
        if(string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName)
           || string.IsNullOrWhiteSpace(dto.Email)  || string.IsNullOrWhiteSpace(dto.Pesel))
            throw new ArgumentException("First Name, Last Name, Email and Pesel must be filled.");

        const string sql_command = """
                                   INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                                   VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)
                                   SELECT CAST(SCOPE_IDENTITY() AS int);
                                   """;
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql_command, conn))
        {
            await conn.OpenAsync();
            try
            {
                cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
                cmd.Parameters.AddWithValue("@LastName", dto.LastName);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Telephone", dto.Telephone);
                cmd.Parameters.AddWithValue("@Pesel", dto.Pesel);
                return (int) await cmd.ExecuteScalarAsync();
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                throw new InvalidOperationException("PESEL must be unique.", ex);
            }
        }

    }

    public async Task RegisterClientToTrip(int clientId, int tripId)
    {
        const string sql = """
                           IF NOT EXISTS (SELECT 1 FROM Client WHERE IdClient = @cid)
                           RAISERROR('Client not found', 16, 1);
                           
                           IF NOT EXISTS (SELECT 1 FROM Trip WHERE IdTrip = @tid)
                               RAISERROR('Trip not found', 16, 1);
                           
                           IF EXISTS (SELECT 1 FROM Client_Trip WHERE IdClient = @cid AND IdTrip = @tid)
                               RAISERROR('Already registered', 16, 1);
                           
                           IF (SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tid) >= 
                              (SELECT MaxPeople FROM Trip WHERE IdTrip = @tid)
                               RAISERROR('Trip is full', 16, 1);
                           
                           INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                           VALUES (@cid, @tid, CONVERT(INT, CONVERT(VARCHAR(8), GETDATE(), 112)), CONVERT(INT, CONVERT(VARCHAR(8), GETDATE(), 112)));
                                
                           """;

        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(sql, conn))
        {
            await conn.OpenAsync();
            try
            {
                cmd.Parameters.AddWithValue("@cid", clientId);
                cmd.Parameters.AddWithValue("@tid", tripId);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }

    public async Task UnregisterClientFromTrip(int clientId, int tripId)
    {
        const string check = """
                             SELECT COUNT(1) FROM Client_Trip 
                             WHERE IdClient = @cid AND IdTrip = @tid;
                             """;
        const string delete_sql_command = @"
                     DELETE FROM Client_Trip
                     WHERE IdClient = @cid AND IdTrip = @tid;";
        
        using (var conn = new SqlConnection(_connectionString))
        using (var check_cmd = new SqlCommand(check, conn))
        {
            await conn.OpenAsync();
            check_cmd.Parameters.AddWithValue("@cid", clientId);
            check_cmd.Parameters.AddWithValue("@tid", tripId);
            var exists = (int) await check_cmd.ExecuteScalarAsync() > 0;
            if (!exists) throw new KeyNotFoundException($"Registration not found.");
        }

        await using (var conn = new SqlConnection(_connectionString))
        await using (var cmd = new SqlCommand(delete_sql_command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@cid", clientId);
            cmd.Parameters.AddWithValue("@tid", tripId);
            await cmd.ExecuteNonQueryAsync();
        }

    }
}