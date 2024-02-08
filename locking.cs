using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace real_time_horror_group3;

public class Locking(NpgsqlDataSource db)
{
    public async Task Lock(string type, int roomId, HttpListenerRequest req, HttpListenerResponse res)
    {
        StreamReader reader = new(req.InputStream, req.ContentEncoding);
        string lockName = reader.ReadToEnd();

        await using var cmd = db.CreateCommand(@$"

            UPDATE entry_point
            SET is_locked = True
            WHERE name = $1 AND room_id = $2 AND type = $3");

        cmd.Parameters.AddWithValue(lockName);
        cmd.Parameters.AddWithValue(roomId);
        cmd.Parameters.AddWithValue(type);

        await cmd.ExecuteNonQueryAsync();

        string message = $"{lockName} is now locked";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        res.ContentType = "text/plain";
        res.StatusCode = (int)HttpStatusCode.OK;

        res.OutputStream.Write(buffer, 0, buffer.Length);
        res.OutputStream.Close();
    }
}
