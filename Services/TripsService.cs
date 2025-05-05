using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services
{
    public class TripsService : ITripsService
    {
        
        public async Task<bool> RemoveClientFromTrip(int clientId, int tripId)
        {
            const string query = @"
        DELETE FROM Client_Trip
        WHERE IdClient = @IdClient AND IdTrip = @IdTrip";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@IdClient", clientId);
            cmd.Parameters.AddWithValue("@IdTrip", tripId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteNonQueryAsync();

            return result > 0; 
        }

        public async Task<bool> AssignClientToTrip(int clientId, int tripId, ClientTripAssignDTO data)
        {
            const string checkQuery = @"
        SELECT COUNT(*) FROM Client_Trip 
        WHERE IdClient = @IdClient AND IdTrip = @IdTrip";

            const string insertQuery = @"
        INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
        VALUES (@IdClient, @IdTrip, @RegisteredAt, @PaymentDate)";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@IdClient", clientId);
                checkCmd.Parameters.AddWithValue("@IdTrip", tripId);
                int exists = (int)await checkCmd.ExecuteScalarAsync();

                if (exists > 0)
                    return false; 
            }

            using var insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@IdClient", clientId);
            insertCmd.Parameters.AddWithValue("@IdTrip", tripId);
            insertCmd.Parameters.AddWithValue("@RegisteredAt", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
            insertCmd.Parameters.AddWithValue("@PaymentDate", int.Parse(data.PaymentDate.ToString("yyyyMMdd")));

            var result = await insertCmd.ExecuteNonQueryAsync();
            return result > 0;
        }



        public async Task<bool> AddClient(ClientDTO client)
        {
            const string query = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
        VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await conn.OpenAsync();
            var result = await cmd.ExecuteNonQueryAsync();

            return result > 0; 
        }

        public async Task<List<ClientTripDTO>> GetTripsForClient(int clientId)
        {
            var trips = new List<ClientTripDTO>();
            const string query = @"
        SELECT t.IdTrip, t.Name
        FROM Trip t
        JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
        WHERE ct.IdClient = @id";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", clientId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new ClientTripDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }

            return trips;
        }

        private readonly IConfiguration _configuration;

        public TripsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<TripDTO>> GetTrips()
        {
            var trips = new List<TripDTO>();
            const string query = "SELECT IdTrip, Name FROM Trip";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(query, conn);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new TripDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }

            return trips;
        }

        public async Task<TripDTO?> GetTrip(int id)
        {
            const string query = "SELECT IdTrip, Name FROM Trip WHERE IdTrip = @id";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new TripDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }

            return null;
        }
    }
}