using Npgsql;
using System.Net;

namespace real_time_horror_group3;

public class PlayerAction()
{
    public static string Lock(NpgsqlDataSource db, string type, HttpListenerRequest request, HttpListenerResponse response)
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string lockName = reader.ReadToEnd();

        var cmd = db.CreateCommand(@$"

            UPDATE entry_point
            SET is_locked = true, ""time"" = null
            WHERE name = $1 AND room_id = $2 AND type = $3;");

        cmd.Parameters.AddWithValue(lockName);
        cmd.Parameters.AddWithValue(Check.PlayerPosition(db, request, response));
        cmd.Parameters.AddWithValue(type);

        cmd.ExecuteNonQuery();
        string message = $"{type} {lockName} is now locked";

        GameEvent.RandomTrigger(db);

        return message;
    }
}
