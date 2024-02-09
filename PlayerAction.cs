using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace real_time_horror_group3;

public class PlayerAction(NpgsqlDataSource db)
{
    public async Task<string> Lock(string type, Check check, Player player, HttpListenerRequest request, HttpListenerResponse response)
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string lockName = reader.ReadToEnd();
      
        await using var cmd = db.CreateCommand(@$"

            UPDATE entry_point
            SET is_locked = true
            WHERE name = $1 AND room_id = $2 AND type = $3;");

        cmd.Parameters.AddWithValue(lockName);
        cmd.Parameters.AddWithValue(await check.PlayerPosition(request, response, player));
        cmd.Parameters.AddWithValue(type);

        await cmd.ExecuteNonQueryAsync();
        string message = $"{type} {lockName} is now locked";

        return message;
    }
}
