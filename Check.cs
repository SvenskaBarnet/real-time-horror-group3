namespace real_time_horror_group3;
using Npgsql;
using System.Net;
using System.Text;

public class Check(NpgsqlDataSource db)
{
    public async Task Room(HttpListenerResponse response)
    {
        await using (var cmd = db.CreateCommand())
        {
            int roomId = 1;
            int doorCount = 0;
            int windowCount = 0;

            await using var findDoors = db.CreateCommand(@"
                SELECT COUNT(id)
                FROM entry_point
	            WHERE room_id = $1 AND type = 'Door';                
                ");
            findDoors.Parameters.AddWithValue(roomId);
            var reader = await findDoors.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                doorCount = reader.GetInt32(0);
            }

            string message = $"Room has {doorCount} external doors and {windowCount} windows.";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentType = "text/plain";
            response.StatusCode = (int)HttpStatusCode.OK;

            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}

