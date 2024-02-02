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
            int doorCount = await CountEntryPoints(roomId, "Door");
            int windowCount = await CountEntryPoints(roomId, "Window");


            string message = $"Room has {doorCount} external doors and {windowCount} windows.";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentType = "text/plain";
            response.StatusCode = (int)HttpStatusCode.OK;

            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    async Task<int> CountEntryPoints(int roomId, string type)
    {
        int count = 0;
            await using var query = db.CreateCommand(@"
                SELECT COUNT(id)
                FROM entry_point
	            WHERE room_id = $1 AND type = $2;                
                ");
            query.Parameters.AddWithValue(roomId);
            query.Parameters.AddWithValue(type);
            var reader = await query.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                count = reader.GetInt32(0);
            }
        return count;
    }
}

